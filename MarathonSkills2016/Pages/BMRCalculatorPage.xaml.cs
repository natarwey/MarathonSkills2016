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

namespace MarathonSkills2016.Pages
{
    /// <summary>
    /// Логика взаимодействия для BMRCalculatorPage.xaml
    /// </summary>
    public partial class BMRCalculatorPage : Page
    {
        private string selectedGender = "Male";

        public BMRCalculatorPage()
        {
            InitializeComponent();
        }

        private void GenderSelected(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            // Получаем Image внутри Border
            var image = border.Child as Image;
            if (image == null) return;

            selectedGender = image.Tag?.ToString();
            if (string.IsNullOrEmpty(selectedGender)) return;

            // Reset borders
            MaleBorder.BorderBrush = Brushes.Transparent;
            MaleBorder.BorderThickness = new Thickness(2);
            FemaleBorder.BorderBrush = Brushes.Transparent;
            FemaleBorder.BorderThickness = new Thickness(2);

            // Highlight selected gender
            border.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("Green");
            border.BorderThickness = new Thickness(3);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(HeightTextBox.Text, out double height) || height <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный рост (положительное число).");
                return;
            }

            if (!double.TryParse(WeightTextBox.Text, out double weight) || weight <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный вес (положительное число).");
                return;
            }

            if (!int.TryParse(AgeTextBox.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный возраст (положительное число).");
                return;
            }

            double bmr = CalculateBMR(height, weight, age);
            UpdateResults(bmr);
        }

        private double CalculateBMR(double height, double weight, int age)
        {
            if (selectedGender == "Male")
            {
                return 66 + (13.7 * weight) + (5 * height) - (6.8 * age);
            }
            else
            {
                return 655 + (9.6 * weight) + (1.8 * height) - (4.7 * age);
            }
        }

        private void UpdateResults(double bmr)
        {
            BMRTextBlock.Text = bmr.ToString("N0");
            SedentaryTextBlock.Text = (bmr * 1.2).ToString("N0");
            LightActivityTextBlock.Text = (bmr * 1.375).ToString("N0");
            ModerateActivityTextBlock.Text = (bmr * 1.55).ToString("N0");
            HeavyActivityTextBlock.Text = (bmr * 1.725).ToString("N0");
            ExtremeActivityTextBlock.Text = (bmr * 1.9).ToString("N0");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset all fields
            HeightTextBox.Text = "180";
            WeightTextBox.Text = "70";
            AgeTextBox.Text = "30";
            
            // Reset gender selection
            selectedGender = "Male";
            MaleBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("Green");
            MaleBorder.BorderThickness = new Thickness(3);
            FemaleBorder.BorderBrush = Brushes.Transparent;
            FemaleBorder.BorderThickness = new Thickness(2);
            
            // Reset results
            BMRTextBlock.Text = "0";
            SedentaryTextBlock.Text = "0";
            LightActivityTextBlock.Text = "0";
            ModerateActivityTextBlock.Text = "0";
            HeavyActivityTextBlock.Text = "0";
            ExtremeActivityTextBlock.Text = "0";
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "Уровни активности:\n\n" +
                            "Сидячий образ: Нет работы или работа за столом\n" +
                            "Маленькая активность: Мало физических работы или занятия спортом 1-3 раза в неделю\n" +
                            "Средняя активность: Умеренная физическая работа или занятия спортом 3-5 дней в неделю\n" +
                            "Сильная активность: Сильные физическая нагрузка или занятия спортом 6-7 дней в неделю\n" +
                            "Максимальная активность: Сильная ежедневная физическая нагрузка или спорт и физическая работа";

            MessageBox.Show(message, "Описание уровней активности", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}//cессия 4 закончена