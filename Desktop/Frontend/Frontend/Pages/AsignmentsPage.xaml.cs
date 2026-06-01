using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para AsignmentsPage.xaml
    /// </summary>

    public partial class AsignmentsPage : Page
    {
        string courseID = "";
        string courseName = "";
        dynamic Asses = new { };

        public AsignmentsPage(string CID,string CName)
        {
            courseID = CID;
            courseName = CName;
            InitializeComponent();
            CourseNameTB.Text = CID+" "+CName;
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
            string url = "http://138.2.239.69/assignment_controller.php?" + "code_course=" + courseID;
            string response = await client.GetStringAsync(url);
            AssList.Items.Clear();

            if (!response.Contains("false"))
            {
                Asses = JsonConvert.DeserializeObject(response);
                foreach (var Ass in Asses)
                {
                    dynamic a = JsonConvert.DeserializeObject(response);
                    AssList.Items.Add(Ass["name_assignment"]);
                }
            }
        }
    }
}