using Microsoft.Win32;
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
using System.IO;

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для CharityEditPage.xaml
    /// </summary>
    public partial class CharityEditPage : Page
    {
        private Charity _currentCharity;
        private bool _isEditMode;
        private string _newLogoPath;
        public CharityEditPage(Charity charity = null)
        {
            InitializeComponent();

            _isEditMode = charity != null;
            _currentCharity = charity ?? new Charity();

            DataContext = _currentCharity;
            UpdateCountdown();

            // Set title based on mode
            if (_isEditMode)
            {
                Title = "Редактирование благотворительной организации";
                // Load current logo if exists
                if (!string.IsNullOrEmpty(_currentCharity.CharityLogo))
                {
                    var logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "materials", _currentCharity.CharityLogo);
                    if (File.Exists(logoPath))
                    {
                        CurrentLogoImage.Source = new BitmapImage(new Uri(logoPath));
                    }
                }
            }
            else
            {
                Title = "Добавление благотворительной организации";
            }
        }
        private void UpdateCountdown()
        {
            CountdownTextBlock.Text = "18 дней 8 часов и 17 минут до старта марафона!";
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BrowseLogoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp",
                Title = "Выберите логотип организации"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _newLogoPath = openFileDialog.FileName;
                LogoPathTextBox.Text = System.IO.Path.GetFileName(_newLogoPath);

                // Preview the new logo
                var bitmap = new BitmapImage(new Uri(_newLogoPath));
                CurrentLogoImage.Source = bitmap;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(_currentCharity.CharityName))
            {
                MessageBox.Show("Поле 'Наименование' не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Handle logo file
                if (!string.IsNullOrEmpty(_newLogoPath))
                {
                    // Validate image file
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = System.IO.Path.GetExtension(_newLogoPath)?.ToLower();

                    if (!validExtensions.Contains(extension))
                    {
                        MessageBox.Show("Недопустимый формат файла логотипа. Разрешены только JPG, JPEG, PNG, GIF, BMP.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Copy logo to materials folder
                    var materialsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "materials");
                    if (!Directory.Exists(materialsPath))
                    {
                        Directory.CreateDirectory(materialsPath);
                    }

                    var destPath = System.IO.Path.Combine(materialsPath, System.IO.Path.GetFileName(_newLogoPath));
                    File.Copy(_newLogoPath, destPath, true);
                    _currentCharity.CharityLogo = System.IO.Path.GetFileName(_newLogoPath);
                }

                // Save to database
                if (!_isEditMode)
                {
                    ConnectionString.connection.Charity.Add(_currentCharity);
                }

                ConnectionString.connection.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}