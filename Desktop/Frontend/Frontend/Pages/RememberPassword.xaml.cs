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
                    //string url = "http://138.2.239.69/api.php?" + "action=remember_pass" + "&email=" + EmailTB.Text;
                    string url = "http://localhost:8080/api.php?" + "action=remember_pass" + "&email=" + EmailTB.Text;
                    try
                    {
                        response = await client.GetStringAsync(url);
                        if (response.Contains("true"))
                        {
                            MessageBox.Show("Password sent to your mail.", "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Email could not be sent.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("The server is not responding.", "Server did not respond", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
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
