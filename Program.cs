using System;
using System.IO;
using System.Text.RegularExpressions;

Console.WriteLine("Enter the path to the folder:");
string folderPath = Console.ReadLine();

// Getting the parent directory
DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
DirectoryInfo parentDirInfo = dirInfo.Parent;


if (parentDirInfo != null && Directory.Exists(parentDirInfo.FullName))
{
  string[] files = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);

  int counter = 1;
  foreach (string file in files)
  {
    try
    {
      // Get folder name and manipulate it
      string folderName = Path.GetFileName(Path.GetDirectoryName(file)) ?? throw new InvalidOperationException("Folder name cannot be null.");
      // Use Regular Expressions to remove the date part
      Regex regex = new Regex(@"-\d{6}");
      folderName = regex.Replace(folderName, "");

      //Generate new file file name
      string newFileName = $"{folderName}-{Path.GetFileNameWithoutExtension(file) ?? "Untitled"}-G{counter.ToString("D10")}.pdf";
      string newFilePath = Path.Combine(Path.GetDirectoryName(file) ?? throw new InvalidOperationException("File path cannot be null."), newFileName);

      // Log for troubleshooting
      Console.WriteLine($"Attempting to rename: {file} to {newFilePath}");

      File.Move(file, newFilePath);
      counter++;

      Console.WriteLine($"Successfully renamed to {newFilePath}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred while processing {file}: {ex.Message}");
    }
  }

  Console.WriteLine("Process completed!");
}
else
{
  Console.WriteLine("Folder path does not exist.");
}