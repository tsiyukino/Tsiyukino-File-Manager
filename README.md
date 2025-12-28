# TSFM - File Manager (WPF .NET 8)

## Migration from Qt/QML to WPF - COMPLETE

Your project has been successfully migrated from Qt/QML to WPF (.NET 8).

## Prerequisites

1. **.NET 8 SDK** (Required)
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version` (should show 8.x.x)

## Quick Start

1. **Build the project:**
   ```
   build.bat
   ```

2. **Run the application:**
   ```
   run.bat
   ```

3. **Clean build artifacts:**
   ```
   clean.bat
   ```

## Project Structure

```
TSFM/
├── TSFM.csproj                          # Project file
├── App.xaml / App.xaml.cs               # Application entry point
├── MainWindow.xaml / MainWindow.xaml.cs # Main window
├── Models/
│   └── DataStructures.cs                # Data models (Category, Mod, Project, Database)
├── ViewModels/
│   └── ProjectManager.cs                # Business logic & state management
├── Data/
│   └── DatabaseManager.cs               # JSON database operations
├── Views/
│   ├── SidebarControl.xaml/.cs          # Navigation sidebar
│   ├── HomeView.xaml/.cs                # Home screen
│   ├── ProjectsView.xaml/.cs            # Games list
│   ├── ManagerView.xaml/.cs             # File manager (COMPLETE)
│   ├── GameCard.xaml/.cs                # Game card component
│   ├── GameDialog.xaml/.cs              # Add/Edit game dialog
│   ├── TextInputDialog.xaml/.cs         # Simple text input dialog (NEW)
│   ├── TagsView.xaml                    # Tags view (placeholder)
│   └── SettingsView.xaml                # Settings view (placeholder)
└── Converters/
    └── DepthToMarginConverter.cs        # Category tree indentation
```

## What's COMPLETE and Working

### ✅ Full Features Implemented:
1. **Project/Game Management**
   - Create, edit, delete games
   - Game preview images (auto-copied to AppData)
   - Game cards with hover effects
   
2. **Category Tree**
   - Hierarchical categories
   - Click to select category
   - Add/Delete categories
   - Files auto-move to parent when category deleted

3. **File Management**
   - Add files to categories
   - Enable/Disable toggle
   - Delete files
   - Click to select file

4. **Database Persistence**
   - JSON storage in `%AppData%\TSFM\`
   - Separate database per game
   - Auto-save on all changes

5. **UI/UX**
   - Google Material Design inspired
   - Smooth animations
   - Hover effects
   - Professional styling

## Key Technical Differences from Qt/QML

| Aspect | Qt/QML | WPF |
|--------|--------|-----|
| Language | C++ | C# |
| UI Markup | QML | XAML |
| Data Binding | Property bindings | {Binding Path} |
| Lists | QVariantList | ObservableCollection<T> |
| Notifications | Q_PROPERTY + emit | INotifyPropertyChanged |
| Navigation | StackLayout | ContentControl switching |

## Build Commands

### Development Build
```bash
dotnet build
```

### Release Build
```bash
dotnet build --configuration Release
```

### Run Without Building
```bash
dotnet run
```

### Publish (Standalone EXE)
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Data Storage Location

Data is stored in:
```
%AppData%\TSFM\
├── games.json              # List of all games
├── game-{id}\
│   ├── game.json          # Game database
│   └── preview.{ext}      # Game preview image
```

## What's NOT Implemented (Optional Features)

These are placeholder views you can complete later:
1. **File Details Panel** - Edit file tags, notes, preview images
2. **Tags View** - Manage global tags and metadata
3. **Settings View** - App configuration
4. **Bulk Operations** - Multi-select files, bulk enable/disable
5. **Search** - Search files by name/tags
6. **Move Files** - Drag & drop or move files between categories

## Extending the Project

### To Add a New View:
1. Create `MyView.xaml` and `MyView.xaml.cs` in `/Views`
2. Add navigation button in `SidebarControl.xaml`
3. Wire up in `MainWindow.xaml.cs`

### To Add New Data Fields:
1. Update model in `Models/DataStructures.cs`
2. Add `[JsonProperty("field_name")]` attribute
3. Field auto-saves with existing save logic

## Common Tasks

### Add New Game:
1. Click "Games" in sidebar
2. Click "Add Game" button
3. Fill in name, optional image/description
4. Click "Save"

### Manage Files:
1. Select a game (clicks game card)
2. Automatically opens "File Manager"
3. Use "Add" to create categories/files
4. Click items to select
5. Use "Toggle" to enable/disable
6. Use "Delete" to remove

## Troubleshooting

### Build Errors:
- **Missing .NET 8**: Install from link above
- **NuGet restore failed**: Run `dotnet restore`
- **XAML errors**: Check for typos in binding paths

### Runtime Errors:
- **No games showing**: Check `%AppData%\TSFM\games.json`
- **Can't add files**: Make sure you've selected a game first
- **Images not showing**: Images copied to AppData, check file permissions

## Migration Notes

### Deleted Qt Files:
- All `.qml` files
- All `.cpp` and `.h` files
- `CMakeLists.txt`
- Qt-specific build scripts

### Kept Files:
- `resources/icons/` (SVG icons work in WPF)
- Existing JSON data files (compatible format)

## Next Steps

1. **Test the application** - Create a game, add categories, add files
2. **Optional: Complete placeholder views** (Tags, Settings, File Details)
3. **Optional: Add icons** - Replace emoji with SVG icons from resources folder
4. **Optional: Add bulk operations** - Multi-select checkboxes
5. **Optional: Deploy** - Use `dotnet publish` for distribution

## Support

For .NET/WPF questions:
- Microsoft Docs: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
- WPF Tutorial: https://wpf-tutorial.com/

---

**Migration Complete!** 🎉
Your Qt/QML project is now a modern WPF application.
