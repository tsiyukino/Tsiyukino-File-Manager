using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Diagnostics;

namespace TSFM.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            
            Debug.WriteLine($"[ImagePathConverter] Converting path: '{path}'");
            
            if (string.IsNullOrEmpty(path))
            {
                Debug.WriteLine($"[ImagePathConverter] Path is null/empty - returning null");
                return null;
            }

            // Check if path is already absolute
            if (Path.IsPathRooted(path))
            {
                if (File.Exists(path))
                {
                    Debug.WriteLine($"[ImagePathConverter] Absolute path exists: '{path}'");
                    return path;
                }
                else
                {
                    Debug.WriteLine($"[ImagePathConverter] Absolute path NOT FOUND: '{path}'");
                    return null;
                }
            }

            // Convert relative path to absolute
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullPath = Path.Combine(appDataPath, "TSFM", path);
            
            Debug.WriteLine($"[ImagePathConverter] Converted relative to: '{fullPath}'");
            
            if (File.Exists(fullPath))
            {
                Debug.WriteLine($"[ImagePathConverter] File EXISTS - returning path");
                return fullPath;
            }
            else
            {
                Debug.WriteLine($"[ImagePathConverter] File NOT FOUND - returning null");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
