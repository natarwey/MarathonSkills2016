using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MarathonSkills2016.Database;
using System.Reflection;

namespace MarathonSkills2016.Windows
{
    /// <summary>
    /// Логика взаимодействия для FondInfo.xaml
    /// </summary>
    public partial class FondInfo : Window
    {
        private readonly Charity _fund;

        public FondInfo(Charity fund)
        {
            InitializeComponent();
            _fund = fund;

            // Установка текстовых данных
            FundNameTxt.Text = _fund.CharityName;
            FundInfoTxt.Text = _fund.CharityDescription;

            // Загрузка логотипа
            LoadLogo();
        }

        private void LoadLogo()
        {
            try
            {
                if (!string.IsNullOrEmpty(_fund.CharityLogo))
                {
                    // Путь к папке с материалами в выходной директории
                    string logoPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Materials",
                        _fund.CharityLogo);

                    if (System.IO.File.Exists(logoPath))
                    {
                        var logoImage = new BitmapImage(new Uri(logoPath));
                        LogoEllipse.Fill = new ImageBrush(logoImage);
                    }
                    else
                    {
                        // Альтернативный вариант - использование ресурсов
                        var uri = new Uri($"pack://application:,,,/MarathonSkills2016;component/materials/{_fund.CharityLogo}");
                        var logoImage = new BitmapImage(uri);
                        LogoEllipse.Fill = new ImageBrush(logoImage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить логотип: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
