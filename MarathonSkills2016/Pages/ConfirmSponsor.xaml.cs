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
    /// Логика взаимодействия для ConfirmSponsor.xaml
    /// </summary>
    public partial class ConfirmSponsor : Page
    {
        Sponsorship sp;
        Charity fund;
        public ConfirmSponsor(Sponsorship spData)
        {
            InitializeComponent();
            sp = spData;
            Registration begun = sp.Registration;
            User beg = begun.Runner.User;
            Country BegunCountry = begun.Runner.Country;
            RunnerTxt.Text = $"{beg.FirstName} {beg.LastName}({begun.RegistrationId}) из {BegunCountry.CountryName}";
            MoneyTxt.Text = $"${sp.Amount}";
            fund = sp.Registration.Charity;
            FundTxt.Text = fund.CharityName;
        }


        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}
