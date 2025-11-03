using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserBasket.xaml
    /// </summary>
    public partial class UserBasket : Page, INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Поля
        private ObservableCollection<ProductBasketPosition> _productBasketPositions;
        #endregion

        #region Свойства
        public ObservableCollection<ProductBasketPosition> ProductBasketPositions
        {
            get => _productBasketPositions;
            private set;
        }

        public float TotalBasketPrice
        {
            get
            {
                float total = 0;
                foreach (var position in ProductBasketPositions)
                    total += position.TotalPrice;

                return total;
            }

            set;
        }
        #endregion 

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProductBasketPosition item in e.NewItems)
                {
                    item.PropertyChanged += OnBasketPositionPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ProductBasketPosition item in e.OldItems)
                {
                    item.PropertyChanged -= OnBasketPositionPropertyChanged;
                }
            }

            OnPropertyChanged(nameof(TotalBasketPrice));
            OnPropertyChanged(nameof(ProductBasketPositions));
        }

        private void OnBasketPositionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductBasketPosition.ProductQuantity) ||
                e.PropertyName == nameof(ProductBasketPosition.Price))
            {
                OnPropertyChanged(nameof(TotalBasketPrice));
            }
        }
        #endregion

        public UserBasket()
        {
            _productBasketPositions = new ObservableCollection<ProductBasketPosition>();
            _productBasketPositions.CollectionChanged += OnCollectionChanged;

            InitializeProductBasketPositions();

            InitializeComponent();
            DataContext = this;
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
                            BasketPositionId = position.BasketPositionId,
                            ProductId = position.ProductId,
                            ImageUrl = result.ImageUrl,
                            ProductTitle = result.ProductTitle,
                            ProductQuantity = position.ProductQuantity,
                            Price = result.Price,
                        });
                    }
                    else
                    {
                        string error = await ResponseErrorHandler.ProcessErrors(response);
                        MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void deleteBasketPosition_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (sender is TextBlock textBlock && textBlock.DataContext is ProductBasketPosition position)
            {
                try
                {
                    var deletePositionModel = new
                    {
                        basketPositionId = position.BasketPositionId
                    };

                    var jsonData = JsonSerializer.Serialize(deletePositionModel);
                    var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, $"Basket/{SessionContext.UserId}/delete_position")
                    {
                        Content = content
                    };

                    using HttpResponseMessage response = await BaseHttpClient.httpClient.SendAsync(message);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadFromJsonAsync<Basket>();
                        SessionContext.UserBasket = jsonResponse;

                        ProductBasketPositions.Remove(position);

                        int count = 0;
                        foreach (BasketPosition bp in SessionContext.UserBasket.Positions)
                            count += bp.ProductQuantity;

                        MainWindowClass.mainWindow.BasketCount = count;
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

        private void makeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MakeOrderPage(TotalBasketPrice, ProductBasketPositions.ToList()));
        }
    }
}
