# 一键开火 API 相关

### 函数展示
```CS
// 一键开火
DGLab.Fire(int strength, int time, bool overrides, string pulseId);
```

### 可选参数介绍
- **strength** 一键开火的惩罚强度，上限由服务器或郊狼客户端决定。默认为 20。
- **time** 一键开火的惩罚时间。单位：毫秒，默认为5000，最高30000。
- **overrides** 当处于多次开火惩罚时，是否重置之前的惩罚计时？开启则重新计算惩罚时间，否则将惩罚时间进行叠加。默认为false。
- **pulseId** 一键开火的波形ID，默认使用当前的波形。

### 响应数据

```CS
public class FireJson
{
	public int Status { get; set; }
	public string? Code { get; set; }
	public string? Message { get; set; }
	public List<Warnings>? Warnings { get; set; }
	public string[]? SuccessClientIds { get; set; }
}
```

## 游戏惩罚数据 API 相关

### 函数展示：默认惩罚

```cs
// 增加默认惩罚强度
DGLab.SetStrength.Add(int value);

// 减少默认惩罚强度
DGLab.SetStrength.Sub(int value);

// 设置默认惩罚强度
DGLab.SetStrength.Set(int value);
```

### 函数展示：随机惩罚

```cs
// 增加随机随机强度
DGLab.SetRandomStrength.Add(int value);

// 减少随机惩罚强度
DGLab.SetRandomStrength.Sub(int value);

// 设置随机惩罚强度
DGLab.SetRandomStrength.Set(int value);
```

### 函数展示：获取惩罚数据

```cs
// 获取游戏强度信息
DGLab.GetStrengthConfig( );
```

### 强制参数介绍

- **value** 所设定的惩罚值，它应该是 int 类型

### 响应数据

```cs
public class StrengthConfigJson
{
	public int Status { get; set; }
	public string? Code { get; set; }
	public string? Message { get; set; }
	public List<Warnings>? Warnings { get; set; }
	public StrengthConfig? StrengthConfig { get; set; }
}
```

## 游戏波形 API 相关

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

## 服务器响应数据 API 相关

### 函数展示
```CS
// 获取服务器响应数据
DGLab.GetServerResponse( );
```

### 响应数据
```CS
public class ServerResponse
{
	public int Status { get; set; }
	public string? Code { get; set; }
	public string? Message { get; set; }
	public List<Warnings>? Warnings { get; set; }
	public int MinApiVersion { get; set; }
	public int MaxApiVersion { get; set; }
}
```

## 游戏响应数据 API 相关

### 函数展示

```CS
// 获取游戏响应数据
DGLab.GetGameResponse( );
```

### 响应数据

```CS
public class GameResponse
{
	public int Status { get; set; }
	public string? Code { get; set; }
	public string? Message { get; set; }
	public List<Warnings>? Warnings { get; set; }
	public StrengthConfig? StrengthConfig { get; set; }
	public GameConfig? GameConfig { get; set; }
	public ClientStrength? ClientStrength { get; set; }
	public string? CurrentPulseId { get; set; }
}
```
