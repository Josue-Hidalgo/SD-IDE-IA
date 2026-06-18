using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Newtonsoft.Json;

namespace Frontend.Pages
{
    /// <summary>
    /// Lógica de interacción para RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        public void Register(object sender, EventArgs e)
        {
            bool valid = true;
            if(NameTB.Text == "") valid = false;
            if(LNameTB.Text == "") valid = false;
            if(EmailTB.Text == "") valid = false;
            if(PasswordTB.Password == "") valid = false;

            if (valid){SendData();}
            else
            {
                MessageBox.Show("All fields must be filled out to register.", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void backToLogin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new LRPage());
            NavigationService.RemoveBackEntry();
        }

        public async void SendData()
        {
            var client = new HttpClient();
            var data = new
            {
                action = "create_student",
                email = EmailTB.Text,
                password = PasswordTB.Password,
                username = NameTB.Text,
                userLast = LNameTB.Text
            };
            //string url = "http://138.2.239.69/api.php";
            string url = "http://localhost:8080/api.php";
            string jsonText = JsonConvert.SerializeObject(data);
            var encodeText = new StringContent(jsonText, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await client.PostAsync(url,encodeText);

            if (respuesta.IsSuccessStatusCode)
            {
                MessageBox.Show("successfully registered.", "Registered!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new LRPage());
                NavigationService.RemoveBackEntry();
            }
            else
            {
                MessageBox.Show("The user already exists.", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
