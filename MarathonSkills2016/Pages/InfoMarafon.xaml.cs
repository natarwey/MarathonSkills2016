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
    /// Логика взаимодействия для InfoMarafon.xaml
    /// </summary>
    public partial class InfoMarafon : Page
    {
        DoneEntities db = new DoneEntities();

        public InfoMarafon()
        {
            InitializeComponent();
            gridDistance.ItemsSource = db.HowLong.Where(x => x.Length != null).ToList();
            gridSpeed.ItemsSource = db.HowLong.Where(x => x.Speed != null).ToList();
        }

        private void gridDistance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = gridDistance.SelectedItem as HowLong;
            if (element != null)
            {
                txtName.Text = element.Name;

                // Парсим длину (удаляем 'm' и конвертируем в double)
                if (double.TryParse(element.Length?.Replace("m", ""), out double lengthMeters))
                {
                    double marathonLength = 42195; // 42.195 км в метрах
                    double count = marathonLength / lengthMeters;

                    txtInfo.Text = $"Длина {element.Name} {element.Length}. " +
                                  $"Это займет {Math.Round(count, 1)} из них, " +
                                  "чтобы покрыть расстояние в 42 км марафона";
                }

                // Загрузка изображения из ресурсов
                imgInfo.Source = LoadImageFromResources(element.Name.ToLower().Replace(" ", "-") + ".jpg");
            }
        }

        private void gridSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = gridSpeed.SelectedItem as HowLong;
            if (element != null)
            {
                txtName.Text = element.Name;

                // Парсим скорость (удаляем 'km/h' и конвертируем в double)
                if (double.TryParse(element.Speed?.Replace("km/h", ""), out double speedKmh))
                {
                    double marathonLength = 42.195; // длина марафона в км
                    double hours = marathonLength / speedKmh;

                    TimeSpan time = TimeSpan.FromHours(hours);
                    string timeString = time.TotalHours >= 1
                        ? $"{Math.Floor(time.TotalHours)} часов {time.Minutes} минут"
                        : $"{time.Minutes} минут";

                    txtInfo.Text = $"Максимальная скорость {element.Name} {element.Speed}. " +
                                  $"Это займет {timeString} чтобы завершить 42km марафон";
                }

                // Загрузка изображения из ресурсов
                imgInfo.Source = LoadImageFromResources(element.Name.ToLower().Replace(" ", "-") + ".jpg");
            }
        }

        // Метод для загрузки изображения из ресурсов
        private BitmapImage LoadImageFromResources(string imageName)
        {
            try
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Resources/HowLong/{imageName}"));
            }
            catch
            {
                // Возвращаем изображение-заглушку, если основное не найдено
                return new BitmapImage(new Uri("pack://application:,,,/Resources/HowLong/default.jpg"));
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}