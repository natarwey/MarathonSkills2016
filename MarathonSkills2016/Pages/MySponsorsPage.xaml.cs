using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarathonSkills2016.Database;

namespace MarathonSkills2016.Pages
{
    public partial class MySponsorsPage : Page
    {
        public MySponsorsPage()
        {
            InitializeComponent();
            LoadSponsorsData();
        }

        private void LoadSponsorsData()
        {
            try
            {
                // Получаем текущего бегуна
                var runner = ConnectionString.connection.Runner
                    .FirstOrDefault(r => r.Email == CurrentUser.Email);

                if (runner == null) return;

                // Получаем регистрацию бегуна
                var registration = runner.Registration.FirstOrDefault();
                if (registration == null)
                {
                    NoSponsorsText.Visibility = Visibility.Visible;
                    SponsorsGrid.Visibility = Visibility.Collapsed;
                    return;
                }

                // Получаем благотворительную организацию
                var charity = registration.Charity;
                if (charity != null)
                {
                    CharityNameText.Text = charity.CharityName;
                    CharityDescriptionText.Text = charity.CharityDescription;

                    // Загружаем логотип
                    if (!string.IsNullOrEmpty(charity.CharityLogo))
                    {
                        try
                        {
                            // Пробуем загрузить из папки Materials
                            string logoPath = System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "Materials",
                                charity.CharityLogo);

                            if (System.IO.File.Exists(logoPath))
                            {
                                var logoImage = new BitmapImage(new Uri(logoPath));
                                CharityLogoImage.Source = logoImage;
                            }
                            else
                            {
                                // Альтернативный вариант - использование ресурсов
                                var uri = new Uri($"pack://application:,,,/MarathonSkills2016;component/Materials/{charity.CharityLogo}");
                                CharityLogoImage.Source = new BitmapImage(uri);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Не удалось загрузить логотип: {ex.Message}");
                        }
                    }
                }

                // Получаем спонсоров бегуна
                var sponsorships = ConnectionString.connection.Sponsorship
                    .Where(s => s.RegistrationId == registration.RegistrationId)
                    .ToList();

                if (sponsorships.Any())
                {
                    var sponsorList = new List<SponsorInfo>();
                    foreach (var s in sponsorships)
                    {
                        var sponsor = new SponsorInfo
                        {
                            SponsorName = s.SponsorName,
                            Amount = s.Amount
                        };

                        // Загружаем логотип для спонсора
                        if (charity != null && !string.IsNullOrEmpty(charity.CharityLogo))
                        {
                            try
                            {
                                // Пробуем загрузить из папки Materials
                                string logoPath = System.IO.Path.Combine(
                                    AppDomain.CurrentDomain.BaseDirectory,
                                    "Materials",
                                    charity.CharityLogo);

                                if (System.IO.File.Exists(logoPath))
                                {
                                    sponsor.LogoImage = new BitmapImage(new Uri(logoPath));
                                }
                                else
                                {
                                    // Альтернативный вариант - использование ресурсов
                                    var uri = new Uri($"pack://application:,,,/MarathonSkills2016;component/Materials/{charity.CharityLogo}");
                                    sponsor.LogoImage = new BitmapImage(uri);
                                }
                            }
                            catch
                            {
                                sponsor.LogoImage = null;
                            }
                        }

                        sponsorList.Add(sponsor);
                    }

                    SponsorsGrid.ItemsSource = sponsorList;
                    NoSponsorsText.Visibility = Visibility.Collapsed;

                    // Вычисляем общую сумму
                    decimal totalAmount = sponsorships.Sum(s => s.Amount);
                    TotalAmountText.Text = $"${totalAmount}";
                }
                else
                {
                    SponsorsGrid.Visibility = Visibility.Collapsed;
                    NoSponsorsText.Visibility = Visibility.Visible;
                    TotalAmountText.Text = "$0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных спонсоров: {ex.Message}");
            }
        }

        /* private BitmapImage LoadImageFromBytes(byte[] imageData)
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
                     return image;
                 }
             }
             catch
             {
                 return null;
             }
         }*/

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }

    public class SponsorInfo
    {
        public BitmapImage LogoImage { get; set; }
        public string SponsorName { get; set; }
        public decimal Amount { get; set; }
    }
}