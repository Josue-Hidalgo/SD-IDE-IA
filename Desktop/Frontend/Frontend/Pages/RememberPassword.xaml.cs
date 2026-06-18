using MimeKit;
using System;
using System.Net;
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
        string email = "ideia12026@gmail.com";
        string emailName = "IDEIA";
        string emailPassword = "nxkl gbcb mgci awbe";
        public RememberPassword()
        {
            InitializeComponent();
        }

        public void BackLogin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new LRPage());
            NavigationService.RemoveBackEntry();
        }

        public void RememberPasswordF(object sender,EventArgs e)
        {
            string Subject = "Password from IDEIA";
            string Body = "Your password is: smthng";

            bool valid = true;
            if (EmailTB.Text == "") valid = false;

            try
            {
                string addresToName = "Name";
                if (valid) { 
                    var addresFrom = new MailAddress(email, emailName);
                    var addresTo = new MailAddress(EmailTB.Text,addresToName);
                    var Email = new MailMessage(addresFrom,addresTo);
                    Email.Subject = Subject;
                    Email.Body = Body;
                    SmtpClient Client = new SmtpClient("smtp.gmail.com");
                    Client.Port = 587;
                    Client.EnableSsl = true;
                    Client.UseDefaultCredentials = false;
                    Client.Credentials = new NetworkCredential(email, emailPassword);
                    try
                    {
                        Client.Send(Email);
                        MessageBox.Show("Password sent succesfully.", "Password sent", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        MessageBox.Show("something went wrong when sending the email.", "Something went wrong :o", MessageBoxButton.OK, MessageBoxImage.Information);
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
