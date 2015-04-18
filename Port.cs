using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace TDS_MetaSolve
{
    public class Port
    {
        public int ID { get; set; }
        public String Remarks { get; set; }
        public String CabinetName { get; set; }
        public int PortNumber { get; set; }
        public string Pedestal { get; set; }
        public int FPortObjectID { get; set; }
    }
}
