using System.Linq;
using System.Windows;
using Microsoft.Win32;
using TSFM.ViewModels;

namespace TSFM.Views
{
    public partial class GameDialog : Window
    {
        private readonly ProjectManager _projectManager;
        private readonly bool _isEditMode;
        private readonly long _gameId;

        public GameDialog(ProjectManager projectManager, bool isEditMode, long gameId = -1)
        {
            InitializeComponent();
            _projectManager = projectManager;
            _isEditMode = isEditMode;
            _gameId = gameId;

            Title = isEditMode ? "Edit Game" : "Add New Game";

            if (isEditMode)
            {
                var game = _projectManager.Projects.FirstOrDefault(p => p.Id == gameId);
                if (game != null)
                {
                    NameTextBox.Text = game.Name;
                    DescriptionTextBox.Text = game.Description;
                    ImagePathTextBox.Text = game.Preview ?? "";
                }
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Select Cover Image"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a game name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {
                _projectManager.UpdateProject(
                    _gameId,
                    NameTextBox.Text.Trim(),
                    DescriptionTextBox.Text.Trim(),
                    ImagePathTextBox.Text.Trim()
                );
            }
            else
            {
                _projectManager.CreateProject(
                    NameTextBox.Text.Trim(),
                    DescriptionTextBox.Text.Trim(),
                    ImagePathTextBox.Text.Trim()
                );
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
