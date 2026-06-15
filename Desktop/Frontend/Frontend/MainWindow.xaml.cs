using Frontend.Decorator;
using Frontend.Pages;
using LibGit2Sharp; // Pa'l Git (Github)
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
//using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Frontend
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process terminal;
        string FileSelected = "";
        string FolderSelected = "";

        IScript script = null;
        SignedScript signedScript = null;

        Stack<string> FolderFILO = new Stack<string>();
        bool selectedFile = false;
        bool terminalOpen = false;
        bool ValidRepo = false;
        GitPage gitPage = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ShortcutsCommandContext(this);

            TerminalGrid.Height = 0;
            AcademicGrid.Width = 0;
            FilesGrid.Width = 0;
            backFolderBTN.Width = 0;

            // Espera a que WebView2 esté listo antes de navegar a Monaco.
            CodeEditor.CoreWebView2InitializationCompleted += OnWebViewReady;
            CodeEditor.EnsureCoreWebView2Async();
        }

        private void OnWebViewReady(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;

            // Busca Monaco relativo al ejecutable; si no existe ahí,
            // cae al path de desarrollo (relativo al working directory de VS).
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string monacoRelativeToExe = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(exeDir, @"Monaco\main.html"));
            string monacoRelativeToDev = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Monaco\main.html"));

            string path = File.Exists(monacoRelativeToExe)
                ? monacoRelativeToExe
                : monacoRelativeToDev;

            CodeEditor.CoreWebView2.Navigate(new Uri(path).AbsoluteUri);
        }

        public void MinimizeW(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void ResizeW(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
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

        public void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
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
                terminal.StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    RedirectStandardInput = true,
                    RedirectStandardError = false,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"/K py -u \"{path}\" 2>&1 & echo _-_Python-File-End_-_"
                };
            }

            terminal.Start();
            var stdo = terminal.StandardOutput;
            Task.Run(async () =>
            {
                int blocks = 1024;
                string text = "";
                char[] buffer = new char[blocks];
                bool finished = true;

                while (!stdo.EndOfStream && finished)
                {
                    int read = await stdo.ReadAsync(buffer, 0, blocks);

                    if (read > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            string chunk = new string(buffer, 0, read);
                            text += chunk;
                            TerminalO.Text = text;
                            TerminalO.ScrollToEnd();
                        });

                        if (text.Contains("_-_Python-File-End_-_"))
                        {
                            finished = false;
                            Dispatcher.Invoke(() =>
                            {
                                TerminalO.Text = TerminalO.Text.Replace("_-_Python-File-End_-_","");
                                TerminalO.ScrollToEnd();
                            });
                        }
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
                terminal.WaitForExit();
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
            if (e.Key == Key.Enter && terminalOpen)
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
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";
            OFD.FilterIndex = 0;
            if (OFD.ShowDialog() == true)
            {
                OpFiAux(OFD.FileName);
            }
            if (!selectedFile)
            {
                FileName.Text = "";
            }
        }

        public void OpFiAux(string route)
        {

            // Verificar integridad antes de abrir
            string csvPath = System.IO.Path.ChangeExtension(route, ".signatures.csv");
            var baseScript = new Script(route);
            signedScript = new SignedScript(new FormattedScript(baseScript, () => {}), csvPath);
            script = signedScript;

            if (!signedScript.VerifySignature())
            {
                MessageBox.Show(
                    $"The file '{System.IO.Path.GetFileName(route)}' does not have a valid signature.",
                    "Access Denied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                CF();
                _ = CodeEditor.CoreWebView2.ExecuteScriptAsync("changeLanguage('txt');");
                signedScript = null;
                script = null;
                return;
            }

            _ = CodeEditor.CoreWebView2.ExecuteScriptAsync("changeLanguage('python');");
            FileSelected = route;
            TextReader reader = new StreamReader(route);
            CodeEditor.CoreWebView2.ExecuteScriptAsync($"setValue(\"{(reader.ReadToEnd()).Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")}\");");
            reader.Close();
            FileName.Text = System.IO.Path.GetFileName(route);
            selectedFile = true;
        }

        public void OpenDirectory(object sender, EventArgs e)
        {
            if (FolderSelected != "")
            {
                FileList.Items.Clear();
                FolderFILO.Clear();
            }
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ODIR(dialog.FileName);
                FilesGrid.Width = 200;
            }
        }

        public void ODIR(string p)
        {
            FileList.Items.Clear();
            FolderSelected = p;
            DirNameTB.Text = Path.GetFileName(p);
            foreach (string d in Directory.GetDirectories(p))
            {
                ListBoxItem i = new ListBoxItem();

                i.Content = Path.GetFileName(d);
                i.Background = new SolidColorBrush(Color.FromRgb(28, 12, 46));
                FileList.Items.Add(i);
            }
            foreach (string d in Directory.GetFiles(p))
            {
                FileList.Items.Add(Path.GetFileName(d));
            }
        }

        public void FileChange(object sender, SelectionChangedEventArgs e)
        {
            if (FileList.SelectedItem == null) return;
            string p;
            if (FileList.SelectedItem is ListBoxItem)
            {
                ListBoxItem it = (ListBoxItem)FileList.SelectedItem;
                p = FolderSelected + "\\" + it.Content.ToString();
            }
            else
            {
                p = FolderSelected + "\\" + FileList.SelectedItem;
            }
            Console.WriteLine(p);
            if (File.Exists(p))
            {
                OpFiAux(p);

            }
            else if (Directory.Exists(p))
            {
                FolderFILO.Push(FolderSelected);
                backFolderBTN.Width = 30;
                ODIR(p);
            }
        }

        public void CloseFolder(object sender, EventArgs e)
        {
            CF();
            FolderSelected = "";
            FileList.Items.Clear();
            FilesGrid.Width = 0;
            backFolderBTN.Width = 0;
            FolderFILO.Clear();
        }

        public void CloseFile(object sender, EventArgs e)
        {
            CF();
        }

        public void CF()
        {
            CodeEditor.CoreWebView2.ExecuteScriptAsync($"setValue(\"\");");
            FileName.Text = "";
            FileSelected = "";
            selectedFile = false;
            CloseTerminal();
        }

        public void LastDIR(object sender, EventArgs e)
        {
            ODIR(FolderFILO.Pop());
            if (FolderFILO.Count == 0)
            {
                backFolderBTN.Width = 0;
            }
        }

        public void RefreshFolder(object sender, EventArgs e)
        {
            FileList.Items.Clear();
            if(FolderSelected != "")
                ODIR(FolderSelected);
        }

        public async void SaveFileAs(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";

            if (SFD.ShowDialog() == true)
            {
                string content = await CodeEditor.CoreWebView2.ExecuteScriptAsync("getValue();");

                content = JsonConvert.DeserializeObject<string>(content);
                File.WriteAllText(SFD.FileName, content);

                script = new Script(SFD.FileName);
                script = new FormattedScript(script, () =>
                {
                    _ = CodeEditor.CoreWebView2.ExecuteScriptAsync("changeLanguage('python');");
                });
                string csvPath = System.IO.Path.ChangeExtension(SFD.FileName, ".signatures.csv");
                signedScript = new SignedScript(script, csvPath);
                script = signedScript;
                signedScript.RegenerateSignature();

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
            StartTerminal("", true);
        }

        public void AcademicBTN(object sender, EventArgs e)
        {
            AcademicFrame.Navigate(new LRPage());
            OpenAcademicArea();
        }

        public void CloseAcademicArea(object sender, EventArgs e)
        {
            AcademicGrid.Width = 0;
        }

        public void OpenAcademicArea()
        {
            AcademicGrid.Width = 200;
        }

        // GIT STUFF
        public void OpenGitW(object sender, EventArgs e)
        {

            var dialog = new CommonOpenFileDialog {IsFolderPicker = true};
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ODIR(dialog.FileName);
                FilesGrid.Width = 200;
                OpenAcademicArea();
                gitPage = new GitPage();
                gitPage.LoadRepo(dialog.FileName);
                AcademicFrame.Navigate(gitPage);
            }
        }
        public void VerifyGit(string path)
        {
            ValidRepo = Repository.IsValid(path);
        }

        public void NewFile(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Python (*.py)|*.py|All Files (*.*)|*.*";

            if (SFD.ShowDialog() == true)
            {
                File.WriteAllText(SFD.FileName, "");

                script = new Script(SFD.FileName);
                script = new FormattedScript(script, () =>
                {
                    _ = CodeEditor.CoreWebView2.ExecuteScriptAsync("changeLanguage('python');");
                });
                string csvPath = System.IO.Path.ChangeExtension(SFD.FileName, ".signatures.csv");
                signedScript = new SignedScript(script, csvPath);
                script = signedScript;
                signedScript.RegenerateSignature();

                FileName.Text = System.IO.Path.GetFileName(SFD.FileName);
                FileSelected = SFD.FileName;
                selectedFile = true;
                OpFiAux(FileSelected);
                RefreshFolder(null, null);
            }
        }
        public async void Save(object sender, EventArgs e)
        {
            if (selectedFile) {
                string content = await CodeEditor.CoreWebView2.ExecuteScriptAsync("getValue();");

                content = JsonConvert.DeserializeObject<string>(content);
                File.WriteAllText(FileSelected, content);

                script = new Script(FileSelected);
                script = new FormattedScript(script, () =>
                {
                    _ = CodeEditor.CoreWebView2.ExecuteScriptAsync("changeLanguage('python');");
                });
                string csvPath = System.IO.Path.ChangeExtension(FileSelected, ".signatures.csv");
                signedScript = new SignedScript(script, csvPath);
                script = signedScript;
                signedScript.RegenerateSignature();

                FileName.Text = System.IO.Path.GetFileName(FileSelected);
            }
            else
            {
                SaveFileAs(null,null);
            }
        }
    }
}
