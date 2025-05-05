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
using System.Windows.Threading;
using MarathonSkills2016.Pages;

namespace MarathonSkills2016
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
            StartTimer();
        }
        private DispatcherTimer timer;
        private DateTime marathonStartDate;

        private void UpdateTimeLeft()
        {
            TimeSpan timeLeft = marathonStartDate - DateTime.Now;
            if (timeLeft.TotalMilliseconds <= 0)
            {
                TimeLeftTextBlock.Text = "Марафон уже начался!";
                timer.Stop();
            }
            else
            {
                TimeLeftTextBlock.Text = $"{timeLeft.Days} дней {timeLeft.Hours} часов и {timeLeft.Minutes} минут до старта марафона!";
            }
        }

        private void StartTimer()
        {
            marathonStartDate = new DateTime(2025, 05, 14, 14, 0, 0);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            UpdateTimeLeft();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeLeft();
        }
    }
}
