using LibGit2Sharp;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
            { afterDeadline.Text = "Allowed after deadline: ✕"; } 
            else 
            { afterDeadline.Text = "Allowed after deadline: ✓"; }
            
            this.Course = Course;
            getAssGrade();
        }
        
        public async void getAssGrade()
        {
            Singleton user = Singleton.Instance;

            var client = new HttpClient();
            string url = "http://138.2.239.69/api.php?" + "action=get_grade&id_stud=" + user.id+"&id_assign="+assignmentID;
            string response = await client.GetStringAsync(url);
            response = response.Trim();

            Grade.Text = "-/100";

            if (response != "0")
            {
                dynamic json = JsonConvert.DeserializeObject(response);
                if (json["grade"] != null) Grade.Text = json["grade"] + "/100";
                Submission.Text = "Submission:\n" + json["name"];
            }
        }

        public void BackAssignments(object sender,EventArgs e)
        {
            NavigationService.Navigate(new AsignmentsPage(Course));
            NavigationService.RemoveBackEntry();
        }

        public async void UploadFile(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "All Files (*.*)|*.*";
            OFD.FilterIndex = 0;
            if (OFD.ShowDialog() == true)
            {
                string route = OFD.FileName;
                TextReader reader = new StreamReader(route);
                string content = (reader.ReadToEnd()).Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");

                byte[] FileBytes = Encoding.UTF8.GetBytes(content);
                string blob = Convert.ToBase64String(FileBytes);

                if (!System.IO.Path.GetFileName(OFD.FileName).Contains(".py"))
                {
                    MessageBox.Show("Your file should be a python file.", "Failed!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Singleton user = Singleton.Instance;

                var client = new HttpClient();
                var data = new
                {
                    action = "create_submission",
                    id_stud = user.id,
                    id_assign = assignmentID,
                    project_name = System.IO.Path.GetFileName(OFD.FileName),
                    project_data = blob
                };
                string url = "http://138.2.239.69/api.php";
                string jsonText = JsonConvert.SerializeObject(data);
                var encodeText = new StringContent(jsonText, Encoding.UTF8, "application/json");
                HttpResponseMessage respuesta = await client.PostAsync(url, encodeText);

                if (respuesta.IsSuccessStatusCode)
                {
                    MessageBox.Show("You submited your file succesfully.", "SUBMITED!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Submission failed.", "Failed :(", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                reader.Close();
            }
        }

        private void AssDropFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private async void AssDropFile_Drop(object sender, DragEventArgs e)
        {
            string[] filesDropped = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if(filesDropped.Length > 1)
            {
                MessageBox.Show("You can only submit 1 file per assignment.", "More than one file", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (string f in filesDropped)
            {
                if (!f.Contains(".py"))
                {
                    MessageBox.Show("Your file should be a python file.", "Failed!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                TextReader reader = new StreamReader(f);
                string content = (reader.ReadToEnd()).Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
                
                byte[] FileBytes = Encoding.UTF8.GetBytes(content);
                string blob = Convert.ToBase64String(FileBytes);

                Singleton user = Singleton.Instance;

                var client = new HttpClient();
                var data = new
                {
                    action = "create_submission",
                    id_stud = user.id,
                    id_assign = assignmentID,
                    project_name = System.IO.Path.GetFileName(f),
                    project_data = blob
                };
                string url = "http://138.2.239.69/api.php";
                string jsonText = JsonConvert.SerializeObject(data);
                var encodeText = new StringContent(jsonText, Encoding.UTF8, "application/json");
                HttpResponseMessage respuesta = await client.PostAsync(url, encodeText);

                if (respuesta.IsSuccessStatusCode)
                {
                    MessageBox.Show("You submited your file succesfully.", "SUBMITED!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Submission failed.", "Failed :(", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                reader.Close();
            }
        }
    }
}
