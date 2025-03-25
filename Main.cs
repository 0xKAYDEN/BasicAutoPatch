using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BasicAutoPatch
{
    public partial class Main : Form
    {
        public List<string> PatchList;
        public List<string> HashList;
        private WebClient webClient;
        private int currentPatchIndex = 0;
        private string tempDownloadPath = Path.Combine(Path.GetTempPath(), "PatchDownload.zip");
        private long totalExtractBytes = 0;
        private long extractedBytes = 0;
        private bool isExtracting = false;
        private Stopwatch downloadStopwatch = new Stopwatch();
        private long previousBytesReceived = 0;

        public Main()
        {

            PatchList = new List<string>();
            HashList = new List<string>();
            InitializeComponent();
            Configrations.ClientVersion = (uint)CheckVersion();
            LoadPatchList();
            HasUpdate(); // Call this to update the button text initially

        }
        private void DownloadNextPatch()
        {
            if (currentPatchIndex >= PatchList.Count)
            {
                lblStatus.Visible = false;
                lblDownloadSpeed.Visible = false;
                lblFile.Visible = false;
                lblSpeed.Visible = false;
                progressBar1.Visible = false;
                if (this.Height != 111)
                    this.Height = 111;
                this.Width = 297;
                btnStart.Location = new Point(12, 12);
                this.Height = 139;
                radDx8.Visible = true;
                radDx9.Visible = true;
                checkNoEffects.Visible = true;
                btnStart.Text = "Play";
                btnStart.Enabled = true;
                MessageBox.Show("All patches have been downloaded and installed!", "Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string patchUrl = PatchList[currentPatchIndex];
            string patchFileName = Path.GetFileName(patchUrl);

            lblStatus.Text = $"Downloading: {patchFileName}";
            lblFile.Text = $"File Hash: {Configrations.PatchHash}";
            progressBar1.Value = 0;
            lblDownloadSpeed.Text = "Starting download...";
            lblSpeed.Text = string.Empty;

            downloadStopwatch.Restart();
            previousBytesReceived = 0;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            webClient.DownloadFileAsync(new Uri(patchUrl), tempDownloadPath);
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                progressBar1.Value = e.ProgressPercentage;
                lblDownloadSpeed.Text = $"Downloaded: {FormatFileSize(e.BytesReceived)} / {FormatFileSize(e.TotalBytesToReceive)}";

                if (downloadStopwatch.IsRunning && downloadStopwatch.ElapsedMilliseconds > 0)
                {
                    long bytesReceived = e.BytesReceived;
                    long bytesDiff = bytesReceived - previousBytesReceived;
                    double speed = bytesDiff / (downloadStopwatch.ElapsedMilliseconds / 1000.0);
                    lblSpeed.Text = $"Speed: {FormatFileSize((long)speed)}/s";
                    previousBytesReceived = bytesReceived;
                    downloadStopwatch.Restart();
                }
            });
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                downloadStopwatch.Stop();

                if (e.Error != null)
                {
                    MessageBox.Show($"Error downloading patch: {e.Error.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnStart.Enabled = true;
                    return;
                }

                if (!File.Exists(tempDownloadPath) || new FileInfo(tempDownloadPath).Length == 0)
                {
                    MessageBox.Show("Downloaded file is empty or missing", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!VerifyPatchHash(tempDownloadPath))
                {
                    MessageBox.Show("Patch file hash verification failed. The file may be corrupted.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnStart.Enabled = true;
                    return;
                }

                try
                {
                    lblStatus.Text = "Preparing extraction...";
                    progressBar1.Value = 0;
                    Application.DoEvents();

                    // Calculate total extraction size
                    totalExtractBytes = 0;
                    using (var archive = ZipFile.OpenRead(tempDownloadPath))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!string.IsNullOrEmpty(entry.Name))
                            {
                                totalExtractBytes += entry.Length;
                            }
                        }
                    }

                    isExtracting = true;
                    extractedBytes = 0;

                    // Start extraction thread
                    new Thread(ExtractPatch).Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error preparing extraction: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnStart.Enabled = true;
                }
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (webClient != null && webClient.IsBusy)
            {
                webClient.CancelAsync();
            }

            // Prevent closing while extracting to avoid corruption
            if (isExtracting)
            {
                if (MessageBox.Show("A patch is currently being extracted. Closing now may corrupt your installation. Are you sure you want to exit?",
                                   "Warning",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        public void HasUpdate()
        {
            if (PatchList.Count == 0)
            {
                btnStart.Text = "No Patches";
                return;
            }
            // Get the last patch number (assuming patches are in order)
            Configrations.ServerVersion = Convert.ToUInt32(Path.GetFileNameWithoutExtension(PatchList[PatchList.Count - 1]).Split('/')[^1]);

            if (Configrations.ServerVersion > Configrations.ClientVersion)
            {
                lblStatus.Visible = true;
                lblDownloadSpeed.Visible = true;
                lblFile.Visible = true;
                lblSpeed.Visible = true;
                progressBar1.Visible = true;
                if (this.Height != 224)
                    this.Height = 224;
                this.Width = 642;
                btnStart.Location = new Point(169, 125);
                btnStart.Text = "Download";
            }
            else if (Configrations.ServerVersion == Configrations.ClientVersion)
            {
                lblStatus.Visible = false;
                lblDownloadSpeed.Visible = false;
                lblFile.Visible = false;
                lblSpeed.Visible = false;
                progressBar1.Visible = false;
                if (this.Height != 111)
                    this.Height = 111;
                this.Width = 297;
                btnStart.Location = new Point(12, 12);
                this.Height = 139;
                radDx8.Visible = true;
                radDx9.Visible = true;
                checkNoEffects.Visible = true;
                btnStart.Text = "Play";
            }
            else
            {
                btnStart.Text = "Play (Client is newer)";
            }
        }
        public async void LoadPatchList()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Download the patch list from the server
                    string patchListContent = await client.DownloadStringTaskAsync(Configrations.ServerAddress + Configrations.PatchListFile);

                    // Split the content into lines
                    string[] lines = patchListContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    // Clear existing patches
                    PatchList.Clear();

                    foreach (var line in lines)
                    {
                        var parts = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            Configrations.PatchNumber = uint.Parse(parts[0]);
                            Configrations.PatchHash = parts[1];
                            PatchList.Add($"{Configrations.ServerAddress}/{Configrations.PatchPath}/{Configrations.PatchNumber}.zip");
                            HashList.Add(Configrations.PatchHash);
                        }
                    }

                    // Update UI based on the loaded patches
                    HasUpdate();
                }
            }
            catch (WebException webEx)
            {
                MessageBox.Show($"Failed to download patch list: {webEx.Message}", "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patch list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public int CheckVersion()
        {
            try
            {
                if (File.Exists("version.dat"))
                {
                    return int.Parse(File.ReadAllText("version.dat"));
                }

                MessageBox.Show("version.dat file is missing", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        private bool VerifyPatchHash(string filePath)
        {
            try
            {
                // First ensure the file exists
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Verification Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Get the expected hash from configuration (trim any whitespace)
                string expectedHash = HashList[currentPatchIndex]?.Trim().ToLower();
                if (string.IsNullOrEmpty(expectedHash))
                {
                    MessageBox.Show("No hash value provided in configuration", "Verification Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Calculate the actual file hash
                string actualHash;
                using (var sha256 = SHA256.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = sha256.ComputeHash(stream);
                        actualHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }

                // Debug output to help troubleshoot
                Debug.WriteLine($"Expected Hash: {expectedHash}");
                Debug.WriteLine($"Actual Hash:   {actualHash}");

                // Compare hashes
                bool match = string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);

                if (!match)
                {
                    MessageBox.Show($"Hash mismatch!\nExpected: {expectedHash}\nActual: {actualHash}",
                        "Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return match;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during hash verification: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ExtractPatch()
        {
            try
            {
                string extractPath = Application.StartupPath;

                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Extracting files...";
                });

                using (var archive = ZipFile.Open(tempDownloadPath, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;

                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = $"Extracting: {entry.Name}";
                        });

                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Create(destinationPath))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = entryStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                                extractedBytes += bytesRead;

                                int progress = (int)((extractedBytes * 100) / totalExtractBytes);
                                this.Invoke((MethodInvoker)delegate
                                {
                                    progressBar1.Value = progress;
                                    lblStatus.Text = $"Extracting: {entry.Name} ({progress}%)";
                                });
                            }
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                    File.WriteAllText("version.dat", Configrations.PatchNumber.ToString());
                    Configrations.ClientVersion = Configrations.PatchNumber;
                    lblStatus.Text = "Patch installed successfully!";
                    currentPatchIndex++;
                    isExtracting = false;

                    try { File.Delete(tempDownloadPath); } catch { }

                    DownloadNextPatch();
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Error extracting patch: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnStart.Enabled = true;
                    isExtracting = false;
                });
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Download")
            {
                btnStart.Enabled = false;
                currentPatchIndex = 0;
                Configrations.ClientVersion = (uint)CheckVersion();
                LoadPatchList();
                HasUpdate();
                DownloadNextPatch();
            }
            else if (btnStart.Text == "Play")
            {
                var verificationResult = await IntegrityCheck.VerifyAndRepairGameFilesAsync();

                if (verificationResult)
                {
                    try
                    {
                        if (radDx8.Checked)
                        {
                            Process.Start("Env_DX8//Conqer.exe", "blacknull");
                            this.Close();
                        }
                        if (radDx9.Checked)
                        {
                            Process.Start("Env_DX9//Conqer.exe", "blacknull");
                            this.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error launching game: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var repairResult = await RepairCorruptedFiles();
                    if (repairResult)
                    {
                        // After repair, verify again before launching
                        if (await IntegrityCheck.VerifyAndRepairGameFilesAsync())
                        {
                            Process.Start(Configrations.GameExecutable);
                            this.Close();
                        }
                    }
                }
            }
        }

        private async Task<bool> RepairCorruptedFiles()
        {
            var result = MessageBox.Show("Some game files are corrupted. Would you like to repair them?",
                                      "File Corruption Detected",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Show repair progress UI
                lblStatus.Text = "Repairing corrupted files...";
                progressBar1.Style = ProgressBarStyle.Marquee;
                btnStart.Enabled = false;
                Application.DoEvents();

                // Trigger full update process
                Configrations.ClientVersion = 0;
                File.WriteAllText("version.dat", "0");
                LoadPatchList();
                HasUpdate();

                // Reset UI
                progressBar1.Style = ProgressBarStyle.Blocks;
                btnStart.Enabled = true;

                if (btnStart.Text == "Download")
                {
                    btnStart.PerformClick(); // Start the download process
                }

                return true;
            }
            return false;
        }

        private void RestoreEffects()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string str = System.IO.Path.Combine(baseDirectory, "c3\\effect0");
            string destDirName = System.IO.Path.Combine(baseDirectory, "c3\\effect");
            try
            {
                if (Directory.Exists(str))
                {
                    Directory.Move(str, destDirName);
                    MessageBox.Show("effects restored");
                }
                else
                    MessageBox.Show("Effects Folder is Missing.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void RemoveEffects()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string str = System.IO.Path.Combine(baseDirectory, "c3\\effect");
            string destDirName = System.IO.Path.Combine(baseDirectory, "c3\\effect0");
            try
            {
                if (Directory.Exists(str))
                {
                    Directory.Move(str, destDirName);
                    MessageBox.Show("effects removed");
                }
                else
                    MessageBox.Show("Effects Folder Is Missing.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        private void checkNoEffects_CheckedChanged(object sender, EventArgs e)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (checkNoEffects.Checked)
            {
                RemoveEffects();
            }
            else
            {
                RestoreEffects();
            }
        }
    }
}
