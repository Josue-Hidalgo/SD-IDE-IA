using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process terminal;
        public MainWindow()
        {
            InitializeComponent();

            CodeEditor.EnsureCoreWebView2Async();

            string path = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    Directory.GetCurrentDirectory(),
                    @"..\..\..\Monaco\main.html"
                )
            );

            CodeEditor.Source = new Uri(path);

            terminal = new Process();
            terminal.StartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = @"/K echo 'Hello world!'"
            };
            terminal.OutputDataReceived += p_OutputDataReceived;
            terminal.ErrorDataReceived += p_ErrorDataReceived;
            terminal.Start();
            terminal.BeginOutputReadLine();
            terminal.BeginErrorReadLine();
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Dispatcher.Invoke(() =>
                {
                    TerminalO.AppendText(e.Data + Environment.NewLine);
                    TerminalO.ScrollToEnd();
                });
            }
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Dispatcher.Invoke(() =>
                {
                    TerminalO.AppendText(e.Data + Environment.NewLine);
                    TerminalO.ScrollToEnd();
                });
            }
        }

        public void TerminalInputKD(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                terminal.StandardInput.WriteLine(TerminalI.Text);
                TerminalI.Clear();
            }
        }

        public void ShutDown(object sender, EventArgs e)
        {

            if(terminal != null && !terminal.HasExited)
            {
                terminal.OutputDataReceived -= p_OutputDataReceived;
                terminal.ErrorDataReceived -= p_ErrorDataReceived;
                terminal.Kill();
            }
            Application.Current.Shutdown();
        }
    }

}
