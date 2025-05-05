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
    /// Логика взаимодействия для SponsorsOverviewPage.xaml
    /// </summary>
    public partial class SponsorsOverviewPage : Page
    {
        public SponsorsOverviewPage()
        {
            InitializeComponent();
            LoadSponsorsData();
        }

        private void LoadSponsorsData()
        {
            try
            {
                string sortField = (SortComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "TotalAmount";

                // Сначала получаем данные без преобразования логотипа
                var sponsorsQuery = ConnectionString.connection.Sponsorship
                    .Where(s => s.Registration != null && s.Registration.Charity != null)
                    .GroupBy(s => s.Registration.Charity)
                    .Select(g => new
                    {
                        Charity = g.Key,
                        TotalAmount = g.Sum(s => s.Amount),
                        SponsorsCount = g.Count()
                    });

                // Выполняем запрос и преобразуем в память
                var sponsorsData = sponsorsQuery.ToList()
                    .Select(x => new CharitySponsorsInfo
                    {
                        CharityId = x.Charity.CharityId,
                        CharityName = x.Charity.CharityName,
                        LogoPath = GetLogoPath(x.Charity.CharityLogo), // Теперь это выполняется в памяти
                TotalAmount = x.TotalAmount,
                        SponsorsCount = x.SponsorsCount
                    })
                    .ToList();

                // Проверяем, что данные получены
                if (sponsorsData != null && CharitiesCountText != null && TotalAmountText != null)
                {
                    // Обновляем сводную информацию
                    CharitiesCountText.Text = sponsorsData.Count.ToString();
                    TotalAmountText.Text = sponsorsData.Sum(c => c.TotalAmount).ToString("C");

                    // Сортировка
                    switch (sortField)
                    {
                        case "CharityName":
                            sponsorsData = sponsorsData.OrderBy(c => c.CharityName).ToList();
                            break;
                        case "SponsorsCount":
                            sponsorsData = sponsorsData.OrderByDescending(c => c.SponsorsCount).ToList();
                            break;
                        default: // "TotalAmount"
                            sponsorsData = sponsorsData.OrderByDescending(c => c.TotalAmount).ToList();
                            break;
                    }

                    SponsorsGrid.ItemsSource = sponsorsData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных спонсоров: {ex.Message}");
            }
        }

        // Метод для получения пути к логотипу (теперь вызывается после выполнения запроса)
        
        private string GetLogoPath(string logoFileName)
        {
            if (string.IsNullOrEmpty(logoFileName))
                return "/Resources/default-charity.png"; // Запасной логотип

            // Убедитесь, что путь соответствует структуре вашего проекта
            return $"/Resources/Charities/{logoFileName}";
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSponsorsData();
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

    public class CharitySponsorsInfo
    {
        public int CharityId { get; set; }
        public string CharityName { get; set; }
        public string LogoPath { get; set; }
        public decimal TotalAmount { get; set; }
        public int SponsorsCount { get; set; }
    }
}