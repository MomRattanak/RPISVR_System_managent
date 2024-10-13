using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Village_Info
    {
        public int V_ID { get; set; }
        public string VL_ID { get; set; }
        public string Village_Name_KH { get; set; }
        public string Village_Name_EN { get; set; }
        public int Province_ID { get; set; }
        public int District_ID { get; set; }
        public int Commune_ID { get; set; }
        public string Village_In_Comm {  get; set; }
        public string Village_In_Dis { get; set; }
        public string Village_In_Pro { get; set; }
        public int SelectedCommune_InVill { get; set; }
        public int SelectedDistrict_InVill { get; set; }
        public int SelectedProvince_InVill { get; set; }

    }
}
