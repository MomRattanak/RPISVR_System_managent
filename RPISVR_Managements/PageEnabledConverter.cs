using Microsoft.UI.Xaml.Data;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements
{
    public class PageEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PageInfo pageInfo)
            {
                if (parameter != null && parameter.ToString() == "Previous")
                {
                    // Enable the Previous button if CurrentPage is greater than 1
                    return pageInfo.CurrentPage > 1;
                }
                else if (parameter != null && parameter.ToString() == "Next")
                {
                    // Enable the Next button if CurrentPage is less than TotalPages
                    return pageInfo.CurrentPage < pageInfo.TotalPages;
                }
            }
            return false;  // Default is disabled
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


}
