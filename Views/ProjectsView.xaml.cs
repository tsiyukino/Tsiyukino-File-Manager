using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TSFM.ViewModels;
using Microsoft.Win32;

namespace TSFM.Views
{
    public partial class ProjectsView : UserControl
    {
        private readonly ProjectManager _projectManager;
        private readonly SidebarControl _sidebar;

        public ProjectsView(ProjectManager projectManager, SidebarControl sidebar)
        {
            InitializeComponent();
            _projectManager = projectManager;
            _sidebar = sidebar;
            DataContext = _projectManager;
        }

        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GameDialog(_projectManager, false);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void GameCard_Click(object sender, long gameId)
        {
            _projectManager.CurrentProjectId = gameId;
            _sidebar.CurrentIndex = 2;
        }

        private void EditGame_Click(object sender, long gameId)
        {
            var dialog = new GameDialog(_projectManager, true, gameId);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void DeleteGame_Click(object sender, long gameId)
        {
            var project = _projectManager.Projects.FirstOrDefault(p => p.Id == gameId);
            if (project == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete \"{project.Name}\"?\nThis action cannot be undone.",
                "Delete Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _projectManager.DeleteProject(gameId);
            }
        }
    }
}
