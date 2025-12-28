using System.Windows;

namespace TSFM.Views
{
    public partial class TextInputDialog : Window
    {
        public string ResultText { get; private set; } = string.Empty;

        public TextInputDialog(string title, string label)
        {
            InitializeComponent();
            Title = title;
            LabelTextBlock.Text = label;
            InputTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResultText = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && !string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                OkButton_Click(sender, e);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }
    }
}
