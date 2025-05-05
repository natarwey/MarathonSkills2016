using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MarathonSkills2016.Database;
using MarathonSkills2016.Windows;

namespace MarathonSkills2016.Pages
{
    public partial class RegistrationMarafon : Page
    {
        // Классы для привязки данных
        public class EventTypeViewModel
        {
            public string EventTypeId { get; set; }
            public string EventTypeName { get; set; }
            public decimal Cost { get; set; }
            public bool IsSelected { get; set; }
            public string EventTypeNameWithPrice => $"{EventTypeName} (${Cost})";
        }

        public class RaceKitOptionViewModel
        {
            public string RaceKitOptionId { get; set; }
            public string RaceKitOption { get; set; }
            public decimal Cost { get; set; }
            public bool IsSelected { get; set; }
            public string RaceKitOptionWithPrice => $"{RaceKitOption} (${Cost})";
        }

        public RegistrationMarafon()
        {
            InitializeComponent();
            LoadData();
            CalculateTotalCost();
        }
        private void EventCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CalculateTotalCost();
        }
   

        private void LoadData()
        {
            try
            {
                // Загрузка типов событий
                var eventTypes = ConnectionString.connection.Event
                    .Join(ConnectionString.connection.EventType,
                        e => e.EventTypeId,
                        et => et.EventTypeId,
                        (e, et) => new EventTypeViewModel
                        {
                            EventTypeId = et.EventTypeId,
                            EventTypeName = et.EventTypeName,
                            Cost = e.Cost ?? 0,
                            IsSelected = false
                        })
                    .Distinct()
                    .ToList();

                EventsList.ItemsSource = eventTypes;

                // Загрузка вариантов комплектов
                var kitOptions = ConnectionString.connection.RaceKitOption
                    .Select(r => new RaceKitOptionViewModel
                    {
                        RaceKitOptionId = r.RaceKitOptionId,
                        RaceKitOption = r.RaceKitOption1,
                        Cost = r.Cost,
                        IsSelected = r.RaceKitOptionId == "A" // Выбран по умолчанию вариант A
                    })
                    .ToList();

                KitOptionsList.ItemsSource = kitOptions;

                // Загрузка благотворительных фондов
                CmbxFond.ItemsSource = ConnectionString.connection.Charity.ToList();
                CmbxFond.DisplayMemberPath = "CharityName";
                CmbxFond.SelectedValuePath = "CharityId";
                CmbxFond.SelectedIndex = 0;

                // Устанавливаем начальную сумму
                CalculateTotalCost();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculateTotalCost()
        {
            decimal total = 0;

            // Сумма за выбранные события
            if (EventsList.ItemsSource is IEnumerable<EventTypeViewModel> eventTypes)
            {
                total += eventTypes.Where(e => e.IsSelected).Sum(e => e.Cost);
            }

            // Стоимость выбранного комплекта
            if (KitOptionsList.ItemsSource is IEnumerable<RaceKitOptionViewModel> kitOptions)
            {
                var selectedKit = kitOptions.FirstOrDefault(k => k.IsSelected);
                if (selectedKit != null)
                {
                    total += selectedKit.Cost;
                }
            }

            // Обновляем текст с общей стоимостью
            TotalCostText.Text = $"${total}";

            // Возвращаем общую стоимость
            return total;
        }


        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Получаем текущего пользователя и проверяем наличие профиля бегуна
                var currentUser = ConnectionString.connection.User
                                .Include(u => u.Runner) // Важно: подгружаем связанные данные
                                .FirstOrDefault(u => u.Email == CurrentUser.Email);

                if (currentUser == null || currentUser.Runner == null || currentUser.Runner.Count == 0)
                {
                    var result = MessageBox.Show("Для регистрации на марафон требуется профиль бегуна. Хотите создать его сейчас?",
                                              "Требуется профиль",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        NavigationService.Navigate(new RedaktProfil());
                    }
                    return;
                }

                // Получаем первого бегуна (предполагаем, что у пользователя только один бегун)
                var runner = currentUser.Runner.FirstOrDefault();
                if (runner == null)
                {
                    MessageBox.Show("Профиль бегуна не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2. Проверка выбранных событий
                var selectedEvents = ((IEnumerable<EventTypeViewModel>)EventsList.ItemsSource)
                        .Where(ev => ev.IsSelected)
                        .ToList();

                if (selectedEvents.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите хотя бы один вид марафона",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. Проверка суммы взноса
                if (!decimal.TryParse(TxtSum.Text.Trim('$'), out decimal sponsorshipAmount) || sponsorshipAmount < 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректную сумму взноса",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 4. Проверка выбранного фонда
                if (CmbxFond.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите благотворительный фонд",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 5. Проверка выбранного комплекта
                var selectedKit = ((IEnumerable<RaceKitOptionViewModel>)KitOptionsList.ItemsSource)
                    .FirstOrDefault(k => k.IsSelected);

                if (selectedKit == null)
                {
                    MessageBox.Show("Пожалуйста, выберите вариант комплекта",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 6. Проверка на существующую регистрацию
                var existingRegistration = ConnectionString.connection.Registration
                    .FirstOrDefault(r => r.RunnerId == runner.RunnerId &&
                                       (r.RegistrationStatusId == 1 || r.RegistrationStatusId == 2)); // 1 = "Pending", 2 = "Confirmed"

                if (existingRegistration != null)
                {
                    MessageBox.Show("У вас уже есть активная регистрация",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                // 7. Создаем объект регистрации
                var registration = new Registration
                {
                    RegistrationDateTime = DateTime.Now,
                    RaceKitOptionId = selectedKit.RaceKitOptionId,
                    RegistrationStatusId = 1, // "Pending"
                    CharityId = (int)CmbxFond.SelectedValue,
                    SponsorshipTarget = sponsorshipAmount,
                    Cost = CalculateTotalCost(),
                    RunnerId = runner.RunnerId // Используем ID найденного бегуна
                };

                // 8. Сохраняем регистрацию
                ConnectionString.connection.Registration.Add(registration);
                ConnectionString.connection.SaveChanges();

                // 9. Добавляем выбранные события
                foreach (var eventType in selectedEvents)
                {
                    var eventObj = ConnectionString.connection.Event
                        .FirstOrDefault(ev => ev.EventTypeId == eventType.EventTypeId);

                    if (eventObj != null)
                    {
                        var registrationEvent = new RegistrationEvent
                        {
                            RegistrationId = registration.RegistrationId,
                            EventId = eventObj.EventId
                        };
                        ConnectionString.connection.RegistrationEvent.Add(registrationEvent);
                    }
                }

                ConnectionString.connection.SaveChanges();

                // 10. Уведомление об успехе и переход
                MessageBox.Show("Регистрация успешно завершена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Переход на страницу подтверждения (без передачи параметров, если конструктор не принимает)
                NavigationService.Navigate(new PodtverditRegMarafon());
                CalculateTotalCost();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.InnerException?.Message}",
                               "Ошибка базы данных",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        // Остальные методы без изменений
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void NameTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSum.Text == "$500")
            {
                TxtSum.Text = string.Empty;
                TxtSum.Foreground = Brushes.Black;
            }
        }

        private void NameTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSum.Text))
            {
                TxtSum.Text = "$500";
                TxtSum.Foreground = Brushes.Green;
            }
            else if (!TxtSum.Text.StartsWith("$"))
            {
                TxtSum.Text = "$" + TxtSum.Text.Trim('$');
            }
        }

        private void Fond_Click(object sender, RoutedEventArgs e)
        {
            if (CmbxFond.SelectedItem is Charity selectedCharity)
            {
                FondInfo fondWindow = new FondInfo(selectedCharity);
                fondWindow.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите благотворительный фонд",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EventCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CalculateTotalCost();
        }

        private void KitOptionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CalculateTotalCost();
        }

    }
}