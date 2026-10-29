using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Model
{
    public class RailwayZone : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int Id { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public string Headquarters { get; set; }

        public string DisplayText => $"{ZoneCode} - {ZoneName}";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

