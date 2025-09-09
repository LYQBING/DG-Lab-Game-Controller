## 日志输出 API 相关

### 函数展示
```CS
// 普通日志输出
DebugHub.Log(string eventName, string content, bool verboseLog = false);

// 成功日志输出
DebugHub.Success(string eventName, string content, bool verboseLog = false);

// 警告日志输出
DebugHub.Warning(string eventName, string content, bool verboseLog = false);

// 错误日志输出
DebugHub.Error(string eventName, string content, bool verboseLog = false);
```

### 强制参数介绍
- **eventName** 日志名称或大概内容，它应该是 string 类型
- **content** 日志的详细输出内容，它应该是 string 类型
### 可选参数介绍
- **verboseLog** 详细日志开启后输出此日志，默认为关闭


## 清空日志 API 相关

### 函数展示
```CS
// 清空日志
DebugHub.Clear();
```