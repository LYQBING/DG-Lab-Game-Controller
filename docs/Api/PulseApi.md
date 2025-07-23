## 游戏波形 API 简介
### 函数展示：获取相关
```CS
// 获取游戏当前波形 ID
DGLab.GetPulseID( );

// 获取邮箱当前波形 ID 列表
DGLab.GetAllPulseList( );
```
### 函数展示：设置相关
```CS
// 设置游戏当前波形 ID
DGLab.SetPulseID(string pulseId);

// 设置游戏当前波形 ID 列表
DGLab.SetPulseID(List<string> pulseIds);
```
### 强制参数介绍
- **pulseId** 您想要指定的波形ID，它应该是 string 类型
- **pulseIds** 您想要设置的波形ID列表，它应该是 string 类型的数组
### 响应数据
```CS
public class PulseId
{
	public int Status { get; set; }
	public string? Code { get; set; }
	// 当前正在播放的波形ID
	public string? CurrentPulseId { get; set; }
	// 当前正在播放的波形 ID 列表
	public string[]? PulseIdList { get; set; }
}
```
