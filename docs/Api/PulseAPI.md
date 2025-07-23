## 波形 API 简介
### 函数展示：获取相关
```CS
// 获取波形列表
DGLab.GetAllPulseList( );

// 获取游戏当前波形ID
DGLab.GetPulseID( );
```
### 函数展示：设置相关
```CS
// 设置游戏当前波形ID
DGLab.SetPulseID(string pulseId);

// 设置游戏当前波形ID
DGLab.SetPulseID(List<string> pulseIds);
```
### 强制参数介绍
#### pulseId 
波形ID
#### pulseIds
波形ID列表
### 响应数据
```CS
public class PulseId
{
	public int Status { get; set; }
	public string? Code { get; set; }
	// 当前正在播放的波形ID
	public string? CurrentPulseId { get; set; }
	// 当前的波形列表
	public string[]? PulseIdList { get; set; }
}
```
