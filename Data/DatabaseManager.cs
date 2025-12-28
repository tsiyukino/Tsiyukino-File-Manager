using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TSFM.Models;

namespace TSFM.Data
{
    public enum StorageLocation
    {
        AppData,
        LocalFolder
    }

    public class DatabaseManager
    {
        private StorageLocation _storageLocation = StorageLocation.AppData;
        private List<Project> _games = new();
        private Database? _currentDatabase;
        private long _currentGameId = -1;

        public StorageLocation Location
        {
            get => _storageLocation;
            set
            {
                _storageLocation = value;
                EnsureStorageDirectoryExists();
            }
        }

        public DatabaseManager()
        {
            EnsureStorageDirectoryExists();
        }

        public string GetStoragePath()
        {
            if (_storageLocation == StorageLocation.AppData)
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appData, "TSFM");
            }
            else
            {
                return Directory.GetCurrentDirectory();
            }
        }

        private string GetGamesFilePath() => Path.Combine(GetStoragePath(), "games.json");

        private string GetGameDatabasePath(long gameId) => Path.Combine(GetGameFolder(gameId), "game.json");

        public string GetGameFolder(long gameId) => Path.Combine(GetStoragePath(), $"game-{gameId}");

        private void EnsureStorageDirectoryExists()
        {
            var path = GetStoragePath();
            Directory.CreateDirectory(path);
        }

        public bool LoadGames()
        {
            var filePath = GetGamesFilePath();

            if (!File.Exists(filePath))
            {
                _games.Clear();
                return true;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                _games = JsonConvert.DeserializeObject<List<Project>>(json) ?? new();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveGames()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_games, Formatting.Indented);
                File.WriteAllText(GetGamesFilePath(), json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Project> GetGames() => _games;

        public bool AddGame(Project game)
        {
            _games.Add(game);

            if (!SaveGames())
            {
                _games.RemoveAt(_games.Count - 1);
                return false;
            }

            var defaultDb = new Database
            {
                RootFolder = "",
                DisabledFolder = "_Disabled",
                ProjectTypeId = game.ProjectTypeId,
                Categories = new()
                {
                    new Category { Id = 1, Name = "Root", ParentId = null, Expanded = true }
                }
            };

            try
            {
                var gameFolder = GetGameFolder(game.Id);
                Directory.CreateDirectory(gameFolder);

                var json = JsonConvert.SerializeObject(defaultDb, Formatting.Indented);
                File.WriteAllText(GetGameDatabasePath(game.Id), json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateGame(Project game)
        {
            var index = _games.FindIndex(g => g.Id == game.Id);
            if (index < 0) return false;

            _games[index] = game;
            return SaveGames();
        }

        public bool DeleteGame(long gameId)
        {
            var removed = _games.RemoveAll(g => g.Id == gameId) > 0;
            if (!removed) return false;

            if (!SaveGames()) return false;

            try
            {
                var gameFolder = GetGameFolder(gameId);
                if (Directory.Exists(gameFolder))
                {
                    Directory.Delete(gameFolder, true);
                }
            }
            catch { }

            return true;
        }

        public bool LoadGameDatabase(long gameId)
        {
            var filePath = GetGameDatabasePath(gameId);
            if (!File.Exists(filePath)) return false;

            try
            {
                var json = File.ReadAllText(filePath);
                _currentDatabase = JsonConvert.DeserializeObject<Database>(json);
                _currentGameId = gameId;
                return _currentDatabase != null;
            }
            catch
            {
                return false;
            }
        }

        public Database? GetCurrentDatabase() => _currentDatabase;

        public long GetCurrentGameId() => _currentGameId;

        public bool SaveCurrentDatabase()
        {
            if (_currentDatabase == null || _currentGameId < 0) return false;

            try
            {
                var gameFolder = GetGameFolder(_currentGameId);
                Directory.CreateDirectory(gameFolder);

                var json = JsonConvert.SerializeObject(_currentDatabase, Formatting.Indented);
                File.WriteAllText(GetGameDatabasePath(_currentGameId), json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EnsureGameDatabaseExists(long gameId)
        {
            if (_currentGameId == gameId && _currentDatabase != null)
                return true;

            if (LoadGameDatabase(gameId))
                return true;

            _currentDatabase = new Database
            {
                Categories = new()
                {
                    new Category { Id = 1, Name = "Root", ParentId = null, Expanded = true }
                }
            };
            _currentGameId = gameId;

            return SaveCurrentDatabase();
        }

        public string? CopyGamePreviewImage(long gameId, string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath)) return null;

                var gameFolder = GetGameFolder(gameId);
                Directory.CreateDirectory(gameFolder);

                var extension = Path.GetExtension(sourcePath);
                if (string.IsNullOrEmpty(extension)) extension = ".png";

                var destFileName = $"preview{extension}";
                var destPath = Path.Combine(gameFolder, destFileName);

                if (File.Exists(destPath))
                    File.Delete(destPath);

                File.Copy(sourcePath, destPath);

                return $"game-{gameId}/{destFileName}";
            }
            catch
            {
                return null;
            }
        }
    }
}
