using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Specialized;
using DGLabGameController.Core.Debug;

namespace DGLabGameController
{
	public partial class LogPage : UserControl
	{
		public LogPage()
		{
			InitializeComponent();
			RefreshLogDocument();

			DebugHub.Logs.CollectionChanged += RefreshLogDocument;
		}

		// 日志格式化
		private void RefreshLogDocument(object ?sender = null, NotifyCollectionChangedEventArgs ?e = null)
		{
			var doc = new FlowDocument();
			foreach (var log in DebugHub.Logs)
			{
				var para = new Paragraph { Margin = new Thickness(0, 0, 0, 4) };
				para.Inlines.Add(new Run(log.TimeString + " ") { Foreground = LogItem.TimeBrush });
				para.Inlines.Add(new Run("[" + log.EventName + "] ") { Foreground = log.EventBrush });
				para.Inlines.Add(new Run(log.Content) { Foreground = log.ContentBrush });
				doc.Blocks.Add(para);
			}
			LogRichTextBox.Document = doc;
			LogRichTextBox.ScrollToEnd();

		}

		// 日志保存按钮
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
				FileName = "Log.txt"
			};
			if (dlg.ShowDialog() == true)
			{
				var text = new TextRange(LogRichTextBox.Document.ContentStart, LogRichTextBox.Document.ContentEnd).Text;
				System.IO.File.WriteAllText(dlg.FileName, text);
			}
		}

		// 清空日志按钮
		private void DeleteButton_Click(object? sender = null, RoutedEventArgs? e = null) => DebugHub.Clear();
	}
}
