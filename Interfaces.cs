using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectorySync
{
  public interface IMainFormView
  {
    string SourceDirectory { get; set; }
    string DestinationDirectory { get; set; }
    bool IsSyncing { get; set; }
    string SyncButtonText { get; set; }
    string StatusLabelText { get; set; }

    event EventHandler BrowseSourceClicked;
    event EventHandler BrowseDestinationClicked;
    event EventHandler SyncButtonClicked;

    void ShowMessage(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
    void AppendLog(string message);
    void ClearLog();
    void UpdateProgress(int value);
  }

  public interface IMainFormPresenter
  {
    void SetView(IMainFormView view);
    void OnBrowseSourceClicked();
    void OnBrowseDestinationClicked();
    void OnSyncButtonClicked();
  }

  public interface ISyncService
  {
    Task SynchronizeDirectories(string source, string destination, CancellationToken cancellationToken, IProgress<string> progress);
  }
}