using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BasicAutoPatch
{
    public class IntegrityCheck
    {
        public static async Task<bool> VerifyAndRepairGameFilesAsync()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Download the hash list from server
                    string hashListContent = await client.DownloadStringTaskAsync(
                        $"{Configrations.ServerAddress}/{Configrations.HashListFile}");

                    // Verify hash list integrity (optional)
                    //if (!string.IsNullOrEmpty(Configrations.HashListHash) &&
                    //    !VerifyContentHash(hashListContent, Configrations.HashListHash))
                    //{
                    //    MessageBox.Show("Hash list verification failed. File may be tampered with.",
                    //                  "Security Warning",
                    //                  MessageBoxButtons.OK,
                    //                  MessageBoxIcon.Warning);
                    //    return false;
                    //}

                    // Parse and verify files
                    string[] lines = hashListContent.Split(
                        new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    var corruptedFiles = new List<FileVerificationInfo>();
                    bool allValid = true;

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        // Format: "relative/path|expected_hash|download_url" (@@ or | separator)
                        string[] parts = line.Split(new[] { "@@", "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            var fileInfo = new FileVerificationInfo
                            {
                                RelativePath = parts[0].Trim(),
                                ExpectedHash = parts[1].Trim().ToLower(),
                                DownloadUrl = parts.Length >= 3 ? parts[2].Trim() : null
                            };

                            if (!VerifySingleFile(fileInfo))
                            {
                                corruptedFiles.Add(fileInfo);
                                allValid = false;
                            }
                        }
                    }

                    if (!allValid)
                    {
                        if (MessageBox.Show("Some game files are corrupted. Would you like to repair them?",
                              "File Corruption Detected",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            return await RepairCorruptedFilesAsync(corruptedFiles);

                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during file verification: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        public static async Task<bool> RepairCorruptedFilesAsync(List<FileVerificationInfo> corruptedFiles)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        // Update progress UI if needed
                    };

                    foreach (var file in corruptedFiles)
                    {
                        if (string.IsNullOrEmpty(file.DownloadUrl))
                        {
                            MessageBox.Show($"No download URL available for: {file.RelativePath}",
                                          "Repair Error",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Error);
                            continue;
                        }

                        string fullPath = Path.Combine(Application.StartupPath, file.RelativePath);
                        string tempPath = Path.GetTempFileName();

                        try
                        {
                            // Download the file
                            await client.DownloadFileTaskAsync(file.DownloadUrl, tempPath);

                            // Verify downloaded file
                            if (!VerifyFileIntegrity(tempPath, file.ExpectedHash))
                            {
                                MessageBox.Show($"Downloaded file failed verification: {file.RelativePath}",
                                              "Repair Error",
                                              MessageBoxButtons.OK,
                                              MessageBoxIcon.Error);
                                continue;
                            }

                            // Replace corrupted file
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                            File.Copy(tempPath, fullPath, overwrite: true);

                            MessageBox.Show($"Successfully repaired: {file.RelativePath}",
                                          "Repair Complete",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        }
                        finally
                        {
                            if (File.Exists(tempPath))
                                File.Delete(tempPath);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during file repair: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static bool VerifySingleFile(FileVerificationInfo fileInfo)
        {
            string fullPath = Path.Combine(Application.StartupPath, fileInfo.RelativePath);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"File missing: {fileInfo.RelativePath}",
                              "Verification Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return false;
            }

            if (!VerifyFileIntegrity(fullPath, fileInfo.ExpectedHash))
            {
                MessageBox.Show($"File corrupted: {fileInfo.RelativePath}\n" +
                              $"Expected hash: {fileInfo.ExpectedHash}\n" +
                              $"Actual hash: {CalculateFileHash(fullPath)}",
                              "Verification Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        public static bool VerifyFileIntegrity(string filePath, string expectedHash)
        {
            if (!File.Exists(filePath)) return false;
            if (string.IsNullOrWhiteSpace(expectedHash)) return false;

            string actualHash = CalculateFileHash(filePath);
            return actualHash?.ToLower() == expectedHash.Trim().ToLower();
        }

        public static string CalculateFileHash(string filePath)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = sha256.ComputeHash(stream);
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static bool VerifyContentHash(string content, string expectedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                byte[] hashBytes = sha256.ComputeHash(contentBytes);
                string actualHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return actualHash == expectedHash.ToLower();
            }
        }

        public class FileVerificationInfo
        {
            public string RelativePath { get; set; }
            public string ExpectedHash { get; set; }
            public string DownloadUrl { get; set; }
        }
    }
}
