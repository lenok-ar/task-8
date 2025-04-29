using System;
using System.Windows.Forms;

namespace DirectorySync
{
  public partial class Form1 : Form, IMainFormView
  {
    private MainFormPresenter presenter;

    public Form1()
    {
      InitializeComponent();
      presenter = new MainFormPresenter(this, new SyncService());
      presenter.SetView(this);
    }

    public string SourceDirectory
    {
      get => txtSource.Text;
      set => txtSource.Text = value;
    }

    public string DestinationDirectory
    {
      get => txtDestination.Text;
      set => txtDestination.Text = value;
    }

    public bool IsSyncing { get; set; }

    public string SyncButtonText
    {
      get => btnSync.Text;
      set => btnSync.Text = value;
    }

    public string StatusLabelText
    {
      get => lblStatus.Text;
      set => lblStatus.Text = value;
    }

    public event EventHandler BrowseSourceClicked;
    public event EventHandler BrowseDestinationClicked;
    public event EventHandler SyncButtonClicked;

    public void ShowMessage(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
      MessageBox.Show(message, title, buttons, icon);
    }

    public void AppendLog(string message)
    {
      if (logTextBox.InvokeRequired)
      {
        logTextBox.Invoke(new Action<string>(AppendLog), message);
      }

      else
      {
        logTextBox.AppendText($"{DateTime.Now}: {message}\r\n");
        logTextBox.SelectionStart = logTextBox.Text.Length;
        logTextBox.ScrollToCaret();
      }
    }

    public void ClearLog()
    {
      if (logTextBox.InvokeRequired)
      {
        logTextBox.Invoke(new Action(ClearLog));
      }

      else
      {
        logTextBox.Clear();
      }
    }

    public void UpdateProgress(int value)
    {
      if (progressBar.InvokeRequired)
      {
        progressBar.Invoke(new Action<int>(UpdateProgress), value);
      }

      else
      {
        progressBar.Value = value;
      }
    }

    private void btnBrowseSource_Click(object sender, EventArgs e)
    {
      BrowseSourceClicked?.Invoke(this, EventArgs.Empty);
    }

    private void btnBrowseDestination_Click(object sender, EventArgs e)
    {
      BrowseDestinationClicked?.Invoke(this, EventArgs.Empty);
    }

    private void btnSync_Click(object sender, EventArgs e)
    {
      SyncButtonClicked?.Invoke(this, EventArgs.Empty);
    }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}