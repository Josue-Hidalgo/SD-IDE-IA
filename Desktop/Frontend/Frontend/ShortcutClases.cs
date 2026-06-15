using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frontend
{
    /// Clases para los shortcuts
    /// Clase base
    public class Shortcut : ICommand
    {
        protected MainWindow win = null;
        public Shortcut(MainWindow w)
        {
            win = w;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
        }
    }
    /// Clases herederas
    public class NewFileKey : Shortcut
    {
        public NewFileKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.OpenFile(null, null);
        }
    }

    public class OpenFileKey : Shortcut
    {
        public OpenFileKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.OpenFile(null, null);
        }
    }

    public class OpenDirectoryKey : Shortcut
    {
        public OpenDirectoryKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.OpenDirectory(null, null);
        }
    }

    public class SaveKey : Shortcut
    {
        public SaveKey(MainWindow w) : base(w) { }


        public override void Execute(object parameter)
        {
            win.Save(null, null);
        }
    }

    public class SaveFileAsKey : Shortcut
    {
        public SaveFileAsKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.SaveFileAs(null, null);
        }
    }

    public class CloseFileKey : Shortcut
    {
        public CloseFileKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.CloseFile(null, null);
        }
    }

    public class CloseDirectoryKey : Shortcut
    {
        public CloseDirectoryKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.CloseFolder(null, null);
        }
    }

    public class RunKey : Shortcut
    {
        public RunKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.RunCode(null, null);
        }
    }

    public class OpenGitKey : Shortcut
    {
        public OpenGitKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.OpenGitW(null, null);
        }
    }

    public class OpenITerminalKey : Shortcut
    {
        public OpenITerminalKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.OITerminal(null, null);
        }
    }

    public class KillITerminalKey : Shortcut
    {
        public KillITerminalKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.KCTerminal(null, null);
        }
    }

    public class OpenAcademicAKey : Shortcut
    {
        public OpenAcademicAKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.AcademicBTN(null, null);
        }
    }

    public class HideAcademicAKey : Shortcut
    {
        public HideAcademicAKey(MainWindow w) : base(w) { }

        public override void Execute(object parameter)
        {
            win.CloseAcademicArea(null, null);
        }
    }

    public class ShortcutsCommandContext
    {
        private MainWindow win = null;

        public ICommand newFileCommand { get; }
        public ICommand openFileCommand { get; }
        public ICommand openDirectoryCommand { get; }
        public ICommand saveCommand { get; }
        public ICommand saveFileAsCommand { get; }
        public ICommand closeFileCommand { get; }
        public ICommand closeDirectoryCommand { get; }
        public ICommand runCommand { get; }
        public ICommand openRepositoryCommand { get; }
        public ICommand openIntegratedTerminalCommand { get; }
        public ICommand killTerminalCommand { get; }
        public ICommand openAcademicAreaCommand { get; }
        public ICommand hideAcademicAreaCommand { get; }

        public ShortcutsCommandContext(MainWindow w)
        {
            win = w;
            this.newFileCommand = new NewFileKey(w);
            this.openFileCommand = new OpenFileKey(w);
            this.openDirectoryCommand = new OpenDirectoryKey(w);
            this.saveCommand = new SaveKey(w);
            this.saveFileAsCommand = new SaveFileAsKey(w);
            this.closeFileCommand = new CloseFileKey(w);
            this.closeDirectoryCommand = new CloseDirectoryKey(w);
            this.runCommand = new RunKey(w);
            this.openRepositoryCommand = new OpenGitKey(w);
            this.openIntegratedTerminalCommand = new OpenITerminalKey(w);
            this.killTerminalCommand = new KillITerminalKey(w);
            this.openAcademicAreaCommand = new OpenAcademicAKey(w);
            this.hideAcademicAreaCommand = new HideAcademicAKey(w);
        }
    }
}
