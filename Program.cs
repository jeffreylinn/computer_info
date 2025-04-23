using System;
using System.Net;
using System.Management;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;
using System.Drawing;

namespace SupportApp
{
    public class MainForm : Form
    {
        private Label infoLabel;
        private Button emailButton;
        private string systemInfo;

        public MainForm()
        {
            this.Text = "Support Tool";
            this.Size = new System.Drawing.Size(400, 370);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Logo code (commented out for now, uncomment to use later)
            /*
            try
            {
                PictureBox logo = new PictureBox();
                logo.Image = Image.FromFile("logo.png"); // Assumes logo.png in same folder
                logo.Size = new System.Drawing.Size(32, 32);
                logo.Location = new System.Drawing.Point(5, 5);
                logo.SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(logo);
                this.Text = "  Support Tool";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logo: {ex.Message}", "Support Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */

            systemInfo = GetSystemInfo();

            infoLabel = new Label();
            infoLabel.Text = systemInfo;
            infoLabel.Location = new System.Drawing.Point(20, 20);
            infoLabel.Size = new System.Drawing.Size(350, 220);
            infoLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Controls.Add(infoLabel);

            emailButton = new Button();
            emailButton.Text = "Email Support";
            emailButton.Location = new System.Drawing.Point(150, 270);
            emailButton.Size = new System.Drawing.Size(100, 30);
            emailButton.Click += new EventHandler(EmailButton_Click!);
            this.Controls.Add(emailButton);
        }

        private string GetSystemInfo()
        {
            string info = "System Information\n\n";
            try
            {
                // Serial Number
                string serialNumber = "Unknown";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        serialNumber = obj["SerialNumber"]?.ToString() ?? "Unknown";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    serialNumber = $"Error: {ex.Message}";
                }
                info += $"Serial Number: {serialNumber}\n";

                // Manufacturer
                string manufacturer = "Unknown";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_ComputerSystem");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    manufacturer = $"Error: {ex.Message}";
                }
                info += $"Manufacturer: {manufacturer}\n";

                // Model
                string model = "Unknown";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        model = obj["Model"]?.ToString() ?? "Unknown";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    model = $"Error: {ex.Message}";
                }
                info += $"Model: {model}\n";

                // Blank line
                info += "\n";

                // Hostname
                info += $"Hostname: {Dns.GetHostName()}\n";

                // IP Address
                info += $"IP Address: {Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()}\n";

                // Uptime
                string uptimeStr = "N/A";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
                    foreach (ManagementObject result in searcher.Get())
                    {
                        DateTime lastBoot = ManagementDateTimeConverter.ToDateTime(result["LastBootUpTime"].ToString());
                        TimeSpan uptime = DateTime.Now - lastBoot;
                        uptimeStr = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    uptimeStr = $"Error: {ex.Message}";
                }
                info += $"Uptime: {uptimeStr}\n";

                // System Memory
                string totalMemoryGB = "N/A";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                    foreach (ManagementObject result in searcher.Get())
                    {
                        ulong totalMemoryKB = Convert.ToUInt64(result["TotalVisibleMemorySize"]);
                        double totalMemoryGBValue = totalMemoryKB / (1024.0 * 1024.0); // KB to GB
                        totalMemoryGB = $"{totalMemoryGBValue:F2} GB";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    totalMemoryGB = $"Error: {ex.Message}";
                }
                info += $"System Memory: {totalMemoryGB}\n";

                // Disk Space
                DriveInfo drive = new DriveInfo("C");
                double freeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0);
                double totalSpaceGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                info += $"Free Disk Space (C:): {freeSpaceGB:F2} GB\n";
                info += $"Total Disk Space (C:): {totalSpaceGB:F2} GB\n";
            }
            catch (Exception ex)
            {
                info += $"Unexpected error: {ex.Message}";
            }
            return info;
        }

        private void EmailButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                string email = "Support@your_e-mail.com"; // Change this!
                string subject = Uri.EscapeDataString("Summarize issue here");
                string bodyWithSpace = Uri.EscapeDataString("Provide detailed information on your issue here" + "\n\n\n\n\n" + systemInfo);
                string mailto = $"mailto:{email}?subject={subject}&body={bodyWithSpace}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = mailto,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening email:\n{ex.Message}", "Support Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
