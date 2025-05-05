using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
using MarathonSkills2016.Windows;

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuBeguna.xaml
    /// </summary>
    public partial class MenuBeguna : Page
    {
        public MenuBeguna()
        {
            InitializeComponent();
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationMarafon());
        }

        private void MyResultsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ResultatBegunaMarafon());
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RedaktProfil());
        }

        private void MySponsorButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MySponsorsPage());
        }

        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            Contacts contactsWindow = new Contacts();
            contactsWindow.Show();
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
