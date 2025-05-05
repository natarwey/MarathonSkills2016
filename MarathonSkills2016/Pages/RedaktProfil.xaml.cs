using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RedaktProfil.xaml
    /// </summary>
    public partial class RedaktProfil : Page
    {
        int age;
        byte[] imageBytes;

        private Runner currentRunner;
        private User currentUser;
        public RedaktProfil()
        {
            InitializeComponent();
            CmbxCountry.ItemsSource = ConnectionString.connection.Country.ToList();
            CmbxGender.ItemsSource = ConnectionString.connection.Gender.ToList();

            LoadCurrentUserData();
        }

        private void LoadCurrentUserData()
        {
            // Получаем текущего пользователя из базы данных
            currentUser = ConnectionString.connection.User.FirstOrDefault(u => u.Email == CurrentUser.Email);
            currentRunner = ConnectionString.connection.Runner.FirstOrDefault(r => r.Email == CurrentUser.Email);

            if (currentUser != null && currentRunner != null)
            {
                // Заполняем поля данными из базы
                EmailTxt.Content = currentUser.Email;
                NameTxt.Text = currentUser.FirstName;
                SecondNameTxt.Text = currentUser.LastName;

                // Устанавливаем пол
                var gender = CmbxGender.Items.Cast<Gender>().FirstOrDefault(g => g.Gender1 == currentRunner.Gender);
                if (gender != null)
                {
                    CmbxGender.SelectedItem = gender;
                    CmbxGender.Foreground = Brushes.Black;
                }

                // Устанавливаем дату рождения
                if (currentRunner.DateOfBirth.HasValue)
                {
                    BirthDate.SelectedDate = currentRunner.DateOfBirth.Value;
                }

                // Устанавливаем страну
                var country = CmbxCountry.Items.Cast<Country>().FirstOrDefault(c => c.CountryCode == currentRunner.CountryCode);
                if (country != null)
                {
                    CmbxCountry.SelectedItem = country;
                    CmbxCountry.Foreground = Brushes.Black;
                    CmbxCountry.FontStyle = FontStyles.Normal;
                }

                // Загружаем изображение, если оно есть
                if (currentRunner.Image != null && currentRunner.Image.Length > 0)
                {
                    imageBytes = currentRunner.Image;
                    LoadImageFromBytes(currentRunner.Image);
                }
            }
        }

        private void LoadImageFromBytes(byte[] imageData)
        {
            try
            {
                using (var ms = new MemoryStream(imageData))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    ImageCharity.Source = image;
                    BorderBg.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                ImagePathTxt.Text = System.IO.Path.GetFileName(selectedFilePath);
                ImagePathTxt.Foreground = Brushes.Black;
                ImagePathTxt.FontStyle = FontStyles.Normal;

                BitmapImage bitmap = new BitmapImage(new Uri(selectedFilePath));
                ImageCharity.Source = bitmap;
                BorderBg.Background = new SolidColorBrush(Colors.Transparent);

                imageBytes = File.ReadAllBytes(selectedFilePath);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(NameTxt.Text) || NameTxt.Text == "Имя")
            {
                MessageBox.Show("Введите имя!");
                return;
            }

            if (string.IsNullOrWhiteSpace(SecondNameTxt.Text) || SecondNameTxt.Text == "Фамилия")
            {
                MessageBox.Show("Введите фамилию!");
                return;
            }

            if (BirthDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату рождения!");
                return;
            }

            if (CmbxGender.SelectedItem == null)
            {
                MessageBox.Show("Выберите пол!");
                return;
            }

            if (CmbxCountry.SelectedItem == null)
            {
                MessageBox.Show("Выберите страну!");
                return;
            }

            // Проверка пароля, если он был изменен
            if (!string.IsNullOrEmpty(PassTxt.Password) || !string.IsNullOrEmpty(PassCheckTxt.Password))
            {
                if (PassTxt.Password != PassCheckTxt.Password)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                if (PassTxt.Password.Length < 6)
                {
                    MessageBox.Show("Пароль слишком короткий!");
                    return;
                }

                if (!PassTxt.Password.Any(char.IsUpper))
                {
                    MessageBox.Show("В пароле нет прописных символов!");
                    return;
                }

                if (!PassTxt.Password.Any(char.IsDigit))
                {
                    MessageBox.Show("В пароле нет цифр!");
                    return;
                }

                string pattern = @"[!@$#%^]";
                if (!Regex.IsMatch(PassTxt.Password, pattern))
                {
                    MessageBox.Show("В пароле нет спецсимволов!\n(!, @, #, $, %, ^)");
                    return;
                }
            }

            // Проверка возраста
            DateTime birthdate = BirthDate.SelectedDate.Value;
            DateTime now = DateTime.Now;
            age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }

            if (age < 10)
            {
                MessageBox.Show("Для участия в марафоне необходимо быть в возрасте 10 лет и старше!");
                return;
            }

            try
            {
                // Обновляем данные пользователя
                currentUser.FirstName = NameTxt.Text;
                currentUser.LastName = SecondNameTxt.Text;

                // Обновляем пароль, если он был изменен
                if (!string.IsNullOrEmpty(PassTxt.Password))
                {
                    currentUser.Password = PassTxt.Password;
                }

                // Обновляем данные бегуна
                currentRunner.Gender = (CmbxGender.SelectedItem as Gender).Gender1;
                currentRunner.DateOfBirth = BirthDate.SelectedDate;
                currentRunner.CountryCode = (CmbxCountry.SelectedItem as Country).CountryCode;

                // Обновляем изображение, если оно было изменено
                if (imageBytes != null)
                {
                    currentRunner.Image = imageBytes;
                }

                // Сохраняем изменения в базе данных
                ConnectionString.connection.SaveChanges();

                MessageBox.Show("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void BirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BirthDate.SelectedDate.HasValue)
            {
                DateTime birthdate = BirthDate.SelectedDate.Value;
                DateTime now = DateTime.Now;
                age = now.Year - birthdate.Year;
                if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
                {
                    age--;
                }
            }
        }
        private void PassCheckTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PassCheckTxt.Password))
            {
                PassCheckEnterLabel.Visibility = Visibility.Visible;
            }
        }

        private void PassCheckTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            PassCheckEnterLabel.Visibility = Visibility.Hidden;
        }

        private void NameTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameTxt.Text == "Имя")
            {
                NameTxt.Text = string.Empty;
                NameTxt.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void NameTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTxt.Text))
            {
                NameTxt.Text = "Имя";
                NameTxt.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void SecondNameTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SecondNameTxt.Text == "Фамилия")
            {
                SecondNameTxt.Text = string.Empty;
                SecondNameTxt.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        private void SecondNameTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SecondNameTxt.Text))
            {
                SecondNameTxt.Text = "Фамилия";
                SecondNameTxt.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void CmbxGender_GotFocus(object sender, RoutedEventArgs e)
        {
            CmbxGender.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void CmbxCountry_GotFocus(object sender, RoutedEventArgs e)
        {
            CmbxCountry.FontStyle = default;
            CmbxCountry.Foreground = System.Windows.Media.Brushes.Black;
        }
        private void EmailTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox emailText = sender as TextBox;
            if (emailText != null && emailText.Text == "Enter your email address")
            {
                emailText.Text = "";
                emailText.FontStyle = default;
                emailText.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void EmailTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox emailText = sender as TextBox;
            if (emailText != null && string.IsNullOrWhiteSpace(emailText.Text))
            {
                emailText.Text = "Enter your email address";
                emailText.FontStyle = default;
                emailText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void PassTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            PasEnterLabel.Visibility = Visibility.Hidden;
        }

        private void PassTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PassTxt.Password))
            {
                PasEnterLabel.Visibility = Visibility.Visible;
            }
        }


    }
}


