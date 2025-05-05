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
using MarathonSkills2016.Database;

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для SpisokBlagotvorOrg.xaml
    /// </summary>
    public partial class SpisokBlagotvorOrg : Page
    {
        public SpisokBlagotvorOrg()
        {
            InitializeComponent();
            LvCharity.ItemsSource = ConnectionString.connection.Charity.ToList();
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        public void LoadPhoto()
        {

        }
    }
}
