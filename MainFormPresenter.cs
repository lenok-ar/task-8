using System;
using System.Threading;
using System.Windows.Forms;

namespace DirectorySync
{
  public class MainFormPresenter : IMainFormPresenter
  {
    private IMainFormView view;
    private readonly ISyncService syncService;
    private CancellationTokenSource _cancelTokenSourse;

    public MainFormPresenter(IMainFormView view, ISyncService syncService)
    {
      this.syncService = syncService;
      this.view = view;
    }

    public void SetView(IMainFormView view)
    {
      this.view = view;
      this.view.BrowseSourceClicked += View_BrowseSourceClicked;
      this.view.BrowseDestinationClicked += View_BrowseDestinationClicked;
      this.view.SyncButtonClicked += View_SyncButtonClicked;
    }

    private void View_BrowseSourceClicked(object sender, EventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
          view.SourceDirectory = folderBrowserDialog.SelectedPath;
        }
      }
    }

    private void View_BrowseDestinationClicked(object sender, EventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
          view.DestinationDirectory = folderBrowserDialog.SelectedPath;
        }
      }
    }

    private async void View_SyncButtonClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(view.SourceDirectory) || string.IsNullOrEmpty(view.DestinationDirectory))
      {
        view.ShowMessage("Пожалуйста, выберите как исходный, так и конечный каталоги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (view.IsSyncing)
      {
        _cancelTokenSourse.Cancel();
        return;
      }

      view.IsSyncing = true;
      view.SyncButtonText = "Отмена";
      _cancelTokenSourse = new CancellationTokenSource();

      try
      {
        view.ClearLog();
        var progress = new Progress<string>(message => view.AppendLog(message));

        await syncService.SynchronizeDirectories(view.SourceDirectory, view.DestinationDirectory, _cancelTokenSourse.Token, progress);
        await syncService.SynchronizeDirectories(view.DestinationDirectory, view.SourceDirectory, _cancelTokenSourse.Token, progress);

        view.ShowMessage("Синхронизация завершена.", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      catch (OperationCanceledException)
      {
        view.ShowMessage("Синхронизация отменена.", "Отменена.", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      catch (Exception ex)
      {
        view.ShowMessage($"Сбой синхронизации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      finally
      {
        view.IsSyncing = false;
        view.SyncButtonText = "Синхронизировать";
      }
    }

    public void OnBrowseSourceClicked()
    {
      throw new NotImplementedException();
    }

    public void OnBrowseDestinationClicked()
    {
      throw new NotImplementedException();
    }

    public void OnSyncButtonClicked()
    {
      throw new NotImplementedException();
    }
  }
}
