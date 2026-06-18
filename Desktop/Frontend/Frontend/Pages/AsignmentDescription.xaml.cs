using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Navigation;
using Newtonsoft.Json.Linq;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para AsignmentDescription.xaml
    /// </summary>
    public partial class AsignmentDescription : Page
    {

        string assignmentID = "";
        string assignmentName = "";
        string assignmentDesc = "";
        string assignmentDeadline = "";
        string[] files = { };
        string[] Course = { };
        string[] Assignment;
        public AsignmentDescription(string[] Course, string[] Ass)
        {
            InitializeComponent();

            Assignment = Ass;

            assignmentID = Ass[0];
            assignmentName = Ass[1];
            assignmentDesc = Ass[2];
            assignmentDeadline = Ass[3];

            AssName.Text = assignmentName;
            AssDescription.Text = assignmentDesc;
            AssDeadline.Text = assignmentDeadline;

            if (Ass[4] == "0")
            { afterDeadline.Text = "Allowed after deadline: ✓"; } 
            else 
            { afterDeadline.Text = "Allowed after deadline: ✕"; }
            
            this.Course = Course;
        }
        
        public void BackAssignments(object sender,EventArgs e)
        {
            NavigationService.Navigate(new AsignmentsPage(Course));
            NavigationService.RemoveBackEntry();
        }

        public void UploadFile(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "All Files (*.*)|*.*";
            OFD.FilterIndex = 0;
            if (OFD.ShowDialog() == true)
            {
                string route = OFD.FileName;
                TextReader reader = new StreamReader(route);
                string content = (reader.ReadToEnd()).Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
                Console.WriteLine(content);
                reader.Close();
            }
        }

        public void FileChange(object sender, SelectionChangedEventArgs e)
        {
            if (FileList.SelectedItem == null) return;

        }

        private void AssDropFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void AssDropFile_Drop(object sender, DragEventArgs e)
        {
            string[] filesDropped = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (string f in filesDropped)
            {
                TextReader reader = new StreamReader(f);
                string content = (reader.ReadToEnd()).Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
                Console.WriteLine(content);
                reader.Close();
            }
        }
    }
}
