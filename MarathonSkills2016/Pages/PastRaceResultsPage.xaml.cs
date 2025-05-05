using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MarathonSkills2016.Database;

namespace MarathonSkills2016.Pages
{
    public partial class PastRaceResultsPage : Page
    {
        public PastRaceResultsPage()
        {
            InitializeComponent();
            LoadFilters();
            LoadResults();
        }
        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем выбранные значения фильтров
            MarathonComboBox.SelectedIndex = -1;
            EventTypeComboBox.SelectedIndex = -1;
            GenderComboBox.SelectedIndex = 0; // "Любой"
            AgeCategoryComboBox.SelectedIndex = 0; // "Любая"

            // Обновляем результаты с очищенными фильтрами
            LoadResults();
        }
        private void LoadFilters()
        {
            try
            {
                // Загрузка марафонов
                MarathonComboBox.ItemsSource = ConnectionString.connection.Marathon.ToList();
                MarathonComboBox.SelectedIndex = -1;

                // Загрузка типов событий
                EventTypeComboBox.ItemsSource = ConnectionString.connection.EventType.ToList();
                EventTypeComboBox.SelectedIndex = -1;

                // Устанавливаем "Любой" для пола и возраста
                GenderComboBox.SelectedIndex = 0;
                AgeCategoryComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}");
            }
        }

        private void LoadResults()
        {
            try
            {
                // Получаем выбранные фильтры
                int? marathonId = MarathonComboBox.SelectedValue as int?;
                string eventTypeId = EventTypeComboBox.SelectedValue as string;
                string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                string ageCategory = (AgeCategoryComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();

                // Получаем результаты
                var results = GetFilteredResults(marathonId, eventTypeId, gender, ageCategory);

                // Обновляем статистику
                UpdateStatistics(results);

                // Отображаем результаты
                ResultsDataGrid.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке результатов: {ex.Message}");
            }
        }

        private List<RaceResult> GetFilteredResults(int? marathonId, string eventTypeId, string gender, string ageCategory)
        {
            // Сначала получаем все результаты без фильтрации по возрасту
            var query = ConnectionString.connection.RegistrationEvent
                .Where(re => re.RaceTime.HasValue)
                .AsQueryable();

            // Фильтр по марафону
            if (marathonId.HasValue)
            {
                query = query.Where(re => re.Event.MarathonId == marathonId.Value);
            }

            // Фильтр по типу события
            if (!string.IsNullOrEmpty(eventTypeId))
            {
                query = query.Where(re => re.Event.EventTypeId == eventTypeId);
            }

            // Фильтр по полу
            if (gender != "Any")
            {
                query = query.Where(re => re.Registration.Runner.Gender == gender);
            }

            // Получаем все результаты после первичной фильтрации
            var filteredResults = query.ToList();

            // Применяем фильтр по возрасту в памяти, если нужно
            if (ageCategory != "Any")
            {
                filteredResults = filteredResults.Where(re =>
                {
                    if (!re.Registration.Runner.DateOfBirth.HasValue) return false;

                    var birthDate = re.Registration.Runner.DateOfBirth.Value;
                    var today = DateTime.Today;
                    var age = today.Year - birthDate.Year;

                    if (birthDate.Date > today.AddYears(-age))
                        age--;

                    switch (ageCategory)
                    {
                        case "Under18": return age < 18;
                        case "18-29": return age >= 18 && age <= 29;
                        case "30-39": return age >= 30 && age <= 39;
                        case "40-55": return age >= 40 && age <= 55;
                        case "56-70": return age >= 56 && age <= 70;
                        case "Over70": return age > 70;
                        default: return false;
                    }
                }).ToList();
            }

            // Сортируем результаты по времени
            var sortedResults = filteredResults
                .OrderBy(re => re.RaceTime)
                .ToList();

            // Формируем список результатов с местами
            var resultsWithPlaces = new List<RaceResult>();
            int place = 1;
            int? previousTime = null;
            int sameTimeCount = 0;

            foreach (var result in sortedResults)
            {
                if (previousTime.HasValue && result.RaceTime != previousTime)
                {
                    place += sameTimeCount;
                    sameTimeCount = 0;
                }

                resultsWithPlaces.Add(new RaceResult
                {
                    Place = place,
                    TimeInSeconds = result.RaceTime.Value,
                    RunnerName = $"{result.Registration.Runner.User.FirstName} {result.Registration.Runner.User.LastName}",
                    Country = result.Registration.Runner.Country.CountryCode
                });

                if (result.RaceTime == previousTime)
                {
                    sameTimeCount++;
                }
                else
                {
                    previousTime = result.RaceTime;
                }
            }

            return resultsWithPlaces;
        }

        private void UpdateStatistics(List<RaceResult> results)
        {
            TotalRunnersText.Text = $"Всего бегунов: {results.Count}";

            int finishedCount = results.Count;
            FinishedRunnersText.Text = $"Всего финишировало: {finishedCount}";

            if (finishedCount > 0)
            {
                double averageSeconds = results.Average(r => r.TimeInSeconds);
                TimeSpan averageTime = TimeSpan.FromSeconds(averageSeconds);
                AverageTimeText.Text = $"Среднее время: {averageTime.Hours}h {averageTime.Minutes}m {averageTime.Seconds}s";
            }
            else
            {
                AverageTimeText.Text = "Среднее время: N/A";
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadResults();
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

    public class RaceResult
    {
        public int Place { get; set; }
        public int TimeInSeconds { get; set; }
        public string RunnerName { get; set; }
        public string Country { get; set; }

        public string FormattedTime =>
            $"{TimeSpan.FromSeconds(TimeInSeconds).Hours}h " +
            $"{TimeSpan.FromSeconds(TimeInSeconds).Minutes}m " +
            $"{TimeSpan.FromSeconds(TimeInSeconds).Seconds}s";
    }
}