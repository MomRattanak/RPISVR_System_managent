using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Districts_Info
    {
        public int D_ID { get; set; }
        public string DS_ID { get; set; } 
        public string District_Name_KH { get; set; }
        public string District_Name_EN { get; set; }
        public int Province_ID { get; set; }
        public string District_In_Pro { get; set; }
        public int SelectedProvince { get; set; }
    }
}
