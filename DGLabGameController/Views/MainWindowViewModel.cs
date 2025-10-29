using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DGLabGameController.Views
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<DockButton> DockButtons { get; } = [];

		private DockButton _selectedDockButton = null!;
		/// <summary> 当前 Dock 按钮 </summary>
		public DockButton SelectedDockButton
		{
			get => _selectedDockButton;
			set
			{
				if (_selectedDockButton != value)
				{
					_selectedDockButton = value;
					OnPropertyChanged();
					CurrentPageContent = _selectedDockButton.GetPage();
					CurrentPageTitle = _selectedDockButton.Title;
				}
			}
		}

		private string _currentPageTitle = string.Empty;
		/// <summary> 当前页面标题 </summary>
		public string CurrentPageTitle
		{
			get => _currentPageTitle;
			set
			{
				if (_currentPageTitle != value)
				{
					_currentPageTitle = value;
					OnPropertyChanged();
				}
			}
		}

		private object _currentPageContent = string.Empty;
		/// <summary> 当前页面内容 </summary>
		public object CurrentPageContent
		{
			get => _currentPageContent;
			set
			{
				if (_currentPageContent != value)
				{
					_currentPageContent = value;
					OnPropertyChanged();
				}
			}
		}

		// INotifyPropertyChanged 实现
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
	}
}