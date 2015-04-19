using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDS_MetaSolve
{
    public class MetaSolveData
    {
        public String Exchange { get; set; }
        #region getters and setters for 18 other properties...
        public String CableName { get; set; }
        public String FiberPair { get; set; }
        public String Remarks { get; set; }
        public String Status { get; set; }
        public String CableType { get; set; }
        public String DSANum { get; set; }
        public String AccessPoint { get; set; }
        public String Route { get; set; }
        public String Pedestal { get; set; }
        public String ApAddress { get; set; }
        public String Map { get; set; }
        public String ConditionCode { get; set; }
        public String Comments { get; set; }
        public String CircuitID { get; set; }
        public String CircuitDesignID { get; set; }
        public String EULName { get; set; }
        public String StreetAddress { get; set; }
        public String AddlInfo { get; set; }
        public String CLLICode { get; set; }
        public Double X { get; set; }
        public Double Y { get; set; }
        #endregion
    }
}
