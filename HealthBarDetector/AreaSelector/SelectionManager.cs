using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace HealthBarDetector
{
    /// <summary>
    /// 负责选区的框选、拖动、缩放等逻辑
    /// </summary>
    public class SelectionManager(Canvas canvas, Rectangle selectionRect, Rectangle resizeHandle, FrameworkElement buttonPanel)
    {

        // 框选相关
        private Point startPoint;
        private bool isSelecting = false;

        // 缩放相关
        private bool isResizing = false;
        private Point resizeStartPoint;
        private double origWidth, origHeight;

        // 拖动相关
        private bool isDraggingSelection = false;
        private Point dragSelectionStartPoint;
        private double dragSelectionOrigLeft, dragSelectionOrigTop;

        // 框选相关事件
        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectionRect.Visibility == Visibility.Visible) return;

            isSelecting = true;
            startPoint = e.GetPosition(canvas);

            Canvas.SetLeft(selectionRect, startPoint.X);
            Canvas.SetTop(selectionRect, startPoint.Y);
            selectionRect.Width = 0;
            selectionRect.Height = 0;
            selectionRect.Visibility = Visibility.Visible;

            buttonPanel.Visibility = Visibility.Visible;
            UpdateHandleAndButton();

            (canvas.Parent as Window)?.CaptureMouse();
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                var pos = e.GetPosition(canvas);
                var x = Math.Max(0, Math.Min(pos.X, startPoint.X));
                var y = Math.Max(0, Math.Min(pos.Y, startPoint.Y));
                var w = Math.Abs(pos.X - startPoint.X);
                var h = Math.Abs(pos.Y - startPoint.Y);

                w = Math.Min(w, canvas.ActualWidth - x);
                h = Math.Min(h, canvas.ActualHeight - y);

                Canvas.SetLeft(selectionRect, x);
                Canvas.SetTop(selectionRect, y);
                selectionRect.Width = w;
                selectionRect.Height = h;

                UpdateHandleAndButton();
            }
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isSelecting) return;
            isSelecting = false;
            (canvas.Parent as Window)?.ReleaseMouseCapture();

            if (selectionRect.Width < 5 || selectionRect.Height < 5)
            {
                selectionRect.Visibility = Visibility.Collapsed;
                buttonPanel.Visibility = Visibility.Collapsed;
                return;
            }

            UpdateHandleAndButton();
            resizeHandle.Visibility = Visibility.Visible;
            buttonPanel.Visibility = Visibility.Visible;
        }

        // 缩放相关事件
        public void OnResizeHandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isResizing = true;
            resizeStartPoint = e.GetPosition(canvas);
            origWidth = selectionRect.Width;
            origHeight = selectionRect.Height;
            resizeHandle.CaptureMouse();
            e.Handled = true;
        }

        public void OnResizeHandleMouseMove(object sender, MouseEventArgs e)
        {
            if (!isResizing) return;
            var pos = e.GetPosition(canvas);
            var dx = pos.X - resizeStartPoint.X;
            var dy = pos.Y - resizeStartPoint.Y;

            double x = Canvas.GetLeft(selectionRect);
            double y = Canvas.GetTop(selectionRect);

            double newWidth = Math.Max(10, origWidth + dx);
            double newHeight = Math.Max(10, origHeight + dy);

            newWidth = Math.Min(newWidth, canvas.ActualWidth - x);
            newHeight = Math.Min(newHeight, canvas.ActualHeight - y);

            selectionRect.Width = newWidth;
            selectionRect.Height = newHeight;
            UpdateHandleAndButton();
        }

        public void OnResizeHandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isResizing = false;
            resizeHandle.ReleaseMouseCapture();
        }

        // 拖动相关事件
        public void OnSelectionRectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isResizing || isSelecting) return;
            var pos = e.GetPosition(selectionRect);
            double margin = 5;
            if (pos.X > margin && pos.X < selectionRect.Width - margin &&
                pos.Y > margin && pos.Y < selectionRect.Height - margin)
            {
                isDraggingSelection = true;
                dragSelectionStartPoint = e.GetPosition(canvas);
                dragSelectionOrigLeft = Canvas.GetLeft(selectionRect);
                dragSelectionOrigTop = Canvas.GetTop(selectionRect);
                selectionRect.CaptureMouse();
                e.Handled = true;
            }
        }

        public void OnSelectionRectMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingSelection) return;
            var pos = e.GetPosition(canvas);
            double dx = pos.X - dragSelectionStartPoint.X;
            double dy = pos.Y - dragSelectionStartPoint.Y;

            double newLeft = dragSelectionOrigLeft + dx;
            double newTop = dragSelectionOrigTop + dy;

            newLeft = Math.Max(0, Math.Min(canvas.ActualWidth - selectionRect.Width, newLeft));
            newTop = Math.Max(0, Math.Min(canvas.ActualHeight - selectionRect.Height, newTop));

            Canvas.SetLeft(selectionRect, newLeft);
            Canvas.SetTop(selectionRect, newTop);

            UpdateHandleAndButton();
        }

        public void OnSelectionRectMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDraggingSelection)
            {
                isDraggingSelection = false;
                selectionRect.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// 更新缩放手柄和按钮面板的位置，使其始终跟随选区
        /// </summary>
        private void UpdateHandleAndButton()
        {
            double x = Canvas.GetLeft(selectionRect);
            double y = Canvas.GetTop(selectionRect);
            double w = selectionRect.Width;
            double h = selectionRect.Height;

            x = Math.Max(0, Math.Min(canvas.ActualWidth - w, x));
            y = Math.Max(0, Math.Min(canvas.ActualHeight - h, y));
            Canvas.SetLeft(selectionRect, x);
            Canvas.SetTop(selectionRect, y);

            Canvas.SetLeft(resizeHandle, x + w - resizeHandle.Width);
            Canvas.SetTop(resizeHandle, y + h - resizeHandle.Height);

            buttonPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double panelWidth = buttonPanel.DesiredSize.Width;
            double panelHeight = buttonPanel.DesiredSize.Height;

            double buttonX = x + (w - panelWidth) / 2;
            buttonX = Math.Max(0, Math.Min(canvas.ActualWidth - panelWidth, buttonX));
            double buttonY = y + h + 8;

            if (buttonY + panelHeight > canvas.ActualHeight)
            {
                buttonY = y - panelHeight - 8;
                if (buttonY < 0)
                    buttonY = 0;
            }

            Canvas.SetLeft(buttonPanel, buttonX);
            Canvas.SetTop(buttonPanel, buttonY);
        }

        /// <summary>
        /// 获取选区的屏幕物理像素坐标
        /// </summary>
        public Rect GetSelectedScreenRect()
        {
            double x = Canvas.GetLeft(selectionRect);
            double y = Canvas.GetTop(selectionRect);
            double w = selectionRect.Width;
            double h = selectionRect.Height;
            var topLeft = canvas.PointToScreen(new Point(x, y));
            var bottomRight = canvas.PointToScreen(new Point(x + w, y + h));
            return new Rect(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
    }
}