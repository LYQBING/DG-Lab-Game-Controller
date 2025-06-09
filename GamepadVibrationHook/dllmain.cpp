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


// 前置声明
static void PipeSenderThread();
static void HookAllXInputDlls();
static void SendHookResultToPipe(bool success, const std::string& message);

// 定义 XInputSetState 函数指针类型，存储所有原始函数指针
typedef DWORD(WINAPI* XInputSetState_t)(DWORD, XINPUT_VIBRATION*);
std::vector<XInputSetState_t> fpXInputSetStateList;

// 原子变量存储最新震动值
static std::atomic<WORD> latestLeft{ 0 };
static std::atomic<WORD> latestRight{ 0 };
static std::atomic<bool> running{ false };
static std::thread pipeThread;

// DLL 模块的运行入口
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	// 初始化 MinHook 库
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		running = true;
		pipeThread = std::thread(PipeSenderThread);

		// 确保 MinHook 库初始化成功
		if (MH_Initialize() == MH_OK) HookAllXInputDlls();
		else SendHookResultToPipe(false, "MinHook Initialization failed");

		break;

	case DLL_PROCESS_DETACH:
		running = false;
		if (pipeThread.joinable()) pipeThread.join();
		MH_Uninitialize();
		break;
	}
	return TRUE;
}

#pragma region 震动事件相关

// 震动数据结构
struct VibrationData
{
	WORD left;
	WORD right;
	bool operator!=(const VibrationData& other) const
	{
		return left != other.left || right != other.right;
	}
};

// 持久化管道线程，只发送变化的最新值
void PipeSenderThread()
{
	HANDLE hPipe = INVALID_HANDLE_VALUE;
	VibrationData lastSent{ 0, 0 };
	while (running)
	{
		// 保持管道连接
		if (hPipe == INVALID_HANDLE_VALUE || hPipe == NULL)
		{
			hPipe = CreateFileA("\\\\.\\pipe\\XInputVibrationPipe", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
			if (hPipe == INVALID_HANDLE_VALUE || hPipe == NULL)
			{
				std::this_thread::sleep_for(std::chrono::milliseconds(100));
				continue;
			}
		}

		// 获取最新值
		VibrationData current{ latestLeft.load(), latestRight.load() };
		if (current != lastSent)
		{
			// 发送格式：[1字节类型][2字节left][2字节right]
			BYTE buf[5]{ 2 };
			memcpy(buf + 1, &current.left, 2);
			memcpy(buf + 3, &current.right, 2);
			DWORD written = 0;
			if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE)
			{
				BOOL ok = WriteFile(hPipe, buf, 5, &written, NULL);
				if (!ok)
				{
					CloseHandle(hPipe);
					hPipe = INVALID_HANDLE_VALUE;
				}
				else
				{
					lastSent = current;
				}
			}
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(2)); // 2ms轮询
	}

	// 关闭管道句柄
	if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE)
	{
		CloseHandle(hPipe);
	}
}

// 只在变化时更新最新值
static void SendVibrationToPipe(WORD left, WORD right)
{
	latestLeft.store(left);
	latestRight.store(right);
}

// 注入后的 XInputSetState 函数
static DWORD WINAPI HookedXInputSetState(DWORD dwUserIndex, XINPUT_VIBRATION* pVibration)
{
	if (pVibration)
	{
		SendVibrationToPipe(pVibration->wLeftMotorSpeed, pVibration->wRightMotorSpeed);
	}

	// 调用所有原始 XInputSetState
	if (!fpXInputSetStateList.empty() && fpXInputSetStateList[0])
	{
		return fpXInputSetStateList[0](dwUserIndex, pVibration);
	}

	return ERROR_PROC_NOT_FOUND;
}

#pragma endregion

#pragma region 注入 DLL 模块相关

// 遍历进程中所有已加载模块，查找并 Hook 所有 XInput DLL
static void HookAllXInputDlls()
{
	HMODULE hMods[1024];
	HANDLE hProcess = GetCurrentProcess();
	DWORD cbNeeded = 0;
	std::string successDlls;
	std::string errorMsg;
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

				// 检查是否是 XInput DLL
				if (nameLen > 4 && _strnicmp(pFileName, "xinput", 6) == 0 &&_stricmp(pFileName + nameLen - 4, ".dll") == 0) 
				{
					// 找到 XInput DLL，尝试 Hook XInputSetState 函数
					void* pTarget = GetProcAddress(hMods[i], "XInputSetState");
					if (pTarget) 
					{
						XInputSetState_t orig = nullptr;
						if (MH_CreateHook(pTarget, &HookedXInputSetState, reinterpret_cast<LPVOID*>(&orig)) == MH_OK && MH_EnableHook(pTarget) == MH_OK)
						{
							// 如果 Hook 成功，记录成功的 DLL 名称
							if (!successDlls.empty()) successDlls += ";";
							successDlls += pFileName;
							hooked = true;

							// 将原始函数指针存储到列表中
							if (orig) fpXInputSetStateList.push_back(orig);
						}
						else 
						{
							// 尝试 Hook 失败
							if (!errorMsg.empty()) errorMsg += "; ";
							errorMsg += pFileName;
						}
					}
					else 
					{
						// 寻找 XInputSetState 函数失败
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

	// 发送结果到管道
	if (hooked) 
	{
		SendHookResultToPipe(true, successDlls);
	}
	else 
	{
		SendHookResultToPipe(false, errorMsg.empty() ? "Could not find XInput DLL" : errorMsg);
	}
}


// 向命名管道发送注入事件的结果 
static void SendHookResultToPipe(bool success, const std::string& message) {
	for (int attempt = 0; attempt < 3; ++attempt) {
		HANDLE hPipe = CreateFileA("\\\\.\\pipe\\XInputVibrationPipe", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
		if (hPipe != NULL && hPipe != INVALID_HANDLE_VALUE) {
			BYTE result = success ? 1 : 0;
			DWORD len = static_cast<DWORD>(message.size());
			DWORD written = 0;
			BOOL ok = WriteFile(hPipe, &result, 1, &written, NULL);
			ok = ok && WriteFile(hPipe, &len, 4, &written, NULL);
			if (len > 0) {
				ok = ok && WriteFile(hPipe, message.data(), len, &written, NULL);
			}
			CloseHandle(hPipe);
			if (ok) return;
		}
		else {
			std::this_thread::sleep_for(std::chrono::milliseconds(50));
		}
	}
}

#pragma endregion