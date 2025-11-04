using ImperialSanWPF.Models;
using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ImperialSanWPF.Utils
{
    public class CurrentPageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightBlue : Brushes.Transparent;
        }

         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class CurrentPageFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class EmptyCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType().IsGenericType == true)
            {
                var countProperty = value.GetType().GetProperty("Count");
                if (countProperty != null)
                {
                    int count = (int)countProperty.GetValue(value);
                    bool invert = parameter?.ToString() == "Invert";

                    if (!invert)
                        return count > 0 ? Visibility.Visible : Visibility.Collapsed;

                    return count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class OrderStatmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToVisibilityInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return (bool)value ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToEyeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return (bool)value ? PackIconKind.EyeOff : PackIconKind.Eye;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StockAvailabilityColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is int inStock &&
                values[1] is int inBasket)
            {
                return inBasket > inStock
                    ? Brushes.Red
                    : Brushes.Green;
            }

            return Brushes.Green;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return (string)value == "Admin" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class RoleEnableStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return (string)value == "Admin" ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class EditModeToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Редактирование товара" : "Добавление товара";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class EditModeToButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Отредактировать" : "Добавить";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class EditModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class OrderStatusToEnableStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "В обработке" ? true : false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
