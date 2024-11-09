using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class StudentInfoService : INotifyPropertyChanged
    {
        private string _searchText_ID_Name_Insert;
        public string SearchText_ID_Name_Insert
        {
            get => _searchText_ID_Name_Insert;
            set
            {
                _searchText_ID_Name_Insert = value;
                OnPropertyChanged(nameof(SearchText_ID_Name_Insert));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
