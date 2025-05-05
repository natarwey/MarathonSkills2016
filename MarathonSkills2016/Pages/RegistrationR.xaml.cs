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
    /// Логика взаимодействия для RegistrationR.xaml
    /// </summary>
    public partial class RegistrationR : Page
    {
        byte[] imageBytes;
        int age;
        public RegistrationR()
        {
            InitializeComponent();
            CmbxGender.ItemsSource = ConnectionString.connection.Gender.ToList();
            CmbxCountry.ItemsSource = ConnectionString.connection.Country.ToList();
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

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void BtnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Multiselect = false
            };
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                ImagePathTxt.Text = System.IO.Path.GetFileName(selectedFilePath);
                ImagePathTxt.Foreground = System.Windows.Media.Brushes.Black;
                ImagePathTxt.FontStyle = default;
                BitmapImage bitmap = new BitmapImage(new Uri(selectedFilePath));
                ImageCharity.Source = bitmap;
                BorderBg.Background = new SolidColorBrush(Colors.Transparent);
                imageBytes = File.ReadAllBytes(selectedFilePath);
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTxt.Text == "Имя")
            {
                MessageBox.Show("Введите имя!");
                return;
            }
            if (SecondNameTxt.Text == "Фамилия")
            {
                MessageBox.Show("Введите фамилию!");
                return;
            }
            if (EmailTxt.Text == "Email")
            {
                MessageBox.Show("Введите ваш email!");
                return;
            }
            if (string.IsNullOrEmpty(PassTxt.Password))
            {
                MessageBox.Show("Введите ваш пароль!");
                return;
            }
            if (PassCheckTxt.Password != PassTxt.Password)
            {
                MessageBox.Show("Вы не подтвердили пароль!");
                return;
            }
            if (ImagePathTxt.Text == "Photo_logo.jpg" && ImagePathTxt.Foreground == System.Windows.Media.Brushes.Green)
            {
                MessageBox.Show("Выберите фото!");
                return;
            }
            if (BirthDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения!");
                return;
            }
            if (CmbxCountry.SelectedIndex == 77 && CmbxCountry.Foreground == System.Windows.Media.Brushes.Green)
            {
                MessageBox.Show("Выберите страну!");
                return;
            }
            if (CmbxGender.SelectedIndex == 0 && CmbxGender.Foreground == System.Windows.Media.Brushes.Green)
            {
                MessageBox.Show("Выберите ваш пол!");
                return;
            }
            string email = EmailTxt.Text;
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            bool isValid = regex.IsMatch(email);
            if (!isValid)
            {
                MessageBox.Show("Email не соответствует формату!");
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
            string anotherPattern = @"[!@$#%^]";
            Regex anRegex = new Regex(anotherPattern);
            if (!anRegex.IsMatch(PassTxt.Password))
            {
                MessageBox.Show("В пароле нет спецсимволов!\n(!, @, #, $, %, ^)");
                return;
            }
            if (age < 10)
            {
                MessageBox.Show("Для участия в марафоне необходимо быть в возрасте 10 лет и старше!");
                return;
            }
            Runner rn = new Runner();
            User user = new User();
            user.Email = email;
            user.Password = PassTxt.Password;
            user.FirstName = NameTxt.Text;
            user.LastName = SecondNameTxt.Text;
            user.RoleId = "R";
            rn.Email = email;
            Gender selGen = CmbxGender.SelectedItem as Gender;
            rn.Gender = selGen.Gender1;
            rn.DateOfBirth = BirthDate.SelectedDate;
            Country SelectedCountry = CmbxCountry.SelectedItem as Country;
            rn.CountryCode = SelectedCountry.CountryCode;
            rn.Image = imageBytes;
            ConnectionString.connection.User.Add(user);
            ConnectionString.connection.Runner.Add(rn);
            ConnectionString.connection.SaveChanges();
            MessageBox.Show("Вы успешно зарегистрировались!");
            return;
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

        private void BirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime birthdate = (DateTime)BirthDate.SelectedDate;
            DateTime now = DateTime.Now;
            age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month ||
            (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }
        }
    }
}