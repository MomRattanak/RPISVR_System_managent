using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;

namespace RPISVR_Managements.Model
{
    public class Communes_Info
    {
        public int C_ID { get; set; }
        public string CM_ID { get; set; }
        public string Commune_Name_KH { get; set; }
        public string Commune_Name_EN { get; set; }
        public int Province_ID { get; set; }
        public int District_ID { get; set; }
        public string Commune_In_Dis { get;set; }
        public string Commune_In_Pro { get; set; }
        public int SelectedDistrict_Incomm {  get; set; }
        public int SelectedProvince_Incomm {  get; set; }

    }
}
