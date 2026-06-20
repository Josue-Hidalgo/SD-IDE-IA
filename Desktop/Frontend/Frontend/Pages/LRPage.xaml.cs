using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para LRPage.xaml
    /// </summary>
    public partial class LRPage : Page
    {
        public LRPage()
        {
            InitializeComponent();
        }

        public void showRegPage(object sender, EventArgs e)
        {
            NavigationService.Navigate(new RegPage());
            NavigationService.RemoveBackEntry();
        }

        public void Login(object sender, EventArgs e)
        {
            bool valid = true;
            if (EmailTB.Text == "") valid = false;
            if (PasswordTB.Password == "") valid = false;

            try
            {
                var addr = new MailAddress(EmailTB.Text);
                if (valid) { getData(); }
                else
                {
                    MessageBox.Show("All fields must be filled out to login.", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("Wrong email format.", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void getData()
        {
            string response = "";
            var client = new HttpClient();
            string url = "http://138.2.239.69/api.php?"+"action=log_student&email="+EmailTB.Text+"&password="+PasswordTB.Password;
            //string url = "http://localhost:8080/api.php?"+"action=log_student&email="+EmailTB.Text+"&password="+PasswordTB.Password;
            try
            {
                response = await client.GetStringAsync(url);
            }
            catch(Exception e)
            {
                MessageBox.Show("The server is not responding."+e.Message, "Server did not respond", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (response.Contains("false"))
            {
                MessageBox.Show("wrong credentials.", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                dynamic json = JsonConvert.DeserializeObject(response);
                Singleton user = Singleton.Instance;
                user.id = json["stud_id"];
                user.name = json["name"];
                user.Lname = json["lastname"];
                user.email = json["email"];
                NavigationService.Navigate(new CoursePage());
                NavigationService.RemoveBackEntry();
            }
        }

        public void RememberPassword(object sender, EventArgs e)
        {
            NavigationService.Navigate(new RememberPassword());
            NavigationService.RemoveBackEntry();
        }
    }
}
