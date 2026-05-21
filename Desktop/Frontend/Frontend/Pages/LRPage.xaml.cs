using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
