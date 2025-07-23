## 一键开火 API 展示
### 函数展示：一键开火
```CS
// 一键开火
DGLab.Fire(int strength, int time, bool overrides, string pulseId);
```
### 可选参数介绍
#### strength 
一键开火的惩罚强度，上限由服务器或郊狼客户端决定。默认为 20。
#### time
一键开火的惩罚时间。单位：毫秒，默认为5000，最高30000。
#### overrides
当处于多次开火惩罚时，是否重置之前的惩罚计时？开启则重新计算惩罚时间，否则将惩罚时间进行叠加。默认为false。
#### pulseId
一键开火的波形ID，默认使用当前的波形。
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
