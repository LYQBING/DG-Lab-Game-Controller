# 展示型弹窗 API 相关

### 函数展示
```CS
// 输入型弹窗
new MessageDialog(string title, string message, string button1Text, Action<MessageDialog> button1Action, string? button2Text = null, Action<MessageDialog>? button2Action = null, Window? owner = null).ShowDialog();
```

### 强制参数介绍
- **title** 弹窗标题文本
- **message** 弹窗内容文本
- **button1Text** 按钮一文本，它通常是确定按钮
- **button1Action** 按钮一事件，你应该为它传入一个委托

### 可选参数介绍
- **button2Text** 按钮二文本，它通常是取消按钮
- **button2Action** 按钮二事件，你应该为它传入一个委托
- **owner** 弹窗显示位置，默认显示在主程序中央

### 参考代码展示

```CS
new MessageDialog("标题", "这里是弹窗内容", "好的", data =>
{
	// 按钮一事件：委托写法A
	data.Close();
},
// 按钮二事件：委托写法B
"取消", data => data.Close()).ShowDialog();
```

## 输入型弹窗 API 相关

### 函数展示
```CS
// 输入型弹窗
new InputDialog(string title, string message, string inputText, string button1Text, Action<InputDialog> button1Action, string button2Text, Action<InputDialog> button2Action, Window owner = null).ShowDialog();
```

### 强制参数介绍
- **title** 弹窗标题文本
- **message** 弹窗内容文本
- **inputText** 输入框默认显示文本
- **button1Text** 按钮一文本，它通常是确定按钮
- **button1Action** 按钮一事件，你应该为它传入一个委托
- **button2Text** 按钮二文本，它通常是取消按钮
- **button2Action** 按钮二事件，你应该为它传入一个委托

### 可选参数介绍
- **owner** 弹窗显示位置，默认显示在主程序中央

### 参考代码展示

```CS
new InputDialog("标题", "这里是弹窗内容", "默认输入内容", "好的", data =>
{
	// 按钮一事件：委托写法A
	data.Close();
},
// 按钮二事件：委托写法B
"取消", data => data.Close()).ShowDialog();
```