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
    public partial class InventoryArrivalPage : Page
    {
        private DoneEntities _context;

        public InventoryArrivalPage()
        {
            InitializeComponent();
            _context = new DoneEntities();
            LoadInventoryData();
        }

        private void LoadInventoryData()
        {
            try
            {
                var inventoryItems = _context.InventoryItem.ToList()
                    .Select(i => new InventoryArrivalViewModel
                    {
                        InventoryItemId = i.InventoryItemId,
                        ItemName = i.ItemName,
                        CurrentStock = i.CurrentStock,
                        ArrivalQuantity = 0 // Default to 0 for new arrivals
                    }).ToList();

                InventoryItemsGrid.ItemsSource = inventoryItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных инвентаря: {ex.Message}");
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and minus sign (for write-offs)
            if (!char.IsDigit(e.Text, 0) && e.Text != "-")
            {
                e.Handled = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (InventoryArrivalViewModel item in InventoryItemsGrid.Items)
                {
                    // Skip items with no change (0 quantity)
                    if (item.ArrivalQuantity == 0)
                        continue;

                    var inventoryItem = _context.InventoryItem.Find(item.InventoryItemId);
                    if (inventoryItem != null)
                    {
                        // Update stock level (can be positive or negative)
                        inventoryItem.CurrentStock += item.ArrivalQuantity;

                        // Ensure stock doesn't go negative (unless explicitly allowed)
                        if (inventoryItem.CurrentStock < 0)
                        {
                            var result = MessageBox.Show(
                                $"Отрицательный остаток для {item.ItemName}. Продолжить?",
                                "Подтверждение",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

                            if (result != MessageBoxResult.Yes)
                            {
                                return; // Abort save if user cancels
                            }
                        }
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены успешно", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh data after save
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose(); // Clean up the context
            NavigationService.GoBack();
        }

        /*protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _context.Dispose(); // Clean up the context when navigating away
        }*/
    }

    public class InventoryArrivalViewModel
    {
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; }
        public int CurrentStock { get; set; }
        public int ArrivalQuantity { get; set; }
    }
}