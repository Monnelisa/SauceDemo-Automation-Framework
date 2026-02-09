using System;
using System.IO;

namespace TakealotAutomation.Utilities
{
    /// <summary>
    /// Utility class for file operations
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Creates a directory if it doesn't exist
        /// </summary>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Deletes a file if it exists
        /// </summary>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Deletes a directory and all its contents
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Gets all files in a directory with specific extension
        /// </summary>
        public static string[] GetFiles(string path, string extension)
        {
            if (!Directory.Exists(path))
                return Array.Empty<string>();

            return Directory.GetFiles(path, $"*{extension}");
        }

        /// <summary>
        /// Reads all text from a file
        /// </summary>
        public static string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Writes text to a file
        /// </summary>
        public static void WriteAllText(string path, string content)
        {
            CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }
    }
}
