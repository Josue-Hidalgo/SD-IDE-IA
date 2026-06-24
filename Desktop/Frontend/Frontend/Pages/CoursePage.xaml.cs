using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        dynamic courses = new {};
        Dictionary<string, JObject> courseData = new Dictionary<string, JObject>();
        public CoursePage()
        {
            InitializeComponent();
            getData();
        }

        public void LogOut(object sender, EventArgs e)
        {
            Singleton user = Singleton.Instance;
            user.id = "";
            user.name = "";
            user.Lname = "";
            user.email = "";
            NavigationService.Navigate(new LRPage());
            NavigationService.RemoveBackEntry();
        }

        public async void Enroll(object sender, EventArgs e)
        {
            if (courseCodeTB.Text == "") return;

            var client = new HttpClient();
            Singleton user = Singleton.Instance;
            var data = new
            {
                action = "enroll_student",
                course_code = courseCodeTB.Text,
                id_stud = user.id
            };
            string url = "http://138.2.239.69/api.php";
            string jsonText = JsonConvert.SerializeObject(data);
            var encodeText = new StringContent(jsonText, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await client.PostAsync(url, encodeText);

            if (respuesta.IsSuccessStatusCode)
            {
                MessageBox.Show("successfully enrolled.", "Enrolled!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Invalid course code.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            getData();
        }

        public void CourseChange(object sender, SelectionChangedEventArgs e)
        {
            if (CourseList.SelectedItem == null) return;

            JObject course = courseData[CourseList.SelectedItem.ToString()];

            string[] code = { (string)course["code_course"], (string)course["name_course"], (string)course["description_course"] };

            NavigationService.Navigate(new AsignmentsPage(code));
            NavigationService.RemoveBackEntry();
        }

        public async void getData()
        {
            var client = new HttpClient();
            Singleton user = Singleton.Instance;
            string url = "http://138.2.239.69/api.php?" + "action=get_enroll_courses&id_stud=" + user.id;
            string response = await client.GetStringAsync(url);
            CourseList.Items.Clear();
            courseData.Clear();

            if (!response.Contains("false"))
            {
                courses = JsonConvert.DeserializeObject(response);
                foreach (var course in courses)
                {
                    Console.Write(course);
                    string data = course["code_course"] + "&" + course["name_course"];
                    CourseList.Items.Add(data);
                    courseData[data] = course;
                }
            }
        }
    }
}