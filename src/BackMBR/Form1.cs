using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackMBR
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern void CloseHandle(IntPtr hFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        public string title; // Variable for random title
        public uint GENERIC_ALL = 0x10000000;
        public uint FILE_SHARE_READ = 0x00000001;
        public uint FILE_SHARE_WRITE = 0x00000002;
        public uint OPEN_EXISTING = 3;
        public byte[] buffer = new byte[65536]; // MBR bytes
        public byte[] mbrData;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void rebootComputer()
        {
            Process.Start(@"C:\Windows\system32\shutdown.exe", "/r /t 0");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Generating a setting the random title
            const string chars = "qwertyuiopasdfghjklzxcvbnm"; // The chars for random title
            StringBuilder stringBuilder = new StringBuilder(12);
            Random random = new Random();

            for (int i = 0; i < 12; i++)
            {
                title = stringBuilder.Append(chars[random.Next(chars.Length)]).ToString(); // The random title
            }

            this.Text = title; // Setting title to form

            stringBuilder.Clear(); // Cleaning symbols

            // Getting a drives and adding drives to ComboBox
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject drive in managementObjectSearcher.Get())
            {
                comboBox1.Items.Add(drive["DeviceID"]); // Adding the drives to ComboBox
            }

            managementObjectSearcher.Dispose(); // Closing the ManagementObjectSearcher
        }

        private void поверхВсехОконToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TopMost == true)
            {
                this.TopMost = false;
            }

            else
            {
                this.TopMost = true;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0); // Killing the program with exit code 0
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveBackup.ShowDialog();

            if (saveBackup.FileName != "") // If file in save dialog is selected
            {
                var hFile = CreateFile(comboBox1.Text, GENERIC_ALL, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero); // Opening a selected drive
                
                ReadFile(hFile, buffer, 65536, out uint lpNumberOfBytesRead, IntPtr.Zero); // Reading MBR file

                File.WriteAllBytes(saveBackup.FileName, buffer); // Writing a MBR backup

                CloseHandle(hFile); // Closing a drive

                MessageBox.Show("Backup MBR успешно создан!", "BackMBR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы действительно хотите перезаписать MBR? Перезапись MBR из поврежденного backup'а или неправильно выбранного файла может привести к полной потере данных", "BackMBR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            openBackup.ShowDialog();

            if (openBackup.FileName != "") // If file in save dialog is selected
            {
                mbrData = File.ReadAllBytes(openBackup.FileName);

                var hFile = CreateFile(comboBox1.Text, GENERIC_ALL, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero); // Opening a selected drive

                WriteFile(hFile, mbrData, 65536, out uint lpNumberOfBytesWritten, IntPtr.Zero); // Overwriting a MBR

                CloseHandle(hFile); // Closing a drive

                MessageBox.Show("MBR успешно восстановлен!", "BackMBR", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (MessageBox.Show("Для продолжения требуется перезагрузка компьютера. Перезагрузить компьютер?", "BackMBR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rebootComputer();
                }
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }
    }
}
