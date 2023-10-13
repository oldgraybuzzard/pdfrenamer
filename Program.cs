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

  //Read the last count from the text file
  string counterFilePath = Path.Combine(folderPath, "counter.txt");
  if (File.Exists(counterFilePath))
  {
    string counterText = File.ReadAllText(counterFilePath);
    if (!int.TryParse(counterText, out counter))
    {
      Console.WriteLine("Invalid counter value in file. Starting from 1.");
      counter = 1;
    }
  }
  else
  {
    Console.WriteLine("Counter file does not exist. Starting from 1.");
  }

  // Code to process the pdf's
  Regex alreadyProcessed = new Regex(@"-G\d{10}\.pdf$");
  string logFilePath = Path.Combine(folderPath, "processed_files_log.csv");

  foreach (string file in files)
  {
    try
    {
      // If file has already been processed, skip it
      if (alreadyProcessed.IsMatch(Path.GetFileName(file)))
      {
        Console.WriteLine($"Skipping already processed file: {file}");
        continue;  // Skip the rest of the loop and move to the next file
      }

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
      // Log to CSV
      string logEntry = $"\"{file}\",\"{newFilePath}\"\n";
      File.AppendAllText(logFilePath, logEntry);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred while processing {file}: {ex.Message}");
    }
  }

  try
  {
    File.WriteAllText(counterFilePath, counter.ToString());
    Console.WriteLine($"Counter saved to {counterFilePath}");
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Failed to save counter: {ex.Message}");
  }

  Console.WriteLine("Process completed!");
}
else
{
  Console.WriteLine("Folder path does not exist.");
}
