using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class CustomDatePickerViewModel : INotifyPropertyChanged
    {
        public CustomDatePickerViewModel()
        {
            // Populate Days and Years
            for (int i = 1; i <= 31; i++) Days.Add(i); // Days 1-31
            for (int i = DateTime.Now.Year - 50; i <= DateTime.Now.Year + 50; i++) Years.Add(i); // Years from 50 years ago to 50 years in the future

            // Default to today's date
            SelectedDay = DateTime.Now.Day;
            SelectedYear = DateTime.Now.Year;
            SelectedKhmerMonth = KhmerMonths[DateTime.Now.Month - 1];
        }
        //
        // Khmer month names
        public List<string> KhmerMonths { get; } = new List<string>
    {
        "មករា", "កម្ភះ", "មីនា", "មេសា", "ឧសភា", "មិថុនា",
        "កក្កដា", "សីហា", "កញ្ញា", "តុលា", "វិច្ឆិកា", "ធ្នូ"
    };

        // Days (1-31)
        public List<int> Days { get; } = new List<int>();
        public List<int> Years { get; } = new List<int>();

        private string _selectedKhmerMonth;
        public string SelectedKhmerMonth
        {
            get => _selectedKhmerMonth;
            set
            {
                if (_selectedKhmerMonth != value)
                {
                    _selectedKhmerMonth = value;
                    OnPropertyChanged(nameof(SelectedKhmerMonth));
                    UpdateSelectedDate();
                }
            }
        }

        private int _selectedDay;
        public int SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged(nameof(SelectedDay));
                    UpdateSelectedDate();
                }
            }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    UpdateSelectedDate();
                }
            }
        }

        // Full selected date property
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            private set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }
        // Updates the SelectedDate property based on the selected day, month, and year
        private void UpdateSelectedDate()
        {
            if (!string.IsNullOrEmpty(SelectedKhmerMonth) && SelectedDay > 0 && SelectedYear > 0)
            {
                int month = KhmerMonths.IndexOf(SelectedKhmerMonth) + 1; // Convert Khmer month to index
                SelectedDate = new DateTime(SelectedYear, month, SelectedDay);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
