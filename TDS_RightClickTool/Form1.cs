using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Miner.Framework.Engine;
using Miner.Interop;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ESRI.ArcGIS.ArcMapUI;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using ESRI.ArcGIS.Geometry;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Windows.Forms;
using System.Net;
using ESRI.ArcGIS.Framework;
namespace TDS_RightClickTool
{
    public partial class Form1 : Form
    {
        string _cabinet = "";
        public Form1(string cabinet)
        {
            InitializeComponent();
            _cabinet = cabinet;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void BlinkGeometry(IGeometry geom, int red, int green, int blue, int size, int interval, int flashTimes)
        {
            try
            {
                IMMArcGISRuntimeEnvironment rte = new Miner.Framework.ArcGISRuntimeEnvironment();
                IMMMapUtilities mmUtils = new mmMapUtilsClass();
                IRgbColor rgbCol = new ESRI.ArcGIS.Display.RgbColorClass();
                rgbCol.Red = 255;
                rgbCol.Blue = 200;
                mmUtils.FlashGeometry(geom, rte.ActiveView.ScreenDisplay, rgbCol, size, (short)interval, (short)flashTimes);
            }
            catch (Exception ex)
            {
                //_log.Error("ERROR - ", ex);
            }
        }
        private void btnViewPorts_Click(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
            System.Diagnostics.Process.Start(ConfigHelper.GetStringValue("PathToChromeWebBrowser"),
            "file:///" + ConfigHelper.GetStringValue("PathToMetasolveHTMLFile"));   
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }
        private int lastZoomID = -1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                // create a new instance of WebClient
                WebClient client = new WebClient();

                // set the user agent to IE6
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");

                // actually execute the GET request
                string ret = client.DownloadString(ConfigHelper.GetStringValue("RestEndPointForZoomOperation"));
                MetaSolveZoomData mszd = JsonConvert.DeserializeObject<MetaSolveZoomData>(ret);
                if (mszd == null)
                {
                    return;
                }
                if (mszd.RandomID != lastZoomID)
                {
                    DateTime dtRequest = mszd.RequestedTime;
                    DateTime dtNow = DateTime.Now;
                    TimeSpan ts = dtNow - dtRequest;
                    //If it has been more than 10 seconds, this request is stale
                    if (Math.Abs(ts.TotalSeconds) > 10)
                    {
                        return;
                    }
                    lastZoomID = mszd.RandomID;
                    Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                    System.Object obj = Activator.CreateInstance(t);
                    ESRI.ArcGIS.Framework.IApplication pApp = obj as ESRI.ArcGIS.Framework.IApplication;
                    IMxDocument mxdoc = pApp.Document as IMxDocument;
                    IMap map = mxdoc.FocusMap;
                    string pedestal = mszd.Pedestal;
                    IEnumLayer enlyr = map.get_Layers(null, true);
                    enlyr.Reset();
                    ILayer ly = enlyr.Next();
                    while (ly != null)
                    {
                        if (ly.Name.ToUpper() == ConfigHelper.GetStringValue("PedestalLayerName").ToUpper() )
                        {
                            IFeatureLayer fl = (IFeatureLayer)ly;
                            IFeatureClass fc = fl.FeatureClass;
                            IQueryFilter qf = new QueryFilterClass();
                            qf.WhereClause = ConfigHelper.GetStringValue("FieldOnLayerHavingMetaSolvId") + " = '" + pedestal + "'";
                            //MessageBox.Show("Where clause: " + qf.WhereClause);
                            IFeatureCursor feCur = fc.Search(qf, false);
                            IFeature fe = feCur.NextFeature();
                            IPoint pnt = fe.ShapeCopy as IPoint;
                            Marshal.FinalReleaseComObject(feCur);
                            IEnvelope env = new EnvelopeClass();
                            env.PutCoords(pnt.X - 100, pnt.Y - 100, pnt.X + 100, pnt.Y + 100);
                            env.SpatialReference = map.SpatialReference;
                            //env.Width = 200;
                            //env.Height = 200;
                            //env.CenterAt(pnt);

                            mxdoc.ActiveView.Extent = env;
                            mxdoc.ActiveView.Refresh();
                            Application.DoEvents();
                            BlinkGeometry(pnt, 255, 0, 255, 20, 100, 10);
                        }
                        ly = enlyr.Next();
                    }


                }

            }
            catch (WebException we)
            {
                timer1.Enabled = false;
                // WebException.Status holds useful information
                Console.WriteLine(we.Message + "\n" + we.Status.ToString());
            }
            catch (NotSupportedException ne)
            {
                // other errors
                Console.WriteLine(ne.Message);
            }
            finally
            {
                timer1.Enabled = true;
                if (this.Visible == false)
                {
                    timer1.Enabled = false;
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                // create a new instance of WebClient
                WebClient client = new WebClient();

                // set the user agent to IE6
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");
                try
                {
                    // actually execute the GET request
                    string ret = client.DownloadString(ConfigHelper.GetStringValue("RestEndPointForMetaSolveOperation"));
                    List<MetaSolveData> msds = JsonConvert.DeserializeObject<List<MetaSolveData>>(ret);
                    string textBoxText =
                    @"" + _cabinet + "\r\n" +
                     "Inservice Ports: INSERVICE  \r\n" +
                     "Unassigned Ports: UNAVAILABLE \r\n" +
                     "Pending Install: OTHER \r\n ";
                    int inserviceCount = 0;
                    int otherCount = 0;
                    int unavailableCount = 0;
                    foreach (MetaSolveData msd in msds)
                    {
                        if (msd.Status == "Inservice")
                        {
                            inserviceCount++;
                        }
                        else if (msd.Status == "Unassigned")
                        {
                            unavailableCount++;
                        }
                        else
                        {
                            otherCount++;
                        }
                        string newText = textBoxText.Replace("INSERVICE", inserviceCount.ToString());
                        newText = newText.Replace("UNAVAILABLE", unavailableCount.ToString());
                        newText = newText.Replace("OTHER", otherCount.ToString());
                        textBox1.Text = newText;
                        this.Refresh();
                        System.Threading.Thread.Sleep(7);
                        Application.DoEvents();
                    }
                }
                catch (WebException we)
                {
                    // WebException.Status holds useful information
                    Console.WriteLine(we.Message + "\n" + we.Status.ToString());
                }
                catch (NotSupportedException ne)
                {
                    // other errors
                    Console.WriteLine(ne.Message);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                timer1.Enabled = false;
            }
        }
    }
    public class MetaSolveZoomData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Pedestal { get; set; }
        public DateTime RequestedTime { get; set; }
        public int RandomID { get; set; }
    }
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
