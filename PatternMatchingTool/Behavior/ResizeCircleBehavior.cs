using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using PatternMatchingTool.ViewModel;

namespace PatternMatchingTool.Behavior
{
    public class ResizeCircleBehavior : Behavior<Ellipse>
    {
        private bool _isResizing = false;
        private System.Windows.Point _clickPosition;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isResizing = true;
            _clickPosition = e.GetPosition(null);
            AssociatedObject.CaptureMouse();

            e.Handled = true;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isResizing) return;

            var ellipse = sender as Ellipse;
            var vm = ellipse.DataContext as SettingPatternPageVM;


            // 현재 마우스 위치
            System.Windows.Point currentPosition = e.GetPosition(null);
            double dx = currentPosition.X - _clickPosition.X;
            double dy = currentPosition.Y - _clickPosition.Y;

            _clickPosition = currentPosition;

            string corner = ellipse.Name;
            Console.WriteLine($"{corner}");

            switch (corner)
            {
                case "TopLeft":
                    vm.PatternRect = new PatternMatchingTool.Model.Rect(
                        (vm.PatternRect.X + dx),
                        (vm.PatternRect.Y + dy),
                        (vm.PatternRect.Width - dx),
                        (vm.PatternRect.Height - dy)
                    );
                    break;
                case "TopRight":
                    vm.PatternRect = new PatternMatchingTool.Model.Rect(
                        vm.PatternRect.X,
                        (vm.PatternRect.Y + dy),
                        (vm.PatternRect.Width + dx),
                        (vm.PatternRect.Height - dy)
                    );
                    break;

                case "BottomLeft":
                    vm.PatternRect = new PatternMatchingTool.Model.Rect(
                        (vm.PatternRect.X + dx),
                        vm.PatternRect.Y,
                        (vm.PatternRect.Width - dx),
                        (vm.PatternRect.Height + dy)
                    );
                    break;

                case "BottomRight":
                    vm.PatternRect = new PatternMatchingTool.Model.Rect(
                        vm.PatternRect.X,
                        vm.PatternRect.Y,
                        (vm.PatternRect.Width + dx),
                        (vm.PatternRect.Height + dy)
                    );
                    break;
            }

            //== Rect 모서리 크기 조절 Ellipse ========================================//
            if (vm != null)
            {
                // 원 위치 임의로 맞춰둠
                vm.PatternRectLeftTopX = vm.PatternRect.X - 2;
                vm.PatternRectLeftTopY = vm.PatternRect.Y - 2;

                vm.PatternRectRightTopX = vm.PatternRect.X + vm.PatternRect.Width - 5;
                vm.PatternRectRightTopY = vm.PatternRect.Y - 2;

                vm.PatternRectLeftBottomX = vm.PatternRect.X - 2;
                vm.PatternRectLeftBottomY = vm.PatternRect.Y + vm.PatternRect.Height - 5;

                vm.PatternRectRightBottomX = vm.PatternRect.X + vm.PatternRect.Width - 5;
                vm.PatternRectRightBottomY = vm.PatternRect.Y + vm.PatternRect.Height - 5;
            }
            //========================================================================//
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isResizing = false;
            AssociatedObject.ReleaseMouseCapture();
        }
    }
}
