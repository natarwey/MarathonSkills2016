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
    /// Логика взаимодействия для ResultatBegunaMarafon.xaml
    /// </summary>
    public partial class ResultatBegunaMarafon : Page
    {
        public ResultatBegunaMarafon()
        {
            InitializeComponent();
            LoadRunnerResults();
        }
        private void LoadRunnerResults()
        {
            try
            {
                // Получаем текущего бегуна
                var runner = ConnectionString.connection.Runner
                    .FirstOrDefault(r => r.Email == CurrentUser.Email);

                if (runner == null) return;

                // Устанавливаем пол
                GenderText.Text = runner.Gender == "Male" ? "мужской" : "женский";

                // Рассчитываем возрастную категорию
                if (runner.DateOfBirth.HasValue)
                {
                    int age = CalculateAge(runner.DateOfBirth.Value);
                    AgeCategoryText.Text = GetAgeCategory(age);
                }

                // Получаем результаты бегуна
                var results = GetRunnerResults(runner.RunnerId);

                if (results.Any())
                {
                    ResultsGrid.ItemsSource = results;
                    NoResultsText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ResultsGrid.Visibility = Visibility.Collapsed;
                    NoResultsText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке результатов: {ex.Message}");
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.Month < birthDate.Month ||
                (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

        private string GetAgeCategory(int age)
        {
            if (age < 18) return "до 18";
            if (age < 30) return "от 18 до 29";
            if (age < 40) return "от 30 до 39";
            if (age < 56) return "от 40 до 55";
            if (age < 71) return "от 56 до 70";
            return "более 70";
        }

        private List<RunnerResult> GetRunnerResults(int runnerId)
        {
            var results = new List<RunnerResult>();

            // Получаем все регистрации бегуна с событиями
            var registrations = ConnectionString.connection.Registration
                .Where(r => r.RunnerId == runnerId)
                .SelectMany(r => r.RegistrationEvent)
                .Where(re => re.RaceTime.HasValue)
                .ToList();

            foreach (var regEvent in registrations)
            {
                // Получаем общее место
                int overallPlace = CalculatePlace(
                    regEvent.Event.RegistrationEvent
                        .Where(re => re.RaceTime.HasValue)
                        .OrderBy(re => re.RaceTime)
                        .ToList(),
                    regEvent.RaceTime.Value);

                // Получаем место в категории
                int categoryPlace = CalculateCategoryPlace(regEvent);

                results.Add(new RunnerResult
                {
                    MarathonName = regEvent.Event.Marathon.MarathonName,
                    EventName = regEvent.Event.EventName,
                    FormattedTime = FormatTime(regEvent.RaceTime.Value),
                    OverallPlace = "#" + overallPlace,
                    CategoryPlace = "#" + categoryPlace
                });
            }

            return results;
        }

        private int CalculateCategoryPlace(RegistrationEvent regEvent)
        {
            var runner = regEvent.Registration.Runner;
            if (!runner.DateOfBirth.HasValue) return 0;

            int age = CalculateAge(runner.DateOfBirth.Value);
            string ageCategory = GetAgeCategory(age);

            var categoryRunners = ConnectionString.connection.RegistrationEvent
                .Where(re => re.EventId == regEvent.EventId &&
                            re.RaceTime.HasValue &&
                            re.Registration.Runner.Gender == runner.Gender)
                .ToList()
                .Where(re =>
                {
                    if (!re.Registration.Runner.DateOfBirth.HasValue) return false;
                    int runnerAge = CalculateAge(re.Registration.Runner.DateOfBirth.Value);
                    return GetAgeCategory(runnerAge) == ageCategory;
                })
                .OrderBy(re => re.RaceTime)
                .ToList();

            return CalculatePlace(categoryRunners, regEvent.RaceTime.Value);
        }

        private int CalculatePlace(List<RegistrationEvent> results, int raceTime)
        {
            int place = 1;
            int previousTime = -1;
            int sameTimeCount = 0;

            foreach (var result in results)
            {
                if (!result.RaceTime.HasValue) continue;

                if (result.RaceTime == previousTime)
                {
                    sameTimeCount++;
                }
                else if (result.RaceTime > previousTime && previousTime != -1)
                {
                    place += sameTimeCount;
                    sameTimeCount = 0;
                }

                if (result.RaceTime == raceTime)
                {
                    return place;
                }

                previousTime = result.RaceTime.Value;
            }

            return 0;
        }

        private string FormatTime(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return $"{time.Hours}h {time.Minutes}m {time.Seconds}s";
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void ShowAllResultsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PastRaceResultsPage());
        }
    }

    public class RunnerResult
    {
        public string MarathonName { get; set; }
        public string EventName { get; set; }
        public string FormattedTime { get; set; }
        public string OverallPlace { get; set; }
        public string CategoryPlace { get; set; }
    }
}