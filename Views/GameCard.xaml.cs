using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TSFM.Views
{
    public partial class GameCard : UserControl
    {
        public static readonly DependencyProperty GameNameProperty =
            DependencyProperty.Register(nameof(GameName), typeof(string), typeof(GameCard));

        public static readonly DependencyProperty GameIdProperty =
            DependencyProperty.Register(nameof(GameId), typeof(long), typeof(GameCard));

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(GameCard));

        public string GameName
        {
            get => (string)GetValue(GameNameProperty);
            set => SetValue(GameNameProperty, value);
        }

        public long GameId
        {
            get => (long)GetValue(GameIdProperty);
            set => SetValue(GameIdProperty, value);
        }

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public event Action<object, long>? GameCardClick;
        public event Action<object, long>? EditClick;
        public event Action<object, long>? DeleteClick;

        public GameCard()
        {
            InitializeComponent();
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GameCardClick?.Invoke(this, GameId);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            EditClick?.Invoke(this, GameId);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DeleteClick?.Invoke(this, GameId);
        }
    }
}
