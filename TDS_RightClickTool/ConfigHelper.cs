using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace TDS_RightClickTool
{
    public class ConfigHelper
    {
        public static string GetStringValue(string property)
        {
            /*PedestalLayerName|PatchLocation
            FieldOnLayerHavingMetaSolvId|Comments
            PathToMetasolveHTMLFile|C:/mm_tech_marketing/tds/Metasolv.html
            PathToChromeWebBrowser|C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
            RestEndPointForMetaSolveOperation|http://localhost:6080/arcgis/rest/services/TDS/MapServer/exts/TDS_SOE/MetaSolveOperation?CableName=a&FiberName=b&f=pjson
            RestEndPointForZoomOperation|http://localhost:6080/arcgis/rest/services/TDS/MapServer/exts/TDS_SOE/GetMetaSolveZoomOperation?Ignored=sdfg&f=pjson*/
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.ToUpper().Replace("FILE:///","");
            FileInfo fi = new FileInfo(path);
            string dir = fi.DirectoryName;
            string[] props = File.ReadAllLines(dir + "\\TDSMetaSolvConfig.txt");
            foreach (string prop in props)
            {
                string[] propValPair = prop.Split('|');
                if (propValPair[0] == property)
                {
                    return propValPair[1];
                }
            }
            return "";
        }
    }
}
