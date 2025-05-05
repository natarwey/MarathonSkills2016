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
    /// Логика взаимодействия для Information.xaml
    /// </summary>
    public partial class Information : Page
    {
        public Information()
        {
            InitializeComponent();
        }
      private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void MarathonSkillsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MarathonInfoPage());
            //MarathonInfoPage
            //InteractiveMap
        }

        private void HowLongButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InfoMarafon());
        }

        private void LastResultsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PastRaceResultsPage ());
        }

        private void BMIButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BMICalculatorPage());
        }

        private void BMRButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BMRCalculatorPage());
        }

        private void SpisokBlagotvorOrg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SpisokBlagotvorOrg());
        }
    }
}
