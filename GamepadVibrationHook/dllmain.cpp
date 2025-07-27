#include "pch.h"
#include <windows.h>
#include <Xinput.h>
#include <Psapi.h>
#include <string>
#include <thread>
#include <atomic>
#include <chrono>
#include <vector>
#include "MinHook.h"

// =====================
//  前置声明
// =====================
static void PipeSenderThread();
static void HookAllXInputDlls();
static void SendHookResultToPipe(bool success, const std::string& message);

// =====================
//  类型定义与全局变量
// =====================

// 定义 XInputSetState 函数指针类型，便于后续保存原始函数指针
typedef DWORD(WINAPI* XInputSetState_t)(DWORD, XINPUT_VIBRATION*);
// 保存所有被 hook 的 XInputSetState 原始函数指针
std::vector<XInputSetState_t> fpXInputSetStateList;

// 原子变量用于线程安全地存储最新的震动值
static std::atomic<WORD> latestLeft{ 0 };
static std::atomic<WORD> latestRight{ 0 };
// 控制管道线程的运行状态
static std::atomic<bool> running{ false };
// 管道线程对象
static std::thread pipeThread;

// =====================
//  DLL 入口函数
// =====================

/**
 * DLL 主入口函数
 * @param hModule DLL 模块句柄
 * @param ul_reason_for_call 调用原因（加载/卸载等）
 * @param lpReserved 保留参数
 */
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		// DLL 被加载时，启动管道线程并初始化 MinHook
		running = true;
		pipeThread = std::thread(PipeSenderThread);

		// 初始化 MinHook 并尝试 hook 所有 XInput DLL
		if (MH_Initialize() == MH_OK)
			HookAllXInputDlls();
		else
			SendHookResultToPipe(false, "MinHook Initialization failed");
		break;

	case DLL_PROCESS_DETACH:
		// DLL 卸载时，停止管道线程并清理 MinHook
		running = false;
		if (pipeThread.joinable()) pipeThread.join();
		MH_Uninitialize();
		break;
	}
	return TRUE;
}

#pragma region 震动事件相关

/**
 * 震动数据结构体，用于比较和传递震动值
 */
struct VibrationData
{
	WORD left;
	WORD right;
	// 重载不等于运算符，便于判断震动值是否发生变化
	bool operator!=(const VibrationData& other) const
	{
		return left != other.left || right != other.right;
	}
};

/**
 * 管道发送线程
 * 持续检测最新震动值，只有发生变化时才通过命名管道发送到主程序
 * 发送格式：[1字节类型][2字节left][2字节right]
 */
void PipeSenderThread()
{
	HANDLE hPipe = INVALID_HANDLE_VALUE;
	VibrationData lastSent{ 0, 0 }; // 上一次发送的震动值

	while (running)
	{
		// 保持管道连接，若断开则重试
		if (hPipe == INVALID_HANDLE_VALUE || hPipe == NULL)
		{
			hPipe = CreateFileA("\\\\.\\pipe\\XInputVibrationPipe", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
			if (hPipe == INVALID_HANDLE_VALUE || hPipe == NULL)
			{
				// 连接失败，等待一段时间后重试
				std::this_thread::sleep_for(std::chrono::milliseconds(100));
				continue;
			}
		}

		// 获取当前最新的震动值
		VibrationData current{ latestLeft.load(), latestRight.load() };
		// 仅当震动值发生变化时才发送
		if (current != lastSent)
		{
			// 构造发送缓冲区
			BYTE buf[5]{ 2 }; // 第一个字节为类型2，表示震动数据
			memcpy(buf + 1, &current.left, 2);  // 2字节left
			memcpy(buf + 3, &current.right, 2); // 2字节right
			DWORD written = 0;
			if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE)
			{
				BOOL ok = WriteFile(hPipe, buf, 5, &written, NULL);
				if (!ok)
				{
					// 写入失败，关闭管道句柄，下次重连
					CloseHandle(hPipe);
					hPipe = INVALID_HANDLE_VALUE;
				}
				else
				{
					// 记录本次已发送的值
					lastSent = current;
				}
			}
		}
		// 2ms 轮询一次，兼顾实时性与性能
		std::this_thread::sleep_for(std::chrono::milliseconds(2));
	}

	// 线程退出时关闭管道句柄
	if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE)
	{
		CloseHandle(hPipe);
	}
}

/**
 * 更新最新震动值（线程安全）
 * 只在 HookedXInputSetState 被调用时触发
 * @param left 左马达强度
 * @param right 右马达强度
 */
static void SendVibrationToPipe(WORD left, WORD right)
{
	latestLeft.store(left);
	latestRight.store(right);
}

/**
 * Hook 后的 XInputSetState 实现
 * 拦截所有 XInputSetState 调用，记录震动值并转发给原始函数
 * @param dwUserIndex 控制器索引
 * @param pVibration 震动参数
 * @return 原始 XInputSetState 的返回值
 */
static DWORD WINAPI HookedXInputSetState(DWORD dwUserIndex, XINPUT_VIBRATION* pVibration)
{
	if (pVibration)
	{
		// 记录最新震动值
		SendVibrationToPipe(pVibration->wLeftMotorSpeed, pVibration->wRightMotorSpeed);
	}

	// 调用原始 XInputSetState
	if (!fpXInputSetStateList.empty() && fpXInputSetStateList[0])
	{
		return fpXInputSetStateList[0](dwUserIndex, pVibration);
	}

	// 未找到原始函数，返回错误
	return ERROR_PROC_NOT_FOUND;
}

#pragma endregion

#pragma region 注入 DLL 模块相关

/**
 * 遍历进程中所有已加载模块，查找并 Hook 所有 XInput DLL 的 XInputSetState 函数
 * 支持多种 XInput 版本共存
 */
static void HookAllXInputDlls()
{
	HMODULE hMods[1024];
	HANDLE hProcess = GetCurrentProcess();
	DWORD cbNeeded = 0;
	std::string successDlls; // 记录 hook 成功的 DLL 名称
	std::string errorMsg;    // 记录 hook 失败的 DLL 名称
	bool hooked = false;

	// 枚举当前进程的所有模块
	if (EnumProcessModules(hProcess, hMods, sizeof(hMods), &cbNeeded))
	{
		// 遍历所有模块，查找 XInput DLL
		for (unsigned int i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
		{
			// 获取模块文件名
			char szModName[MAX_PATH] = { 0 };
			if (GetModuleFileNameA(hMods[i], szModName, sizeof(szModName) / sizeof(char)))
			{
				const char* pFileName = strrchr(szModName, '\\');
				pFileName = pFileName ? (pFileName + 1) : szModName;
				size_t nameLen = strlen(pFileName);

				// 检查是否是 XInput DLL（文件名以 xinput 开头且以 .dll 结尾）
				if (nameLen > 4 && _strnicmp(pFileName, "xinput", 6) == 0 && _stricmp(pFileName + nameLen - 4, ".dll") == 0)
				{
					// 找到 XInput DLL，尝试 Hook XInputSetState 函数
					void* pTarget = GetProcAddress(hMods[i], "XInputSetState");
					if (pTarget)
					{
						XInputSetState_t orig = nullptr;
						// 创建并启用 hook
						if (MH_CreateHook(pTarget, &HookedXInputSetState, reinterpret_cast<LPVOID*>(&orig)) == MH_OK &&
							MH_EnableHook(pTarget) == MH_OK)
						{
							// 记录 hook 成功的 DLL
							if (!successDlls.empty()) successDlls += ";";
							successDlls += pFileName;
							hooked = true;

							// 保存原始函数指针
							if (orig) fpXInputSetStateList.push_back(orig);
						}
						else
						{
							// hook 失败，记录错误
							if (!errorMsg.empty()) errorMsg += "; ";
							errorMsg += pFileName;
						}
					}
					else
					{
						// 未找到 XInputSetState，记录错误
						if (!errorMsg.empty()) errorMsg += "; ";
						errorMsg += pFileName;
					}
				}
			}
		}
	}
	else
	{
		errorMsg = "Failed to enumerate process modules";
	}

	// 发送 hook 结果到主程序
	if (hooked)
	{
		SendHookResultToPipe(true, successDlls);
	}
	else
	{
		SendHookResultToPipe(false, errorMsg.empty() ? "Could not find XInput DLL" : errorMsg);
	}
}

/**
 * 向命名管道发送注入事件的结果（成功/失败及相关信息）
 * @param success 是否成功
 * @param message 结果信息
 */
static void SendHookResultToPipe(bool success, const std::string& message)
{
	for (int attempt = 0; attempt < 3; ++attempt)
	{
		HANDLE hPipe = CreateFileA("\\\\.\\pipe\\XInputVibrationPipe", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
		if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE)
		{
			BYTE result = success ? 1 : 0;
			DWORD len = static_cast<DWORD>(message.size());
			DWORD written = 0;
			// 发送1字节结果类型
			BOOL ok = WriteFile(hPipe, &result, 1, &written, NULL);
			// 发送4字节消息长度
			ok = ok && WriteFile(hPipe, &len, 4, &written, NULL);
			// 发送消息内容
			if (len > 0)
			{
				ok = ok && WriteFile(hPipe, message.data(), len, &written, NULL);
			}
			CloseHandle(hPipe);
			if (ok) return; // 发送成功则退出
		}
		else
		{
			// 连接失败，稍后重试
			std::this_thread::sleep_for(std::chrono::milliseconds(50));
		}
	}
}

#pragma endregion