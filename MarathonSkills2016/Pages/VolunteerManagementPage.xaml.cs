using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MarathonSkills2016.Database;

namespace MarathonSkills2016.Pages
{
    public partial class VolunteerManagementPage : Page
    {
        public VolunteerManagementPage()
        {
            InitializeComponent();
            Loaded += VolunteerManagementPage_Loaded;
        }

        private void VolunteerManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadVolunteers();
                UpdateCountdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVolunteers(string sortBy = "LastName")
        {
            if (VolunteersListView == null) return;

            try
            {
                using (var context = ConnectionString.connection)
                {
                    IQueryable<Volunteer> query = context.Volunteer;

                    switch (sortBy)
                    {
                        case "FirstName":
                            query = query.OrderBy(v => v.FirstName);
                            break;
                        case "CountryCode":
                            query = query.OrderBy(v => v.CountryCode);
                            break;
                        case "Gender":
                            query = query.OrderBy(v => v.Gender);
                            break;
                        default:
                            query = query.OrderBy(v => v.LastName);
                            break;
                    }

                    var volunteers = query.ToList();
                    VolunteersListView.ItemsSource = volunteers;
                    VolunteerCountText.Text = $"Всего волонтеров: {volunteers.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке волонтеров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCountdown()
        {
            CountdownTextBlock.Text = "18 дней 8 часов и 17 минут до старта марафона!";
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string sortBy = selectedItem.Content.ToString();
                switch (sortBy)
                {
                    case "Фамилии":
                        LoadVolunteers("LastName");
                        break;
                    case "Имени":
                        LoadVolunteers("FirstName");
                        break;
                    case "Стране":
                        LoadVolunteers("CountryCode");
                        break;
                    case "Полу":
                        LoadVolunteers("Gender");
                        break;
                }
                LoadVolunteers(sortBy);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new VolunteerImportPage());
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVolunteers();
        }
    }
}