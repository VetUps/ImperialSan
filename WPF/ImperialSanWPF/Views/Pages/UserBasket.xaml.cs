using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserBasket.xaml
    /// </summary>
    public partial class UserBasket : Page, INotifyPropertyChanged
    {
        #region Поля
        private ObservableCollection<ProductBasketPosition> _productBasketPositions;
        #endregion

        #region Свойства
        public ObservableCollection<ProductBasketPosition> ProductBasketPositions
        {
            get => _productBasketPositions;
            set
            {
                if (value != _productBasketPositions)
                {
                    _productBasketPositions = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion 

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public UserBasket()
        {
            _productBasketPositions = new ObservableCollection<ProductBasketPosition>();

            InitializeComponent();
            DataContext = this;

            InitializeProductBasketPositions();
        }

        private async Task InitializeProductBasketPositions()
        {
            Basket currentBasket = SessionContext.UserBasket;
            
            try
            {
                foreach (BasketPosition position in currentBasket.Positions)
                {
                    HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Product/{position.ProductId}");

                    if (response.IsSuccessStatusCode)
                    {
                        Product result = await response.Content.ReadFromJsonAsync<Product>();

                        ProductBasketPositions.Add(new ProductBasketPosition
                        {
                            ProductId = position.ProductId,
                            ImageUrl = result.ImageUrl,
                            ProductTitle = result.ProductTitle,
                            ProductQuantity = position.ProductQuantity,
                            Price = result.Price,
                        });
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

                        if (errorResponse.Errors != null)
                        {
                            List<string> errorMessages = new List<string>();

                            foreach (var error in errorResponse.Errors)
                                errorMessages.Add(error.Value[0]);

                            MessageBox.Show(errorMessages[0]);
                        }

                        else
                        {
                            MessageBox.Show($"Неизвестная ошибка: {response.StatusCode} {errorResponse}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private void backToCatalogButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage());
        }
    }
}
