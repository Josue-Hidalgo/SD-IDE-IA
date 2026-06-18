using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para AsignmentsPage.xaml
    /// </summary>

    public partial class AsignmentsPage : Page
    {
        string courseID = "";
        string courseName = "";
        string courseDesc = "";
        dynamic Asses = new { };
        string[] Course = {};

        public AsignmentsPage(string[] Course)
        {
            this.Course = Course;
            courseID = Course[0];
            courseName = Course[1];
            InitializeComponent();
            CourseNameTB.Text = Course[0]+"\n"+Course[1];
            CourseDescTB.Text = Course[2];
            getData();
        }

        public void backCourses(object sender, EventArgs e)
        {
            NavigationService.Navigate(new CoursePage());
            NavigationService.RemoveBackEntry();
        }

        public void AssChange(object sender, SelectionChangedEventArgs e)
        {
            if (AssList.SelectedItem == null) return;

        }

        public async void getData()
        {
            var client = new HttpClient();
            string url = "http://138.2.239.69/index.php?" + "action=get_assign_by_course&code_course=" + courseID;
            string response = await client.GetStringAsync(url);
            AssList.Items.Clear();

            if (!response.Contains("false"))
            {
                Asses = JsonConvert.DeserializeObject(response);
                foreach (var Ass in Asses)
                {
                    AssList.Items.Add(Ass["name_assignment"]);
                }
            }
        }
    }
}