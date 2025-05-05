using MarathonSkills2016.Database;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace MarathonSkills2016.Pages
{
    public partial class CharityManagementPage : Page
    {
        public CharityManagementPage()
        {
            InitializeComponent();
            LoadCharities();
            UpdateCountdown();
        }

        private void LoadCharities()
        {
            try
            {
                LvCharity.ItemsSource = ConnectionString.connection.Charity.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCountdown()
        {
            CountdownTextBlock.Text = "18 дней 8 часов и 17 минут до старта марафона!";
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Функциональность добавления будет реализована позже",
            //    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new CharityEditPage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int charityId = (int)button.Tag;
                var charityToEdit = ConnectionString.connection.Charity.FirstOrDefault(c => c.CharityId == charityId);
                if (charityToEdit != null)
                {
                    NavigationService.Navigate(new CharityEditPage(charityToEdit));
                }
            }
        }
    }
}