using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DirectorySync
{
  public class SyncService : ISyncService
  {
    public async Task SynchronizeDirectories(string source, string destination, CancellationToken cancellationToken, IProgress<string> progress)
    {
      await Task.Run(() =>
      {
        try
        {
          SyncDirectory(source, destination, cancellationToken, progress);
        }

        catch (OperationCanceledException)
        {
          throw;
        }

        catch (Exception ex)
        {
          progress.Report($"ОШИБКА: Ошибка синхронизации {source} в {destination}: {ex.Message}");
          throw;
        }
      }, cancellationToken);
    }

    private void SyncDirectory(string source, string destination, CancellationToken cancellationToken, IProgress<string> progress)
    {
      if (!Directory.Exists(destination))
      {
        Directory.CreateDirectory(destination);
        progress.Report($"Созданный каталог: {destination}");
      }

      string[] sourceFiles = Directory.GetFiles(source);
      string[] destinationFiles = Directory.GetFiles(destination);

      foreach (string sourceFile in sourceFiles)
      {
        cancellationToken.ThrowIfCancellationRequested();
        string fileName = Path.GetFileName(sourceFile);
        string destinationFile = Path.Combine(destination, fileName);

        if (!File.Exists(destinationFile))
        {
          progress.Report($"•   Файл  \"{fileName}\"  создан");
          File.Copy(sourceFile, destinationFile);
          progress.Report($"Скопируемый файл: {fileName} из {source} в {destination}");
        }

        else
        {
          FileInfo sourceInfo = new FileInfo(sourceFile);
          FileInfo destinationInfo = new FileInfo(destinationFile);

          if (sourceInfo.LastWriteTime > destinationInfo.LastWriteTime || sourceInfo.Length != destinationInfo.Length)
          {
            progress.Report($"•   Файл  \"{fileName}\"  изменен");
            File.Copy(sourceFile, destinationFile, true);
            progress.Report($"Обновленный файл: {fileName} из {source} в {destination}");
          }
        }
      }

      foreach (string destinationFile in destinationFiles)
      {
        cancellationToken.ThrowIfCancellationRequested();
        string fileName = Path.GetFileName(destinationFile);
        string sourceFile = Path.Combine(source, fileName);

        if (!File.Exists(sourceFile))
        {
          progress.Report($"•   Файл  \"{fileName}\"  удален");
          File.Delete(destinationFile);
          progress.Report($"Удаленный файл: {fileName} из {destination} потому что этого не существует в {source}");
        }
      }

      string[] sourceDirectories = Directory.GetDirectories(source);

      foreach (string sourceDir in sourceDirectories)
      {
        cancellationToken.ThrowIfCancellationRequested();

        string directoryName = Path.GetFileName(sourceDir);
        string destinationDirectoryPath = Path.Combine(destination, directoryName);

        if (!Directory.Exists(destinationDirectoryPath))
        {
          Directory.CreateDirectory(destinationDirectoryPath);
          progress.Report($"Созданный каталог: {destinationDirectoryPath}");
        }


        SyncDirectory(sourceDir, destinationDirectoryPath, cancellationToken, progress);
      }

      string[] destinationDirectories = Directory.GetDirectories(destination);

      foreach (string destinationDir in destinationDirectories)
      {
        cancellationToken.ThrowIfCancellationRequested();

        string dirName = Path.GetFileName(destinationDir);
        string sourceDir = Path.Combine(source, dirName);

        if (!Directory.Exists(sourceDir))
        {
          try
          {
            Directory.Delete(destinationDir, true);
            progress.Report($"•   Каталог  \"{dirName}\" удален");
            progress.Report($"Удаленный каталог: {dirName} из {destination} потому что этого не существует в {source}");
          }

          catch (Exception ex)
          {
            progress.Report($"ОШИБКА: Не удалось удалить каталог {dirName} из {destination}: {ex.Message}");
          }
        }
      }
    }
  }
}