using System;
using System.Collections.Generic;
using System.IO.Ports;
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
//using WPF.UI.Growls.Other;
using MarathonSkills2016.Windows;
using WPF.UI.Growls.Other;

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для RunnerSponsor.xaml
    /// </summary>
    public partial class RunnerSponsor : Page
    {
        Charity fund;
        Registration rg;
        public RunnerSponsor()
        {
            InitializeComponent();
            CmbxRunners.ItemsSource = ConnectionString.connection.Registration.ToList();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность всех полей и правильность данных
            if (string.IsNullOrWhiteSpace(SponsorName.Text) || SponsorName.Text == "Ваше имя")
            {
                MessageBox.Show("Пожалуйста, введите ваше имя.");
                SponsorName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CardHolderName.Text) || CardHolderName.Text == "Владелец карты")
            {
                MessageBox.Show("Пожалуйста, введите имя владельца карты.");
                CardHolderName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CardNumTxt.Text) || CardNumTxt.Text.Length != 19 || !IsCardNumberValid(CardNumTxt.Text))
            {
                MessageBox.Show("Номер карты некорректен. Он должен состоять из 16 цифр и соответствовать формату '0000 0000 0000 0000'.");
                CardNumTxt.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CardMonth.Text) || string.IsNullOrWhiteSpace(CardYear.Text) || !IsExpirationDateValid(CardMonth.Text, CardYear.Text))
            {
                MessageBox.Show("Срок действия карты некорректен. Пожалуйста, введите действительный месяц и год.");
                CardMonth.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CVCTxt.Text) || CVCTxt.Text.Length != 3 || !CVCTxt.Text.All(char.IsDigit))
            {
                MessageBox.Show("CVC код некорректен. Он должен состоять из 3 цифр.");
                CVCTxt.Focus();
                return;
            }

            // Проверка суммы
            if (!int.TryParse(MoneyTextBox.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму пожертвования (число больше 0).");
                MoneyTextBox.Focus();
                return;
            }

            Sponsorship sp = new Sponsorship
            {
                SponsorName = SponsorName.Text,
                Amount = amount,
                RegistrationId = rg.RegistrationId
            };

            ConnectionString.connection.Sponsorship.Add(sp);
            ConnectionString.connection.SaveChanges();

            NavigationService.Navigate(new ConfirmSponsor(sp));
        }

        private bool IsCardNumberValid(string cardNumber)
        {
            // Проверка формата номера карты
            return cardNumber.Replace(" ", "").All(char.IsDigit);
        }
        private bool IsExpirationDateValid(string month, string year)
        {
            // Проверка на допустимые значения месяца и двухзначного года
            if (!int.TryParse(month, out int monthValue) || !int.TryParse(year, out int yearValue))
            {
                return false;
            }

            if (monthValue < 1 || monthValue > 12)
                return false;

            // Переводим двухзначный год в полный
            int fullYear = DateTime.Now.Year - (DateTime.Now.Year % 100) + yearValue;

            // Текущая дата
            var currentDate = DateTime.Now;

            // Проверка действительности года карты
            if (fullYear < currentDate.Year || (fullYear == currentDate.Year && monthValue < currentDate.Month))
            {
                return false;
            }

            // Проверка года на допустимые значения
            if (yearValue < 25)
            {
                return false; // Год не может быть меньше 25 (2025)
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            int money = int.Parse(MoneyTextBox.Text);
            money += 10;
            MoneyTextBox.Text = money.ToString();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            int money = int.Parse(MoneyTextBox.Text);
            if (money >= 10)
            {
                money -= 10;
                MoneyTextBox.Text = money.ToString();
            }
        }

        private void MoneyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем, вводим мы число
            if (int.TryParse(MoneyTextBox.Text, out int money) && money > 0)
            {
                LabelMoney.Content = $"${money}";
            }
            else
            {
                LabelMoney.Content = "$0"; // Если не числовое значение или <= 0, показываем 0
            }
        }

        private void CmbxRunners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rg = CmbxRunners.SelectedItem as Registration;
            fund = rg.Charity;
            FundNameLabel.Text = fund.CharityName;
        }

        private void SponsorName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SponsorName.Text == "Ваше имя")
            {
                SponsorName.Text = "";
                SponsorName.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SponsorName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SponsorName.Text))
            {
                SponsorName.Text = "Ваше имя";
                SponsorName.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CardHolderName.Text == "Владелец карты")
            {
                CardHolderName.Text = "";
                CardHolderName.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardHolderName.Text))
            {
                CardHolderName.Text = "Владелец карты";
                CardHolderName.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void TextBox_GotFocus_2(object sender, RoutedEventArgs e)
        {
            if (CardMonth.Text == "01" && CardMonth.Foreground != System.Windows.Media.Brushes.Black)
            {
                CardMonth.Text = "";
                CardMonth.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus_2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardMonth.Text))
            {
                CardMonth.Text = "01";
                CardMonth.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void TextBox_GotFocus_3(object sender, RoutedEventArgs e)
        {
            if (CardYear.Text == "2017" && CardYear.Foreground != System.Windows.Media.Brushes.Black)
            {
                CardYear.Text = "";
                CardYear.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus_3(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardYear.Text))
            {
                CardYear.Text = "2017";
                CardYear.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void TextBox_GotFocus_4(object sender, RoutedEventArgs e)
        {
            if (CVCTxt.Text == "123" && CVCTxt.Foreground != System.Windows.Media.Brushes.Black)
            {
                CVCTxt.Text = "";
                CVCTxt.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus_4(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CVCTxt.Text))
            {
                CVCTxt.Text = "123";
                CVCTxt.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            if (CardNumTxt.Text == "1234 1234 1234 1234")
            {
                CardNumTxt.Text = "";
                CardNumTxt.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(CardNumTxt.Text))
            {
                CardNumTxt.Text = "1234 1234 1234 1234";
                CardNumTxt.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void Fond_Click(object sender, RoutedEventArgs e)
        {
            FondInfo fondWindow = new FondInfo(fund);
            fondWindow.Show();
        }

 
    }
}