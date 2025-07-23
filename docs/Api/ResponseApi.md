## 服务器响应数据 API 简介

### 函数展示：获取相关
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

## 游戏响应数据 API 简介

### 函数展示：获取相关

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
