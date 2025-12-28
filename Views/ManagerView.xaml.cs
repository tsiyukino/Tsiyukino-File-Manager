using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TSFM.ViewModels;

namespace TSFM.Views
{
    public partial class ManagerView : UserControl
    {
        private readonly ProjectManager _projectManager;

        public ManagerView(ProjectManager projectManager)
        {
            InitializeComponent();
            _projectManager = projectManager;
            DataContext = _projectManager;

            if (_projectManager.CurrentProjectId > 0)
            {
                _projectManager.LoadCategories();
            }
        }

        // Category selection
        private void Category_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is long categoryId)
            {
                _projectManager.CurrentCategoryId = categoryId;
            }
        }

        // File selection
        private void File_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is long fileId)
            {
                _projectManager.SelectFile(fileId);
            }
        }

        // Category button handlers
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TextInputDialog("Add Category", "Category Name:");
            dialog.Owner = Window.GetWindow(this);
            
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResultText))
            {
                _projectManager.CreateCategory(dialog.ResultText, _projectManager.CurrentCategoryId);
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_projectManager.CurrentCategoryId == 1)
            {
                MessageBox.Show("Cannot delete root category.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this category and all its subcategories?\nFiles will be moved to the parent category.",
                "Delete Category",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _projectManager.DeleteCategory(_projectManager.CurrentCategoryId);
            }
        }

        // File button handlers
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TextInputDialog("Add File", "File Name:");
            dialog.Owner = Window.GetWindow(this);
            
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResultText))
            {
                _projectManager.CreateFile(dialog.ResultText, _projectManager.CurrentCategoryId);
            }
        }

        private void ToggleFile_Click(object sender, RoutedEventArgs e)
        {
            if (_projectManager.CurrentFile != null)
            {
                _projectManager.ToggleFile(_projectManager.CurrentFile.Id);
            }
        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (_projectManager.CurrentFile == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete \"{_projectManager.CurrentFile.Name}\"?",
                "Delete File",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _projectManager.DeleteFile(_projectManager.CurrentFile.Id);
            }
        }
    }
}
