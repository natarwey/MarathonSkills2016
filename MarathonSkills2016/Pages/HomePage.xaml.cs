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
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }
        private void RunnerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProverkaUhastiaBegunov());
        }

        private void SponsorButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RunnerSponsor());
        }

        private void MoreInfoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Information());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }
    }
}
