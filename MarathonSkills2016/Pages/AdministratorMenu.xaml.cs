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
    /// Логика взаимодействия для AdministratorMenu.xaml
    /// </summary>
    public partial class AdministratorMenu : Page
    {
        public AdministratorMenu()
        {
            InitializeComponent();
        }
        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new User)
        }

        private void VolonteursButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VolunteerManagementPage());
        }

        private void OrgsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CharityManagementPage());
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InventoryPage());
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}
