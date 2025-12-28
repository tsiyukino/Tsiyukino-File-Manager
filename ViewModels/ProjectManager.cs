using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using TSFM.Data;
using TSFM.Models;

namespace TSFM.ViewModels
{
    public class ProjectManager : INotifyPropertyChanged
    {
        private readonly DatabaseManager _dbManager;
        private long _currentProjectId = -1;
        private long _currentCategoryId = 1;
        private long _currentFileId = -1;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? ErrorOccurred;

        public ObservableCollection<Project> Projects { get; } = new();
        public ObservableCollection<CategoryViewModel> Categories { get; } = new();
        public ObservableCollection<Mod> Files { get; } = new();

        private Mod? _currentFile;
        public Mod? CurrentFile
        {
            get => _currentFile;
            set
            {
                _currentFile = value;
                OnPropertyChanged();
            }
        }

        public long CurrentProjectId
        {
            get => _currentProjectId;
            set
            {
                if (_currentProjectId != value)
                {
                    _currentProjectId = value;
                    OnPropertyChanged();

                    if (value > 0)
                    {
                        if (_dbManager.EnsureGameDatabaseExists(value))
                        {
                            _currentCategoryId = 1;
                            LoadCategories();
                            LoadFiles(1);
                            OnPropertyChanged(nameof(CurrentCategoryId));
                        }
                    }
                }
            }
        }

        public long CurrentCategoryId
        {
            get => _currentCategoryId;
            set
            {
                if (_currentCategoryId != value)
                {
                    _currentCategoryId = value;
                    OnPropertyChanged();
                    LoadFiles(value);
                }
            }
        }

        public ProjectManager()
        {
            _dbManager = new DatabaseManager();
        }

        public void LoadProjects()
        {
            _dbManager.LoadGames();
            Projects.Clear();
            foreach (var game in _dbManager.GetGames())
            {
                Projects.Add(game);
            }
        }

        public void CreateProject(string name, string description, string imagePath, string rootFolder = "", string projectTypeId = "file")
        {
            var project = new Project
            {
                Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Name = name,
                Description = description,
                RootFolder = rootFolder,
                ProjectTypeId = projectTypeId
            };

            if (!string.IsNullOrEmpty(imagePath))
            {
                var relativePath = _dbManager.CopyGamePreviewImage(project.Id, imagePath);
                if (!string.IsNullOrEmpty(relativePath))
                {
                    project.Preview = relativePath;
                }
            }

            if (_dbManager.AddGame(project))
            {
                LoadProjects();
            }
            else
            {
                ErrorOccurred?.Invoke("Failed to create project");
            }
        }

        public void UpdateProject(long projectId, string name, string description, string imagePath, string rootFolder = "", string projectTypeId = "file")
        {
            var project = _dbManager.GetGames().FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                ErrorOccurred?.Invoke("Project not found");
                return;
            }

            project.Name = name;
            project.Description = description;
            project.RootFolder = rootFolder;
            project.ProjectTypeId = projectTypeId;

            if (!string.IsNullOrEmpty(imagePath) && (imagePath.StartsWith("file:///") || Path.IsPathRooted(imagePath)))
            {
                var relativePath = _dbManager.CopyGamePreviewImage(projectId, imagePath);
                if (!string.IsNullOrEmpty(relativePath))
                {
                    project.Preview = relativePath;
                }
            }

            if (_dbManager.UpdateGame(project))
            {
                LoadProjects();
            }
            else
            {
                ErrorOccurred?.Invoke("Failed to update project");
            }
        }

        public void DeleteProject(long projectId)
        {
            if (_dbManager.DeleteGame(projectId))
            {
                LoadProjects();
            }
            else
            {
                ErrorOccurred?.Invoke("Failed to delete project");
            }
        }

        public void LoadCategories()
        {
            Categories.Clear();
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var visibleIds = new HashSet<long>();

            // Add root categories
            foreach (var c in db.Categories.Where(c => !c.ParentId.HasValue || c.ParentId.Value == 0))
            {
                visibleIds.Add(c.Id);
            }

            // Add children of expanded categories
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var c in db.Categories)
                {
                    if (c.ParentId.HasValue && !visibleIds.Contains(c.Id))
                    {
                        var parentId = c.ParentId.Value;
                        if (visibleIds.Contains(parentId))
                        {
                            var parent = db.Categories.FirstOrDefault(p => p.Id == parentId);
                            if (parent != null && parent.Expanded)
                            {
                                visibleIds.Add(c.Id);
                                changed = true;
                            }
                        }
                    }
                }
            }

            // Build hierarchy
            var rootCategories = db.Categories.Where(c => visibleIds.Contains(c.Id) && (!c.ParentId.HasValue || c.ParentId.Value == 0)).ToList();
            foreach (var root in rootCategories)
            {
                AddCategoryWithChildren(db.Categories, visibleIds, root, 0);
            }
        }

        private void AddCategoryWithChildren(List<Category> allCategories, HashSet<long> visibleIds, Category parent, int depth)
        {
            var hasChildren = allCategories.Any(c => c.ParentId.HasValue && c.ParentId.Value == parent.Id);

            Categories.Add(new CategoryViewModel
            {
                Id = parent.Id,
                Name = parent.Name,
                ParentId = parent.ParentId,
                Expanded = parent.Expanded,
                Depth = depth,
                HasChildren = hasChildren
            });

            if (parent.Expanded)
            {
                foreach (var child in allCategories.Where(c => visibleIds.Contains(c.Id) && c.ParentId.HasValue && c.ParentId.Value == parent.Id))
                {
                    AddCategoryWithChildren(allCategories, visibleIds, child, depth + 1);
                }
            }
        }

        public void CreateCategory(string name, long parentId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var usedIds = new HashSet<long>(db.Categories.Select(c => c.Id));
            long newId = 1;
            while (usedIds.Contains(newId)) newId++;

            var category = new Category
            {
                Id = newId,
                Name = name,
                ParentId = parentId,
                Expanded = true
            };

            db.Categories.Add(category);
            _dbManager.SaveCurrentDatabase();
            LoadCategories();
        }

        public void DeleteCategory(long categoryId)
        {
            if (categoryId == 1)
            {
                ErrorOccurred?.Invoke("Cannot delete root category");
                return;
            }

            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) return;

            var parentId = category.ParentId ?? 1;
            var toDelete = new HashSet<long> { categoryId };

            // Find all descendants
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var c in db.Categories)
                {
                    if (!toDelete.Contains(c.Id) && c.ParentId.HasValue && toDelete.Contains(c.ParentId.Value))
                    {
                        toDelete.Add(c.Id);
                        changed = true;
                    }
                }
            }

            // Move files to parent
            foreach (var mod in db.Mods.Where(m => toDelete.Contains(m.CategoryId)))
            {
                mod.CategoryId = parentId;
            }

            db.Categories.RemoveAll(c => toDelete.Contains(c.Id));
            _dbManager.SaveCurrentDatabase();

            CurrentCategoryId = parentId;
            LoadCategories();
        }

        public void ToggleCategory(long categoryId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                category.Expanded = !category.Expanded;
                _dbManager.SaveCurrentDatabase();
                LoadCategories();
            }
        }

        public void LoadFiles(long categoryId)
        {
            CurrentCategoryId = categoryId;

            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var categoryIds = new HashSet<long> { categoryId };

            // Find all descendants
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var c in db.Categories)
                {
                    if (!categoryIds.Contains(c.Id) && c.ParentId.HasValue && categoryIds.Contains(c.ParentId.Value))
                    {
                        categoryIds.Add(c.Id);
                        changed = true;
                    }
                }
            }

            Files.Clear();
            foreach (var mod in db.Mods.Where(m => categoryIds.Contains(m.CategoryId)))
            {
                Files.Add(mod);
            }
        }

        public void CreateFile(string name, long categoryId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var usedIds = new HashSet<long>(db.Mods.Select(m => m.Id));
            long newId = 1;
            while (usedIds.Contains(newId)) newId++;

            var mod = new Mod
            {
                Id = newId,
                Name = name,
                CategoryId = categoryId,
                Enabled = true
            };

            db.Mods.Add(mod);
            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public void UpdateFile(long fileId, string? name = null, string? notes = null, List<string>? tags = null, string? preview = null)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var mod = db.Mods.FirstOrDefault(m => m.Id == fileId);
            if (mod == null) return;

            if (name != null) mod.Name = name;
            if (notes != null) mod.Notes = notes;
            if (tags != null) mod.Tags = tags;
            if (preview != null) mod.Preview = preview;

            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);

            if (_currentFileId == fileId)
            {
                CurrentFile = mod;
            }
        }

        public void DeleteFile(long fileId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            db.Mods.RemoveAll(m => m.Id == fileId);
            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public void ToggleFile(long fileId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            var mod = db.Mods.FirstOrDefault(m => m.Id == fileId);
            if (mod != null)
            {
                mod.Enabled = !mod.Enabled;
                _dbManager.SaveCurrentDatabase();
                LoadFiles(_currentCategoryId);

                if (_currentFileId == fileId)
                {
                    CurrentFile = mod;
                }
            }
        }

        public void SelectFile(long fileId)
        {
            _currentFileId = fileId;
            var db = _dbManager.GetCurrentDatabase();
            CurrentFile = db?.Mods.FirstOrDefault(m => m.Id == fileId);
        }

        public void BulkEnable(List<long> fileIds)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            foreach (var mod in db.Mods.Where(m => fileIds.Contains(m.Id)))
            {
                mod.Enabled = true;
            }

            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public void BulkDisable(List<long> fileIds)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            foreach (var mod in db.Mods.Where(m => fileIds.Contains(m.Id)))
            {
                mod.Enabled = false;
            }

            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public void BulkDelete(List<long> fileIds)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            db.Mods.RemoveAll(m => fileIds.Contains(m.Id));
            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public void MoveFiles(List<long> fileIds, long targetCategoryId)
        {
            var db = _dbManager.GetCurrentDatabase();
            if (db == null) return;

            foreach (var mod in db.Mods.Where(m => fileIds.Contains(m.Id)))
            {
                mod.CategoryId = targetCategoryId;
            }

            _dbManager.SaveCurrentDatabase();
            LoadFiles(_currentCategoryId);
        }

        public string? GetPreviewImagePath(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;

            var fullPath = Path.Combine(_dbManager.GetStoragePath(), relativePath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CategoryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? ParentId { get; set; }
        public bool Expanded { get; set; }
        public int Depth { get; set; }
        public bool HasChildren { get; set; }
    }
}
