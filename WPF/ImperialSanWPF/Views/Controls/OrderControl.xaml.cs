using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace ImperialSanWPF.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для OrderControl.xaml
    /// </summary>
    public partial class OrderControl : UserControl
    {
        public OrderControl()
        {
            InitializeComponent();
        }

        private async void GetReceipt_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int orderId = (DataContext as OrderForProfile).OrderId;
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Order/get_receipt/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PDF файлы (*.pdf)|*.pdf|Все файлы (*.*)|*.*",
                        FileName = $"Чек_Заказ_{orderId}.pdf"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, pdfBytes);
                        MessageBox.Show("Чек сохранён!");
                    }
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private async void abortOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int orderId = (DataContext as OrderForProfile).OrderId;
                var abortOrderModel = new
                {
                    orderId = orderId
                };

                var jsonData = JsonSerializer.Serialize(abortOrderModel);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, "Order/abort_order")
                {
                    Content = content
                };

                using HttpResponseMessage response = await BaseHttpClient.httpClient.SendAsync(message);

                if (response.IsSuccessStatusCode)
                {
                    MainWindowClass.mainWindow.mainFrame.Navigate(new UserProfilePage());
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }
    }
}
