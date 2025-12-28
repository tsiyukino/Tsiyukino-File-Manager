using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TSFM.ViewModels;

namespace TSFM.Views
{
    public partial class SidebarControl : UserControl
    {
        private int _currentIndex = 0;
        private readonly ProjectManager _projectManager;
        private bool _isExpanded = true;
        private const double ExpandedWidth = 240;
        private const double CollapsedWidth = 64;

        public event Action<int>? NavigationChanged;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                UpdateButtonStyles();
                NavigationChanged?.Invoke(value);
            }
        }

        public SidebarControl(ProjectManager projectManager)
        {
            InitializeComponent();
            _projectManager = projectManager;
            DataContext = _projectManager;
            Width = ExpandedWidth;
            UpdateButtonStyles();
            UpdateExpandedState();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;
            AnimateSidebar();
        }

        private void AnimateSidebar()
        {
            var targetWidth = _isExpanded ? ExpandedWidth : CollapsedWidth;
            
            var widthAnimation = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            widthAnimation.Completed += (s, e) => UpdateExpandedState();
            this.BeginAnimation(WidthProperty, widthAnimation);
        }

        private void UpdateExpandedState()
        {
            // Header elements
            ExpandButton.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ExpandedHeader.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Section labels
            MainLabel.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
            ProjectLabel.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Home button
            HomeIconCollapsed.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            HomeExpanded.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Games button
            GamesIconCollapsed.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            GamesExpanded.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Manager button
            ManagerIconCollapsed.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ManagerExpanded.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Tags button
            TagsIconCollapsed.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            TagsExpanded.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            // Settings button
            SettingsIconCollapsed.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            SettingsExpanded.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        }

        private void HomeButton_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentIndex = 0;
        }

        private void GamesButton_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentIndex = 1;
        }

        private void ManagerButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (_projectManager.CurrentProjectId >= 0)
                CurrentIndex = 2;
        }

        private void TagsButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (_projectManager.CurrentProjectId >= 0)
                CurrentIndex = 3;
        }

        private void SettingsButton_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentIndex = 4;
        }

        private void NavButton_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hover effect now handled by XAML triggers
        }

        private void NavButton_MouseLeave(object sender, MouseEventArgs e)
        {
            // Hover effect now handled by XAML triggers
        }

        private void UpdateButtonStyles()
        {
            var buttons = new[] { HomeButton, GamesButton, ManagerButton, TagsButton, SettingsButton };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == _currentIndex)
                {
                    buttons[i].Background = new SolidColorBrush(Color.FromArgb(38, 66, 133, 244));
                }
                else
                {
                    buttons[i].Background = Brushes.Transparent;
                }
            }
        }
    }
}
