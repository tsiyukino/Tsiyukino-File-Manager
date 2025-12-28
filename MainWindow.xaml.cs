using System.Windows;
using TSFM.ViewModels;
using TSFM.Views;

namespace TSFM
{
    public partial class MainWindow : Window
    {
        private readonly ProjectManager _projectManager;
        private readonly SidebarControl _sidebar;
        private HomeView? _homeView;
        private ProjectsView? _projectsView;
        private ManagerView? _managerView;
        private TagsView? _tagsView;
        private SettingsView? _settingsView;

        public MainWindow()
        {
            InitializeComponent();

            _projectManager = new ProjectManager();
            DataContext = _projectManager;

            _sidebar = new SidebarControl(_projectManager);
            _sidebar.NavigationChanged += Sidebar_NavigationChanged;
            SidebarContent.Content = _sidebar;

            // Show home view by default
            ShowHomeView();

            // Load projects
            _projectManager.LoadProjects();
        }

        private void Sidebar_NavigationChanged(int index)
        {
            switch (index)
            {
                case 0:
                    ShowHomeView();
                    break;
                case 1:
                    ShowProjectsView();
                    break;
                case 2:
                    ShowManagerView();
                    break;
                case 3:
                    ShowTagsView();
                    break;
                case 4:
                    ShowSettingsView();
                    break;
            }
        }

        private void ShowHomeView()
        {
            _homeView ??= new HomeView();
            MainContent.Content = _homeView;
        }

        private void ShowProjectsView()
        {
            _projectsView ??= new ProjectsView(_projectManager, _sidebar);
            MainContent.Content = _projectsView;
        }

        private void ShowManagerView()
        {
            _managerView ??= new ManagerView(_projectManager);
            MainContent.Content = _managerView;
        }

        private void ShowTagsView()
        {
            _tagsView ??= new TagsView();
            MainContent.Content = _tagsView;
        }

        private void ShowSettingsView()
        {
            _settingsView ??= new SettingsView();
            MainContent.Content = _settingsView;
        }
    }
}
