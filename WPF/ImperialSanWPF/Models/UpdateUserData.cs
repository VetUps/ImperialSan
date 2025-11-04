using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class UpdateUserData : INotifyPropertyChanged
    {
        public int UserId { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }

        private string? _deliveryAddress;
        public string? DeliveryAddress 
        {
            get => _deliveryAddress;

            set
            {
                _deliveryAddress = value;
                OnPropertyChanged();
            } 
        }
        public string? NewPassword { get; set; }

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
