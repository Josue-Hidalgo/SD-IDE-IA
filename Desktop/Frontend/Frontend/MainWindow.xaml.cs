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
using Frontend.Pages;
using LibGit2Sharp; // Pa'l Git (Github)
using WinForms = System.Windows.Forms; // Alias pa' evitar ambigüedad


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
        bool terminalOpen = false;
        bool loggedIn = false; // <------ CAMBIO loged por loggedIn 

        // Variables pa'l Git
        GitPage gitPage = null;

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
            TerminalGrid.Height = 0;
            AcademicFrame.Navigate(new LRPage());
        }

        public void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        public void StartTerminal(string path)
        {
            terminal = new Process();
            terminal.StartInfo = new ProcessStartInfo("py")
            {
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $@" -u {path}"
            };
            terminal.Start();
            Task.Run(async () =>
            {
                char[] buffer = new char[1];

                while (!terminal.StandardOutput.EndOfStream)
                {
                    int read = await terminal.StandardOutput.ReadAsync(buffer, 0, 1);

                    if (read > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TerminalO.AppendText(buffer[0].ToString());
                            TerminalO.ScrollToEnd();
                        });
                    }
                }
            });
            Task.Run(async () =>
            {
                char[] buffer = new char[1];

                while (!terminal.StandardError.EndOfStream)
                {
                    int read = await terminal.StandardError.ReadAsync(buffer, 0, 1);

                    if (read > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TerminalO.AppendText(buffer[0].ToString());
                            TerminalO.ScrollToEnd();
                        });
                    }
                }
            });
            TerminalGrid.Height = 200;
            terminalOpen = true;
        }

        public void CloseTerminal()
        {
            if (terminal != null && !terminal.HasExited)
            {
                terminal.Kill();
                terminal.Dispose();
            }
            TerminalGrid.Height = 0;
            TerminalO.Text = "";
            TerminalI.Text = "";
            terminalOpen = false;
        }

        private void KCTerminal(object sender, EventArgs e)
        {
            CloseTerminal();
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
            if(e.Key == Key.Enter && terminalOpen)
            {
                terminal.StandardInput.WriteLine(TerminalI.Text);
                TerminalO.AppendText(TerminalI.Text);
                TerminalO.AppendText("\n");
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
            CloseTerminal();
        }
       
        public async void SaveFileAs(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";
            if (SFD.ShowDialog() == true)
            {
                string content = await CodeEditor.CoreWebView2.ExecuteScriptAsync("getValue();");
                Console.WriteLine(content);
                // ExecuteScriptAsync devuelve el valor JSON-encoded, así que hay que limpiar las comillas
                content = content.Trim('"');
                File.WriteAllText(SFD.FileName, content.Replace("\\\\n","\\hi").Replace("\\n", "\n").Replace("\\hi", "\\n")
                    .Replace("\\\\r", "\\hi").Replace("\\r", "\r").Replace("\\hi", "\\r")
                    .Replace("\\\\t", "\\hi").Replace("\\t", "\t").Replace("\\hi", "\\t")
                    .Replace("\\\"", "\"").Replace("\\\'","\'").Replace("\\\\","\\"));
                FileName.Text = System.IO.Path.GetFileName(SFD.FileName);
                FileSelected = SFD.FileName;
                selectedFile = true;
            }
        }

        public void RunCode(object sender, EventArgs e)
        {
            if (selectedFile && loggedIn)
            //if (selectedFile)
            {
                CloseTerminal();
                StartTerminal(FileSelected);
            }
        }

        public void ResizeW(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
       
        public void ShutDown(object sender, EventArgs e)
        {
            CloseTerminal();
            Application.Current.Shutdown();
        }

        // GIT STUFF
        public void OpenRepo(object sender, EventArgs e)
        {
            var dialog = new WinForms.FolderBrowserDialog
            {
                Description = "Selecciona la carpeta del repositorio Git"
            };

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                string repoPath = dialog.SelectedPath;

                if (!Repository.IsValid(repoPath))
                {
                    MessageBox.Show(
                        "La carpeta seleccionada no es un repositorio Git válido.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                gitPage = new GitPage();
                gitPage.LoadRepo(repoPath);
                AcademicFrame.Navigate(gitPage);
            }
        }

    }

}
