## 网络请求 API相关

### 函数展示
```CS
// Get网络请求
await FTPManager.GetAsync(string url);

// Post网络请求
await FTPManager.PostAsync(string url,IEnumerable<KeyValuePair<string, string>> formData);
```

### 强制参数介绍
- **url** 请求的网络地址
- **formData** 表单数据，应传入本次 Post 的所有数据

## 网络请求辅助类 API 相关

### 函数展示
```CS
// Get网络请求并进行JSON解析
await ApiHelper.GetAndParseAsync<T>(string api);

// Post网络请求并进行JSON解析
await ApiHelper.PostAndParseAsync<T>(string api,,IEnumerable<KeyValuePair<string, string>> formData);
```

### 强制参数介绍
- **T** 将返回的 json 转换为 T 类型
- **api** 请求的网络地址
- **formData** 表单数据，应传入本次 Post 的所有数据