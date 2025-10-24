using Microsoft.Xaml.Behaviors;
using PatternMatchingTool.ViewModel;
using System.Runtime.Remoting.Channels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace PatternMatchingTool.Behavior
{
    public class RectBehavior : Behavior<System.Windows.Shapes.Rectangle>
    {
        private bool m_bIsDragging = false;
        private System.Windows.Point m_objClickPosition;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            var viewmodel = rect.DataContext as SettingPatternPageVM;

            if (viewmodel != null)
            {
                viewmodel.IsPatternRectClick = true;
            }
            m_bIsDragging = true;
            m_objClickPosition = e.GetPosition(rect); 
            rect.CaptureMouse();

            // Page에 이벤트 전달 안 되게끔
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null) return;

            m_bIsDragging = false;
            rect.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null || !m_bIsDragging) return;

            var viewmodel = rect.DataContext as SettingPatternPageVM;

            var canvas = VisualTreeHelper.GetParent(rect) as Canvas;
            if (canvas == null) return;

            // 마우스 위치 계산
            System.Windows.Point mousePos = e.GetPosition(canvas);
            double newX = mousePos.X - m_objClickPosition.X;
            double newY = mousePos.Y - m_objClickPosition.Y;

            viewmodel.PatternRect.X = newX;
            viewmodel.PatternRect.Y = newY;


            //== Rect 모서리 크기 조절 Ellipse ========================================//
            if (viewmodel != null)
            {
                // 원 위치 임의로 맞춰둠
                viewmodel.PatternRectLeftTopX = newX - 2;
                viewmodel.PatternRectLeftTopY = newY - 2;

                viewmodel.PatternRectRightTopX = newX + rect.Width - 5;
                viewmodel.PatternRectRightTopY = newY - 2;

                viewmodel.PatternRectLeftBottomX = newX - 2;
                viewmodel.PatternRectLeftBottomY = newY + rect.Height - 5;

                viewmodel.PatternRectRightBottomX = newX + rect.Width - 5;
                viewmodel.PatternRectRightBottomY = newY + rect.Height - 5;
            }
            //========================================================================//
        }

    }
}