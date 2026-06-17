using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Navigation;

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
        dynamic AsignmentData = new { };
        string[] files = { };
        string[] Course = { };
        string[] Assignment = { };
        public AsignmentDescription(string[] Course, string[] Assignment)
        {
            this.Assignment = Assignment;
            this.Course = Course;
            InitializeComponent();
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
