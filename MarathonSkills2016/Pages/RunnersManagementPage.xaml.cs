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
using System.Data.Entity;
using System.IO;

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для RunnersManagementPage.xaml
    /// </summary>
    public partial class RunnersManagementPage : Page
    {
        public RunnersManagementPage()
        {
            InitializeComponent();
            LoadRunnersData();
        }
        private void InitializeComboBoxes()
        {
            try
            {
                // Инициализация комбобокса статусов
                StatusComboBox.Items.Add(new ComboBoxItem { Content = "Все", Tag = "All", IsSelected = true });
                StatusComboBox.Items.Add(new ComboBoxItem { Content = "Registered", Tag = "Registered" });
                StatusComboBox.Items.Add(new ComboBoxItem { Content = "Payment Confirmed", Tag = "Payment Confirmed" });
                StatusComboBox.Items.Add(new ComboBoxItem { Content = "Race Kit Sent", Tag = "Race Kit Sent" });
                StatusComboBox.Items.Add(new ComboBoxItem { Content = "Race Attended", Tag = "Race Attended" });

                // Инициализация комбобокса дистанций
                DistanceComboBox.Items.Add(new ComboBoxItem { Content = "Все", Tag = "All", IsSelected = true });
                DistanceComboBox.Items.Add(new ComboBoxItem { Content = "42km полный марафон", Tag = "Full" });
                DistanceComboBox.Items.Add(new ComboBoxItem { Content = "21km полумарафон", Tag = "Half" });
                DistanceComboBox.Items.Add(new ComboBoxItem { Content = "5km мини-марафон", Tag = "Mini" });

                // Инициализация комбобокса сортировки
                SortComboBox.Items.Add(new ComboBoxItem { Content = "Имя", Tag = "FirstName", IsSelected = true });
                SortComboBox.Items.Add(new ComboBoxItem { Content = "Фамилия", Tag = "LastName" });
                SortComboBox.Items.Add(new ComboBoxItem { Content = "Email", Tag = "Email" });
                SortComboBox.Items.Add(new ComboBoxItem { Content = "Статус", Tag = "Status" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации фильтров: {ex.Message}");
            }
        }
        private void LoadRunnersData()
        {
            try
            {
                // Проверяем инициализацию комбобоксов
                if (StatusComboBox == null || DistanceComboBox == null || SortComboBox == null)
                {
                    MessageBox.Show("Ошибка инициализации фильтров");
                    return;
                }

                // Получаем выбранные фильтры с проверкой на null
                string statusFilter = (StatusComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
                string distanceFilter = (DistanceComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
                string sortField = (SortComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "FirstName";

                // Получаем список бегунов с учетом фильтров
                var runners = GetFilteredRunners(statusFilter, distanceFilter, sortField);

                // Обновляем общее количество
                TotalRunnersText.Text = $"Total runners: {runners.Count}";

                if (runners.Any())
                {
                    RunnersGrid.ItemsSource = runners;
                    NoResultsText.Visibility = Visibility.Collapsed;
                    RunnersGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    RunnersGrid.Visibility = Visibility.Collapsed;
                    NoResultsText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных бегунов: {ex.Message}");
            }
        }

        private List<RunnerInfo> GetFilteredRunners(string statusFilter, string distanceFilter, string sortField)
        {
            var query = ConnectionString.connection.Runner.AsQueryable();

            // Фильтр по статусу
            if (statusFilter != "All")
            {
                query = query.Where(r => r.Registration.Any(reg =>
                    reg.RegistrationStatus.RegistrationStatus1 == statusFilter));
            }

            // Фильтр по дистанции
            if (distanceFilter != "All")
            {
                query = query.Where(r => r.Registration.Any(reg =>
                    reg.RegistrationEvent.Any(re =>
                        re.Event.EventType.EventTypeName.Contains(distanceFilter))));
            }

            // Сортировка
            switch (sortField)
            {
                case "FirstName":
                    query = query.OrderBy(r => r.User.FirstName);
                    break;
                case "LastName":
                    query = query.OrderBy(r => r.User.LastName);
                    break;
                case "Email":
                    query = query.OrderBy(r => r.Email);
                    break;
                case "Status":
                    query = query.OrderBy(r => r.Registration.FirstOrDefault().RegistrationStatus.RegistrationStatus1);
                    break;
                default:
                    query = query.OrderBy(r => r.User.FirstName);
                    break;
            }

            // Преобразуем в список RunnerInfo
            return query.Select(r => new RunnerInfo
            {
                RunnerId = r.RunnerId,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                Email = r.Email,
                Status = r.Registration.FirstOrDefault().RegistrationStatus.RegistrationStatus1
            }).ToList();
        }

        private void EditRunner_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int runnerId)
            {
                // Переход на страницу редактирования бегуна
                NavigationService.Navigate(new RunnerDetailsPage(runnerId));
            }
        }

        private void ExportDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем текущий список ID бегунов из DataGrid
                var runnerIds = (RunnersGrid.ItemsSource as IEnumerable<RunnerInfo>)?.Select(r => r.RunnerId).ToList();
                if (runnerIds == null || !runnerIds.Any())
                {
                    MessageBox.Show("Нет данных для экспорта", "Экспорт данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Запрашиваем у пользователя место сохранения файла
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv",
                    DefaultExt = ".csv",
                    FileName = $"Marathon_Runners_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                // Получаем данные о бегунах напрямую из базы данных
                var runnersData = ConnectionString.connection.Runner
                    .Where(r => runnerIds.Contains(r.RunnerId))
                    .Select(r => new
                    {
                        r.RunnerId,
                        r.User.FirstName,
                        r.User.LastName,
                        r.Email,
                        r.Gender,
                        Country = r.Country.CountryName,
                        r.DateOfBirth,
                        RegistrationStatus = r.Registration.FirstOrDefault().RegistrationStatus.RegistrationStatus1,
                        RaceKitOption = r.Registration.FirstOrDefault().RaceKitOption.RaceKitOption1,
                        MarathonTypes = r.Registration.FirstOrDefault().RegistrationEvent
                            .Select(re => re.Event.EventType.EventTypeName)
                            .Distinct()
                    })
                    .ToList();

                // Создаем CSV содержимое
                var csvContent = new StringBuilder();

                // Заголовки столбцов
                csvContent.AppendLine("Имя,Фамилия,Эл. адрес,Пол,Страна,Дата рождения,Возраст,Состояние регистрации,Выбранный комплект,Тип марафонов");

                // Данные по каждому бегуну
                foreach (var runner in runnersData)
                {
                    var marathonTypes = runner.MarathonTypes != null
                        ? string.Join(", ", runner.MarathonTypes)
                        : "не указано";

                    var age = runner.DateOfBirth.HasValue
                        ? (DateTime.Now.Year - runner.DateOfBirth.Value.Year -
                           (DateTime.Now.DayOfYear < runner.DateOfBirth.Value.DayOfYear ? 1 : 0)).ToString()
                        : "не указано";

                    csvContent.AppendLine(
                        $"\"{runner.FirstName ?? "не указано"}\"," +
                        $"\"{runner.LastName ?? "не указано"}\"," +
                        $"\"{runner.Email}\"," +
                        $"\"{runner.Gender}\"," +
                        $"\"{runner.Country ?? "не указано"}\"," +
                        $"\"{runner.DateOfBirth?.ToString("dd.MM.yyyy") ?? "не указано"}\"," +
                        $"\"{age}\"," +
                        $"\"{runner.RegistrationStatus ?? "не указано"}\"," +
                        $"\"{runner.RaceKitOption ?? "не указано"}\"," +
                        $"\"{marathonTypes}\"");
                }

                // Сохраняем файл
                File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), Encoding.UTF8);

                MessageBox.Show($"Данные успешно экспортированы в файл:\n{saveFileDialog.FileName}",
                    "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем текущий список ID бегунов из DataGrid
                var runnerIds = (RunnersGrid.ItemsSource as IEnumerable<RunnerInfo>)?.Select(r => r.RunnerId).ToList();
                if (runnerIds == null || !runnerIds.Any())
                {
                    MessageBox.Show("Нет данных для экспорта", "Экспорт email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получаем данные о бегунах (имя, фамилия, email) напрямую из базы данных
                var runnersEmails = ConnectionString.connection.Runner
                    .Where(r => runnerIds.Contains(r.RunnerId))
                    .Select(r => new
                    {
                        r.User.FirstName,
                        r.User.LastName,
                        r.Email
                    })
                    .ToList();

                // Формируем список email в требуемом формате
                var emailsList = new StringBuilder();
                foreach (var runner in runnersEmails)
                {
                    emailsList.Append($"\"{runner.LastName} {runner.FirstName}\" <{runner.Email}>; ");
                }

                // Удаляем последнюю точку с запятой и пробел
                if (emailsList.Length > 0)
                {
                    emailsList.Length -= 2;
                }

                // Создаем всплывающее окно для отображения списка email
                var emailWindow = new Window
                {
                    Title = "Список email бегунов",
                    Width = 600,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var textBox = new TextBox
                {
                    Text = emailsList.ToString(),
                    TextWrapping = TextWrapping.Wrap,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Margin = new Thickness(10),
                    IsReadOnly = true,
                    FontFamily = new FontFamily("Courier New"),
                    AcceptsReturn = true,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                var copyButton = new Button
                {
                    Content = "Копировать в буфер",
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Style = (Style)FindResource("RoundedButtonStyle"),
                    Background = (Brush)new BrushConverter().ConvertFrom("Green")
                };

                copyButton.Click += (s, args) =>
                {
                    Clipboard.SetText(textBox.Text);
                    MessageBox.Show("Список email скопирован в буфер обмена", "Копирование",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                };

                var stackPanel = new StackPanel();
                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(copyButton);

                emailWindow.Content = stackPanel;
                emailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте email: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRunnersData();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRunnersData();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRunnersData();
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

    public class RunnerInfo
    {
        public int RunnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}