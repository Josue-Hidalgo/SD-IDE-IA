using MimeKit;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para RememberPassword.xaml
    /// </summary>
    public partial class RememberPassword : Page
    {
        public RememberPassword()
        {
            InitializeComponent();
        }

        public void BackLogin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new LRPage());
            NavigationService.RemoveBackEntry();
        }

        public async void RememberPasswordF(object sender,EventArgs e)
        {
            bool valid = true;
            if (EmailTB.Text == "") valid = false;

            try
            {
                if (valid) {
                    string response = "";
                    var client = new HttpClient();
                    string url = "http://138.2.239.69/index.php?" + "action=remember_pass" + "&email=" + EmailTB.Text;
                    try
                    {
                        response = await client.GetStringAsync(url);
                        MessageBox.Show("Password sent to your mail.", "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        MessageBox.Show("The server is not responding.", "Server did not respond", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (response.Contains("false"))
                    {
                        MessageBox.Show("Email could not be sent.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
