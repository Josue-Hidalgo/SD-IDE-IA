using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        dynamic courses = new {};
        public CoursePage()
        {
            InitializeComponent();
            getData();
        }


        public async void Enroll(object sender, EventArgs e)
        {
            if (courseCodeTB.Text == "") return;

            var client = new HttpClient();
            Singleton user = Singleton.Instance;
            var data = new
            {
                course_code = courseCodeTB.Text,
                id_stud = user.id
            };
            string url = "http://138.2.239.69/student_controller.php";
            string jsonText = JsonConvert.SerializeObject(data);
            var encodeText = new StringContent(jsonText, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await client.PostAsync(url, encodeText);

            if (respuesta.IsSuccessStatusCode)
            {
                MessageBox.Show("successfully enrolled.", "Enrolled!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Console.WriteLine(respuesta.ReasonPhrase,respuesta.StatusCode);
                MessageBox.Show("Invalid course code.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            getData();
        }

        public void CourseChange(object sender, SelectionChangedEventArgs e)
        {
            if (CourseList.SelectedItem == null) return;

            string[] code = CourseList.SelectedItem.ToString().Split(' ');

            NavigationService.Navigate(new AsignmentsPage(code[0], code[1]));
            NavigationService.RemoveBackEntry();
        }

        public async void getData()
        {
            var client = new HttpClient();
            Singleton user = Singleton.Instance;
            string url = "http://138.2.239.69/student_controller.php?" + "id_stud=" + user.id;
            string response = await client.GetStringAsync(url);
            CourseList.Items.Clear();

            if (!response.Contains("false"))
            {
                courses = JsonConvert.DeserializeObject(response);
                foreach (var course in courses)
                {
                    dynamic c = JsonConvert.DeserializeObject(response);
                    CourseList.Items.Add(c[0]["code_course"] +" "+c[0]["name_course"]);
                }
            }
        }
    }
}