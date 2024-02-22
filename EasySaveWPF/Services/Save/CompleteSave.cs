using System.IO; // Required for File operations
using System.Windows;
using EasySaveWPF.Models;

namespace EasySaveWPF.Services.Save;

/// <summary>
/// Implements a complete save operation by copying all files from source to target directory.
/// </summary>
public class CompleteSave : CommonSaveCommand
{
    /// <summary>
    /// Initializes a new complete save operation based on the provided save configuration.
    /// </summary>
    /// <param name="save">The save configuration.</param>
    public CompleteSave(BackupJobModel save)
    {
        Init(save);
    }

    /// <summary>
    /// Executes the complete save operation, copying all files and updating real-time statistics.
    /// </summary>
    /// <param name="save">The save configuration.</param>
    public void Execute(BackupJobModel save, string process)
    {
        // Prepares the target directory tree to mirror the source structure.
        SetTree(save.SourcePath, save.TargetPath);

        // Copies each file from the source to the target, updating stats for each file.
        int counter = 0;
        foreach (string element in SourcePathAllFiles)
        {
            if (process != null)
            {
                CheckProcess(process);
            }
            // Simulate stats update delay (replace with async/await in the future).
            //Thread.Sleep(10);
            SetInfosInStatsRTModel(save, element.Replace(save.SourcePath, ""));

            string fileExtension = Path.GetExtension(element);
            string[] allowedExtensions = save.Extensions.Split(';');

            if (allowedExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                string target = element.Replace(save.SourcePath, save.TargetPath);
                string filename = Path.GetFileName(target);
                string encryptedFilename = $".encrypted.{filename}";
                string targetDirectory = target.Substring(0, target.Length - filename.Length);
                target = Path.Combine(targetDirectory, encryptedFilename);
                CipherOrDecipher(element, target);
            }
            else
            {
                File.Copy(element, element.Replace(save.SourcePath, save.TargetPath), true);
            }

            //Thread.Sleep(10);
            UpdateFinishedFileSave();

            counter++;

            if (SourcePathAllFiles.Count == counter)
            {
                MessageBox.Show($"La sauvegarde {save.Name} est finie", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }
}
