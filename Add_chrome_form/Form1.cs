using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Add_chrome_form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                int size = 0;//300 * Convert.ToInt32(b % screenWidth);
                int size2 = 10; //410 * Convert.ToInt32(b / screenWidth);
                size = 240 * Convert.ToInt32(i % 220);
                size2 = 300 * Convert.ToInt32(i / 300);
                Task.Run(() =>
                {
                    var service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;
                    service.SuppressInitialDiagnosticInformation = true;
                    ChromeOptions options = new ChromeOptions();
                    options.AddArgument("--window-size=310,420");
                    options.AddArguments(new string[]
        {
      // "--disable-notifications",
       
            "--app=https://d.facebook.com/",

       "--ignore-certificate-errors",

       "--disable-popup-blocking",

        });
                    var chrome = new ChromeDriver(service, options);
                    chrome.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(120));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)chrome;
                    string title = (string)js.ExecuteScript($"document.title = 'Huymanh - {i.ToString()}'"); i++;//đặt tên chrome để get hWnd
                    var pr = GetProcess("chrome", chrome.Title);
                    Panel pt = new Panel();
                    pt.Width = 240;
                    pt.Height = 300;
                    pt.AutoScroll = true;
                    pt.AutoSize = true;
                    pt.Location = new Point(size, size2);//vị trí panel
                    pt.SetAutoScrollMargin(300, 400);
                    pt.BorderStyle = BorderStyle.FixedSingle;

                    this.Invoke((Action)(() =>
                    {

                        panel1.Controls.Add(pt);
                        foreach (ProcessThread thread in pr.Threads)
                        {
                            EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                            {
                                // Kiểm tra xem cửa sổ có hiển thị hay không
                                if (IsWindowVisible(hWnd))
                                {
                                    // In handle của cửa sổ
                                    Console.WriteLine("Handle: " + hWnd);

                                    // Hiển thị cửa sổ và đưa nó lên phía trước
                                    ShowWindow(hWnd, SW_RESTORE);
                                    SetForegroundWindow(hWnd);

                                    // Thiết lập kiểu của cửa sổ thành WS_CHILD
                                    int windowStyle = GetWindowLong(hWnd, GWL_STYLE);
                                    SetWindowLong(hWnd, GWL_STYLE, windowStyle | WS_CHILD | WS_VISIBLE);

                                    // Thiết lập cha của cửa sổ là Panel trong Form (panel1 là tên của Panel trên Form)
                                    SetParent(hWnd, pt.Handle);

                                    // Di chuyển cửa sổ vào Panel (ví dụ, 10, 10 là tọa độ mới)
                                    MoveWindow(hWnd, 4, 10, 240, 300, true);
                                }

                                return true;
                            }, IntPtr.Zero);
                        }
                    }));
                });
            }
        }
        const int SW_RESTORE = 9;
        const int GWL_STYLE = -16;
        const int WS_CHILD = 0x40000000;
        const int WS_VISIBLE = 0x10000000;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);
        Process GetProcess(string processName, string title)
        {
            Process process = null;
            while (true)
            {
                if (processName != "")
                    process = Process.GetProcessesByName(processName).Where(x => x.MainWindowTitle.Contains(title)).FirstOrDefault();
                else
                    process = Process.GetProcesses().Where(x => x.MainWindowTitle.Contains(title)).FirstOrDefault();
                if (process != null)
                {
                    return process;
                }
            }
        }
    }
}
