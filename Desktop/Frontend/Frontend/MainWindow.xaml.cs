using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Security.Cryptography;
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
        string FileSelected = "";
        bool selectedFile = false;

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

        public void OpenFile(object sender, EventArgs e)
        {
            string route;
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";
            OFD.FilterIndex = 0;
            if (OFD.ShowDialog() == true)
            {
                route = OFD.FileName;
                FileSelected = route;
                TextReader reader = new StreamReader(route);
                CodeEditor.CoreWebView2.ExecuteScriptAsync($"setValue(\"{(reader.ReadToEnd()).Replace("\r","").Replace("\\","\\\\").Replace("\"","\\\"").Replace("\n","\\n")}\");");
                reader.Close();
                FileName.Text = System.IO.Path.GetFileName(route);
                selectedFile = true;
            }
            if (!selectedFile)
            {
                FileName.Text = "";
            }
        }

        public void CloseFile(object sender, EventArgs e)
        {
            CodeEditor.CoreWebView2.ExecuteScriptAsync($"setValue(\"\");");
            FileName.Text = "";
            FileSelected = "";
            selectedFile = false;
        }
        public async void SaveFileAs(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";
            if (SFD.ShowDialog() == true)
            {
                string content = await CodeEditor.CoreWebView2.ExecuteScriptAsync("getValue();");
                // ExecuteScriptAsync devuelve el valor JSON-encoded, así que hay que limpiar las comillas
                content = content.Trim('"');
                File.WriteAllText(SFD.FileName, content);
            }

            if (!selectedFile)
            {
                FileName.Text = "";
            }
        }

        public void RunCode(object sender, EventArgs e)
        {
            if (selectedFile)
            {
                closeTerminal();
                terminal.OutputDataReceived += p_OutputDataReceived;
                terminal.ErrorDataReceived += p_ErrorDataReceived;
                terminal.Start();
                terminal.BeginOutputReadLine();
                terminal.BeginErrorReadLine();
                terminal.StandardInput.WriteLine($"python -u \"{FileSelected}\"");
                Console.WriteLine($"python \"{FileSelected}\"");
            }
        }

        public void closeTerminal()
        {
            if (terminal != null && !terminal.HasExited)
            {
                terminal.CancelErrorRead();
                terminal.CancelOutputRead();
                terminal.OutputDataReceived -= p_OutputDataReceived;
                terminal.ErrorDataReceived -= p_ErrorDataReceived;
                terminal.Kill();
            }
        }
        public void ShutDown(object sender, EventArgs e)
        {
            closeTerminal();
            Application.Current.Shutdown();
        }
    }

}
