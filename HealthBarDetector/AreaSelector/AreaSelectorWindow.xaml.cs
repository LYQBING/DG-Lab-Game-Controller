using System.Windows;
using System.Windows.Input;

namespace HealthBarDetector
{
    /// <summary>
    /// 区域选择窗口，支持框选、拖动、缩放和确认/取消操作
    /// </summary>
    public partial class AreaSelectorWindow : Window
    {
        public Rect SelectedRect { get; private set; }
        private readonly SelectionManager selectionManager;

        public AreaSelectorWindow()
        {
            InitializeComponent();

            // 初始化选区管理器
            selectionManager = new SelectionManager(canvas, selectionRect, resizeHandle, buttonPanel);

            // 绑定事件
            MouseLeftButtonDown += selectionManager.OnMouseLeftButtonDown;
            MouseMove += selectionManager.OnMouseMove;
            MouseLeftButtonUp += selectionManager.OnMouseLeftButtonUp;

            resizeHandle.MouseLeftButtonDown += selectionManager.OnResizeHandleMouseLeftButtonDown;
            resizeHandle.MouseMove += selectionManager.OnResizeHandleMouseMove;
            resizeHandle.MouseLeftButtonUp += selectionManager.OnResizeHandleMouseLeftButtonUp;

            selectionRect.MouseLeftButtonDown += selectionManager.OnSelectionRectMouseLeftButtonDown;
            selectionRect.MouseMove += selectionManager.OnSelectionRectMouseMove;
            selectionRect.MouseLeftButtonUp += selectionManager.OnSelectionRectMouseLeftButtonUp;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedRect = selectionManager.GetSelectedScreenRect();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}