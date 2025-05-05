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

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для PodtverditRegMarafon.xaml
    /// </summary>
    public partial class PodtverditRegMarafon : Page
    {
        public PodtverditRegMarafon()
        {
            InitializeComponent();
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuBeguna());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuBeguna());
        }
    }
}