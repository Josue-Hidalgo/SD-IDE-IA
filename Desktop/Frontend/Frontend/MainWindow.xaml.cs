using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
//using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
            AcademicGrid.Width = 0;
            AcademicFrame.Navigate(new LRPage());
        }

        public void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        public void StartTerminal(string path, bool integrated = false)
        {
            terminal = new Process();
            if (integrated)
            {
                terminal.StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    RedirectStandardInput = true,
                    RedirectStandardError = false,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = @"/K py -i 2>&1"
                };
            }
            else
            {
                terminal.StartInfo = new ProcessStartInfo("py")
                {
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $@" -u {path}"
                };
            }
            terminal.Start();
            var stdo = terminal.StandardOutput;
            Task.Run(async () =>
            {
                char[] buffer = new char[1];

                while (!stdo.EndOfStream)
                {
                    int read = await stdo.ReadAsync(buffer, 0, 1);

                    if (read > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TerminalO.AppendText(buffer[0].ToString());
                            TerminalO.ScrollToEnd();
                        });
                    }
                }
                Console.WriteLine("tuki?");
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
                terminal = null;
            }
            TerminalGrid.Height = 0;
            TerminalO.Text = "";
            TerminalI.Text = "";
            terminalOpen = false;
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

        public void KCTerminal(object sender, EventArgs e)
        {
            CloseTerminal();
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
            if (selectedFile)
            {
                CloseTerminal();
                StartTerminal(FileSelected);
            }
        }

        public void OITerminal(object sender, EventArgs e)
        {
            CloseTerminal();
            StartTerminal("",true);
        }

        public void OAArea(object sender, EventArgs e)
        {
            if(AcademicGrid.Width != 0)
            {
                AcademicGrid.Width = 0;
            }
            else
            {
                AcademicGrid.Width = 200;
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
