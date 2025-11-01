using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImperialSanWPF.Utils;

namespace ImperialSanWPF.Models
{
    public class Order : INotifyPropertyChanged
    {
        public int UserId { get; set; } = SessionContext.UserId;
        public string? UserComment { get; set; } = null;

        private string? _diliveryAddress = null;
        private string? _paymentMethod = null;

        public string? DiliveryAddress
        {
            get => _diliveryAddress;

            set
            {
                if (value != _diliveryAddress)
                {
                    _diliveryAddress = value;
                    OnPropertyChanged();
                }
            }
        }
        public string? PaymentMethod
        {
            get => _paymentMethod;

            set
            {
                if (value != _paymentMethod)
                {
                    _paymentMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
