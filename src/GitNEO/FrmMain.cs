using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitNEO
{
    public partial class FrmMain : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private string _appConfigFile;

        public FrmMain()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeContextMenu();
            InitializeForm();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.module,
                Text = "GitNEO - Clone Git Helper",
                Visible = true
            };

            notifyIcon.MouseClick += notifyIcon_MouseClick;
            this.FormClosing += MainForm_FormClosing;
        }

        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Clone Git", CloneGit_Click);
            contextMenu.MenuItems.Add(new MenuItem("-"));
            contextMenu.MenuItems.Add("Pengaturan", Pengaturan_Click);
            contextMenu.MenuItems.Add("Tentang", Tentang_Click);
            contextMenu.MenuItems.Add(new MenuItem("-"));
            contextMenu.MenuItems.Add("Keluar", Keluar_Click);

            notifyIcon.ContextMenu = contextMenu;
        }

        private void InitializeForm()
        {
            _appConfigFile = $"{Utils.appPath()}\\GitNEO.exe.config";
            chkStartup.Checked = IsAppInStartup();
            txtDir.Text = AppConfigHelper.GetValue("dir", _appConfigFile);
            chkOpen.Checked = AppConfigHelper.GetValue("open", _appConfigFile).ToLower() == "true" ? true : false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void CloneGit_Click(object sender, EventArgs e)
        {
            using (var dlg = new FrmDialog("Inputkan alamat GitHub yang akan di Clone", "Git URL"))
            {
                dlg.InputValue = String.Empty;
                dlg.ShowDialog(this);

                if (dlg.DialogResult == DialogResult.OK)
                {
                    var v = dlg.InputValue.Trim();
                    if (!String.IsNullOrEmpty(v) && IsValidGitHubLink(v))
                    {
                        string repositoryName = GetRepositoryName(v);
                        string targetFolder = GetTargetFolder(repositoryName, txtDir.Text);

                        bool cloneSuccess = CloneRepository(v, targetFolder);
                        Utils.showToast(cloneSuccess ? "SUCCESS" : "ERROR", cloneSuccess ? "Clone Berhasil." : "Clone Gagal.");

                        if (cloneSuccess && chkOpen.Checked)
                            OpenFolder(targetFolder);
                    }
                    else
                    {
                        Utils.showToast("WARNING", "Link GitHub tidak valid.");
                    }
                }
            }
        }

        #region Git Section
        static string GetRepositoryName(string repositoryUrl)
        {
            Uri uri = new Uri(repositoryUrl);
            return Path.GetFileNameWithoutExtension(uri.LocalPath);
        }

        static string GetTargetFolderOld(string repositoryName, string dir)
        {
            dir = @"D:\DEVELOPMENT\GitNEO";

            string targetFolder = Path.Combine(dir, repositoryName);

            if (Directory.Exists(targetFolder))
            {
                FrmDialog dlg = new FrmDialog("Folder sudah ada. Masukkan nama baru:", "Input Nama Baru")
                {
                    InputValue = repositoryName
                };
                dlg.ShowDialog();

                if (dlg.DialogResult == DialogResult.OK)
                {
                    string newName = dlg.InputValue.Trim();

                    if (!String.IsNullOrEmpty(newName))
                    {
                        targetFolder = Path.Combine(dir, newName);
                    }
                }
            }

            return targetFolder;
        }

        static string GetTargetFolder(string repositoryName, string dir)
        {

            string targetFolder = Path.Combine(dir, repositoryName);

            while (Directory.Exists(targetFolder))
            {
                FrmDialog dlg = new FrmDialog("Folder sudah ada. Masukkan nama baru:", "Input Nama Baru")
                {
                    InputValue = repositoryName
                };
                dlg.ShowDialog();

                if (dlg.DialogResult == DialogResult.OK)
                {
                    string newName = dlg.InputValue.Trim();

                    if (!String.IsNullOrEmpty(newName))
                    {
                        targetFolder = Path.Combine(dir, newName);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return targetFolder;
        }

        static void OpenFolder(string folderPath)
        {
            try
            {
                System.Diagnostics.Process.Start(folderPath);
            }
            catch (Exception ex)
            {
                Utils.showToast("WARNING", $"Error opening folder: {ex.Message}");
            }
        }

        static bool IsValidGitHubLink(string link)
        {
            string pattern = @"^https:\/\/github\.com\/.*\.git$";

            Match match = Regex.Match(link, pattern, RegexOptions.IgnoreCase);

            return match.Success;
        }

        static bool CloneRepository(string repositoryUrl, string targetFolder)
        {
            try
            {
                using (Process gitProcess = new Process())
                {
                    gitProcess.StartInfo.FileName = "git";
                    gitProcess.StartInfo.Arguments = $"clone {repositoryUrl} {targetFolder}";
                    gitProcess.StartInfo.UseShellExecute = false;
                    gitProcess.StartInfo.RedirectStandardOutput = true;
                    gitProcess.StartInfo.RedirectStandardError = true;
                    gitProcess.StartInfo.CreateNoWindow = true;

                    gitProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine(e.Data);
                        }
                    };

                    gitProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine(e.Data);
                        }
                    };

                    gitProcess.Start();
                    gitProcess.BeginOutputReadLine();
                    gitProcess.BeginErrorReadLine();

                    gitProcess.WaitForExit();

                    return gitProcess.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        #endregion

        private void Pengaturan_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
            Show();
        }

        private void Tentang_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
            Show();
        }

        private void Keluar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.tabControl1.SelectedTab = this.tabPage1;
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            string selectedFolderPath = BrowseFolder();

            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                txtDir.Text = selectedFolderPath;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            UpdateStartupSettings();
            SaveAppConfig();
        }

        private void UpdateStartupSettings()
        {
            if (chkStartup.Checked)
            {
                AddAppToStartup();
            }
            else
            {
                RemoveAppFromStartup();
            }
        }

        private void SaveAppConfig()
        {
            AppConfigHelper.SaveValue("dir", txtDir.Text, _appConfigFile);
            AppConfigHelper.SaveValue("open", chkOpen.Checked.ToString(), _appConfigFile);
        }

        static string BrowseFolder()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Pilih Folder";
                folderBrowserDialog.ShowNewFolderButton = true;

                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    return folderBrowserDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

        private void AddAppToStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey != null)
            {
                registryKey.SetValue(Application.ProductName, Application.ExecutablePath);
                registryKey.Close();
            }
        }

        private void RemoveAppFromStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey != null)
            {
                registryKey.DeleteValue(Application.ProductName, false);
                registryKey.Close();
            }
        }

        private bool IsAppInStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            if (registryKey != null)
            {
                object value = registryKey.GetValue(Application.ProductName);
                registryKey.Close();

                return (value != null);
            }

            return false;
        }
    }
}
