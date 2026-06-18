using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Pages
{
    public partial class GitPage : Page
    {
        private string _repoPath = "";

        private const string CommitPlaceholder = "Commit message...";

        public GitPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Llamado desde MainWindow cuando el usuario abre un repo.
        /// </summary>
        public void LoadRepo(string repoPath)
        {
            _repoPath = repoPath;

            if (!Repository.IsValid(repoPath))
            {
                SetStatus("❌ La carpeta seleccionada no es un repositorio Git válido.");
                return;
            }

            RepoPathTB.Text = repoPath;
            RefreshBranchInfo();
            SetStatus("✅ Repo cargado.");
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private void RefreshBranchInfo()
        {
            if (!Repository.IsValid(_repoPath)) return;

            using (var repo = new Repository(_repoPath))
            {
                CurrentBranchTB.Text = repo.Head.FriendlyName;

                var branches = repo.Branches
                    .Where(b => !b.IsRemote)
                    .Select(b => b.FriendlyName)
                    .ToList();

                BranchCombo.ItemsSource = branches;
                BranchCombo.SelectedItem = repo.Head.FriendlyName;

                MergeBranchCombo.ItemsSource = branches;
            }
        }

        private void SetStatus(string msg)
        {
            StatusTB.Text = msg;
        }

        /// <summary>
        /// Ejecuta git.exe nativo para aprovechar el GCM de Windows.
        /// </summary>
        private (bool success, string output) RunGit(string arguments)
        {
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("git", arguments)
                    {
                        WorkingDirectory = _repoPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                string stdout = p.StandardOutput.ReadToEnd();
                string stderr = p.StandardError.ReadToEnd();
                p.WaitForExit(15000);

                string output = string.IsNullOrWhiteSpace(stdout) ? stderr : stdout;
                return (p.ExitCode == 0, output.Trim());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ─── Operaciones Git ────────────────────────────────────────────────────

        private void DoPull(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_repoPath)) { SetStatus("❌ No hay repo cargado."); return; }

            SetStatus("⏳ Haciendo pull...");
            var (success, output) = RunGit("pull");
            SetStatus(success ? $"✅ Pull: {output}" : $"❌ Pull falló: {output}");
            if (success) RefreshBranchInfo();
        }

        private void DoCommit(object sender, RoutedEventArgs e)
        {
            if (!Repository.IsValid(_repoPath)) { SetStatus("❌ No hay repo cargado."); return; }

            string msg = CommitMsgTB.Text.Trim();
            if (string.IsNullOrEmpty(msg) || msg == CommitPlaceholder)
            {
                SetStatus("❌ Escribe un mensaje de commit.");
                return;
            }

            try
            {
                using (var repo = new Repository(_repoPath))
                {
                    Commands.Stage(repo, "*");

                    if (!repo.Index.Any())
                    {
                        SetStatus("⚠ No hay cambios para commitear.");
                        return;
                    }

                    string name  = repo.Config.Get<string>("user.name")?.Value  ?? "IDEIA User";
                    string email = repo.Config.Get<string>("user.email")?.Value ?? "user@ideia.local";

                    var signature = new Signature(name, email, DateTimeOffset.Now);
                    repo.Commit(msg, signature, signature);
                    CommitMsgTB.Text = CommitPlaceholder;
                    SetStatus($"✅ Commit creado: \"{msg}\"");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Commit falló: {ex.Message}");
            }
        }

        private void DoPush(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_repoPath)) { SetStatus("❌ No hay repo cargado."); return; }

            SetStatus("⏳ Haciendo push...");
            var (success, output) = RunGit("push");
            SetStatus(success ? $"✅ Push exitoso: {output}" : $"❌ Push falló: {output}");
        }

        private void CheckoutBranch(object sender, RoutedEventArgs e)
        {
            if (!Repository.IsValid(_repoPath)) { SetStatus("❌ No hay repo cargado."); return; }

            string selected = BranchCombo.SelectedItem as string;
            if (string.IsNullOrEmpty(selected)) return;

            try
            {
                using (var repo = new Repository(_repoPath))
                {
                    if (repo.Head.FriendlyName == selected)
                    {
                        SetStatus($"⚠ Ya estás en la rama '{selected}'.");
                        return;
                    }

                    var branch = repo.Branches[selected];
                    Commands.Checkout(repo, branch);
                    RefreshBranchInfo();
                    SetStatus($"✅ Cambiado a rama '{selected}'.");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Checkout falló: {ex.Message}");
            }
        }

        private void DoMerge(object sender, RoutedEventArgs e)
        {
            if (!Repository.IsValid(_repoPath)) { SetStatus("❌ No hay repo cargado."); return; }

            string sourceBranch = MergeBranchCombo.SelectedItem as string;
            if (string.IsNullOrEmpty(sourceBranch)) { SetStatus("❌ Selecciona una rama para mergear."); return; }

            try
            {
                using (var repo = new Repository(_repoPath))
                {
                    if (repo.Head.FriendlyName == sourceBranch)
                    {
                        SetStatus("⚠ No puedes mergear una rama consigo misma.");
                        return;
                    }

                    string name  = repo.Config.Get<string>("user.name")?.Value  ?? "IDEIA User";
                    string email = repo.Config.Get<string>("user.email")?.Value ?? "user@ideia.local";

                    var branch    = repo.Branches[sourceBranch];
                    var signature = new Signature(name, email, DateTimeOffset.Now);
                    var result    = repo.Merge(branch, signature, new MergeOptions());

                    switch (result.Status)
                    {
                        case MergeStatus.FastForward:
                            SetStatus($"✅ Merge fast-forward desde '{sourceBranch}'.");
                            break;
                        case MergeStatus.NonFastForward:
                            SetStatus($"✅ Merge commit creado desde '{sourceBranch}'.");
                            break;
                        case MergeStatus.UpToDate:
                            SetStatus($"⚠ Ya estás al día con '{sourceBranch}'.");
                            break;
                        case MergeStatus.Conflicts:
                            SetStatus($"⚠ Merge con conflictos desde '{sourceBranch}'. Resuélvelos manualmente.");
                            break;
                    }

                    RefreshBranchInfo();
                }
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Merge falló: {ex.Message}");
            }
        }

        // ─── Placeholder del TextBox ─────────────────────────────────────────────

        private void CommitMsg_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CommitMsgTB.Text == CommitPlaceholder)
                CommitMsgTB.Text = "";
        }

        private void CommitMsg_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommitMsgTB.Text))
                CommitMsgTB.Text = CommitPlaceholder;
        }

        private void BranchCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Solo reacciona al click del usuario, no al cargar items
        }
    }
}
