## 在创建脚本之前

感谢您选择 GameValueDetector 创建您的郊狼游戏脚本。它将给您带来简单迅速且强大的脚本编辑体验！

如果您是小白也不必担心，对于创建 GameValueDetector 脚本，您只需要会使用 CE 查找内存基址及 JSON 编写即可，通常您只需要 15-30 分钟即可出师！

脚本采用 JSON 文件描述，主要包含三个层级：
- **GameMonitorConfig**：整个监控脚本的配置入口
- **MonitorItem**：每个需要监控的内存项
- **ScenarioPunishment**：每个监控项下的触发情景与惩罚动作
---

## 参数详解

### 1. GameMonitorConfig（监控脚本配置）

| 参数名         | 类型            | 说明               | 示例值      |
| ----------- | ------------- | ---------------- | -------- |
| Description | string        | 脚本介绍：启动成功后输出介绍     | "监控血量变化" |
| ProcessName | string        | 目标进程名称：如游戏进程名       | "Game"   |
| Is32Bit     | bool          | 是否为32位程序                  | true     |
| Monitors    | MonitorItem[] | 监控项列表：每个元素为一个监控项  |          |
```json
{
	"Description": "这里是你的脚本的介绍",
	"ProcessName": "这里是游戏进程的名称",
    "Is32Bit": 通常忽略此项，除非你要适配的程序或您的设备为 32 位
	"Monitors": 
	[
	    {
			这里是第一项监控内容
			如何编辑这里？请参考 MonitorItem
	    },
	    {
			这里是第二项监控内容
			也就是说：你有多少需要监控的就可以向下创建多少个
	    }
	]
}
```
---

### 2. MonitorItem（监控项）

| 参数名         | 类型                   | 说明                           | 示例值               |
| ----------- | -------------------- | ---------------------------- | ----------------- |
| Module      | string               | 监控的 DLL 名称                   | "Game.dll"        |
| BaseAddress | string               | 基地址                          | "0x12345678"      |
| Offsets     | string[]             | 偏移列表                         | ["0", "10", "20"] |
| Type        | string               | 监控项类型：如 Int32, Float, String | "Float"           |
| Scenarios   | ScenarioPunishment[] | 惩罚情景列表：每个元素为一个情景项            |                   |
| History     | HistoryValue         | 历史数据类，可以存放初始数据               | 通常留空              |
```json
"Monitors": 
[
	{
		"Module": "监控的 DLL 名称",
		"BaseAddress": "基地址",
		"Offsets": ["偏移1", "偏移2", "偏移3"], 
		"Type": "所监控项的类型",
		"Scenarios": 
		[
			{
				这里是第一项惩罚数据
				如何编辑这里？请参考 ScenarioPunishment
			},
			{
				这里是第二项监控内容
				也就是说：你有多少需要监控的就可以向下创建多少个
			},
		]
		"History":
		{
			"LastValue": 上次值
			"InitialValue":当前值
			"MaxValue":最大值
		}
    },
]
```
---

### 3. ScenarioPunishment（情景惩罚配置）

| 参数名              | 类型     | 说明                                          | 示例值              |
| ---------------- | ------ | ------------------------------------------- | ---------------- |
| Scenario         | string | 触发条件类型（如 Changed, Increased, GreaterThan 等） | "GreaterThan"    |
| CompareValue     | float  | 比较参数，部分情景需要（如大于某值、正则匹配等）                    | 100              |
| Action           | string | 惩罚动作（如 SetStrengthSet, Fire 等）              | "SetStrengthSet" |
| ActionMode       | string | 惩罚模式（如 default, percent, diff 等）            | "percent"        |
| ActionValue      | float  | 惩罚模式的参数值                                    | 50.0             |
| Time             | int    | 惩罚持续时间（如开火时长，单位毫秒）                          | 3000             |
| Overrides        | bool   | 是否覆盖参数（如开火时是否强制覆盖）                          | false            |
| AccumulatedValue | float  | 累计处罚值，可作为首次处罚的额外值填写                         | 1                |
```json
"Scenarios": 
[
	{
          "Scenario": "这里是触发的条件",
          "CompareValue": "为 触发的条件 传参使用，参考 Scenario 常用值",
          "Action": "当触发时所降下的惩罚，参考 Action 常用值",
          "ActionMode": "如何计算惩罚参数，参考 ActionMode 常用值",
          "ActionValue": 为 计算惩罚参数传参，参考 ActionMode 常用值,
          "Time": 惩罚持续时间，通常用于 开火惩罚，一般留空,
          "Overrides": 是否覆盖参数，通常用于 开火惩罚，一般留空
          "AccumulatedValue":通常无视，留空
	},
]
```
---

## 常用场景与参数选择

### 1. Scenario（情景类型）常用值

| 类型                 | 说明         | CompareValue 示例 |
| ------------------ | ---------- | --------------- |
| Changed            | 值发生变化时触发   | 无需填写，默认为 0      |
| Increased          | 值增加X时处罚    | 无需填写，默认为 0      |
| Decreased          | 值减少X时触发    | 无需填写，默认为 0      |
| EqualTo            | 值等于某个数时触发  | 100             |
| GreaterThan        | 值大于某个数时触发  | 100             |
| LessThan           | 值小于某个数时触发  | 50              |
| NotEqualTo         | 值不等于某个值时触发 | 123             |
| PercentLessThan    | 值小于最大百分比时  | 0.96            |
| PercentGreaterThan | 值大于最大百分比时  | 0.69            |
**注意：**  
- 如果 Scenario 类型不需要 CompareValue，可省略或填 null。
- 如果 Scenario 类型需要 CompareValue，必须填写对应值。

---

### 2. Action（惩罚动作）常用值

| 类型                   | 说明      |
| -------------------- | ------- |
| SetStrengthSet       | 设置强度值   |
| SetStrengthAdd       | 增加强度值   |
| SetStrengthSub       | 减少强度值   |
| SetRandomStrengthSet | 设置随机强度值 |
| SetRandomStrengthAdd | 增加随机强度值 |
| SetRandomStrengthSub | 减少随机强度值 |
| Fire                 | 执行开火动作  |

---

### 3. ActionMode（惩罚模式）常用值

| 类型                    | 说明                     | ActionValue 填写示例    |
| --------------------- | ---------------------- | ------------------- |
| Default               | 返回基值 乘 目标值             | 倍率：建议 0.1 - 1 (可超出) |
| Fixed                 | 返回 目标值                 |                     |
| Diff                  | 返回 内存值 与 上次值 的差值 乘 目标值 | 倍率：建议 0.1 - 1 (可超出) |
| MemoryValue           | 返回 内存值 乘 目标值           |                     |
| Percent               | 当前内存的值 与 最大值 的 正百分比    | 倍率：建议 1 (可超出)       |
| Reverse_Percent       | 当前内存的值 与 最大值 的 反百分比    | 倍率：建议 1 (可超出)       |
| ChangePercent         | 当前内存的值 与 上次值 的 变化正百分比  | 倍率：建议 1 (可超出)       |
| Reverse_ChangePercent | 当前内存的值 与 上次值 的 变化反百分比  | 倍率：建议 1 (可超出)       |
**注意：**  
- 填写 ActionValue 是倍数时：通常你只需要输入 0.1-1 的范围 (可超出) ，建议根据实际情况配置。
- Percent 的附加说明：假设你使用它检测血条，它会自动保存值的最大值。惩罚将根据用户所设置的值进行计算：血量越多则惩罚越强。公式：用户设置的值 ⨉ 百分比 ⨉ 倍率 = 输出
- ChangePercent 的附加说明：他与 Percent 类似，但它所计算的是 [( 上次的值 - 当前值 )/ 最大值 ] 得到变化的百分比。公式：用户设置的值 ⨉ 百分比 ⨉ 倍率 = 输出
