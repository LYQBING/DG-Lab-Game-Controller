## 游戏惩罚 API 展示
### 函数展示：默认惩罚
```CS
// 增加默认惩罚强度
DGLab.SetStrength.Add(int value);

// 减少默认惩罚强度
DGLab.SetStrength.Sub(int value);

// 设置默认惩罚强度
DGLab.SetStrength.Set(int value);
```
### 函数展示：随机惩罚
```CS
// 增加随机随机强度
DGLab.SetRandomStrength.Add(int value);

// 减少随机惩罚强度
DGLab.SetRandomStrength.Sub(int value);

// 设置随机惩罚强度
DGLab.SetRandomStrength.Set(int value);
```
### 函数展示：获取惩罚数据
```CS
// 获取游戏强度信息
DGLab.GetStrengthConfig( );
```
### 强制参数介绍
#### value
所设定的惩罚值，它应该是 int 类型
### 响应数据
```CS
public class StrengthConfigJson
{
	public int Status { get; set; }
	public string? Code { get; set; }
	public string? Message { get; set; }
	public List<Warnings>? Warnings { get; set; }
	public StrengthConfig? StrengthConfig { get; set; }
}
```
