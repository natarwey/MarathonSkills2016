using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MarathonSkills2016.Database;

using System.Data.Entity;
namespace MarathonSkills2016.Pages
{
    public partial class RunnerDetailsPage : Page
    {
        private int _runnerId;
        public RunnerDetailsPage(int runnerId)
        {
            _runnerId = runnerId;
            InitializeComponent();
            LoadRunnerDetails();
        }

        private void LoadRunnerDetails()
        {
            try
            {
                var runner = ConnectionString.connection.Runner
                    .Include(r => r.User)
                    .Include(r => r.Country)
                    .Include(r => r.Registration.Select(reg => reg.RegistrationStatus))
                    .Include(r => r.Registration.Select(reg => reg.Charity))
                    .Include(r => r.Registration.Select(reg => reg.RaceKitOption))
                    .Include(r => r.Registration.Select(reg => reg.RegistrationEvent.Select(re => re.Event.EventType)))
                    .FirstOrDefault(r => r.RunnerId == _runnerId);

                if (runner != null)
                {
                    // Основная информация
                    EmailText.Text = runner.Email;
                    FirstNameText.Text = runner.User?.FirstName ?? "не указано";
                    LastNameText.Text = runner.User?.LastName ?? "не указано";
                    GenderText.Text = runner.Gender == "M" ? "мужской" : "женский";
                    BirthDateText.Text = runner.DateOfBirth?.ToString("dd MMMM yyyy") ?? "не указано";
                    CountryText.Text = runner.Country?.CountryName ?? "не указано";

                    // Информация о благотворительности
                    var registration = runner.Registration.FirstOrDefault();
                    CharityText.Text = registration?.Charity?.CharityName ?? "не указано";
                    DonationText.Text = registration?.SponsorshipTarget.ToString("C") ?? "$0";

                    // Информация о забеге
                    var registrationEvent = registration?.RegistrationEvent.FirstOrDefault();
                    PackageText.Text = GetPackageName(registration?.RaceKitOptionId);
                    //DistanceText.Text = GetDistanceName(registrationEvent?.Event?.EventType?.EventTypeName);

                    // Статусы
                    var status = registration?.RegistrationStatus?.RegistrationStatus1; // Обратите внимание на RegistrationStatus1
                    SetStatusIcon(RegisteredCheck, status == "Registered");
                    SetStatusIcon(PaymentCheck, status == "Payment Confirmed");
                    SetStatusIcon(KitCheck, status == "Race Kit Sent");
                    SetStatusIcon(StartedCheck, status == "Race Attended");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных бегуна: {ex.Message}");
            }
        }

        private string GetPackageName(string raceKitOptionId)
        {
            if (string.IsNullOrEmpty(raceKitOptionId)) return "не указано";

            var option = ConnectionString.connection.RaceKitOption
                .FirstOrDefault(o => o.RaceKitOptionId == raceKitOptionId);

            return option?.RaceKitOption1 ?? "не указано";
        }

        private string GetDistanceName(string eventType)
        {
            if (string.IsNullOrEmpty(eventType)) return "не указано";

            if (eventType.Contains("Full")) return "42km полный марафон";
            if (eventType.Contains("Half")) return "21km полумарафон";
            if (eventType.Contains("Mini")) return "5km мини-марафон";
            return "не указано";
        }

        private void SetStatusIcon(CheckBox checkBox, bool isChecked)
        {
            checkBox.IsChecked = isChecked;
            checkBox.Content = isChecked ? "✓" : "✗";
            checkBox.Foreground = isChecked ? Brushes.Green : Brushes.Red;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}