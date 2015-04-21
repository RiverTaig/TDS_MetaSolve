using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json.Serialization;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;


//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace TDS_SOE
{
    [ComVisible(true)]
    [Guid("8e31fce4-d47f-45e4-a8f8-9e75b273eb4a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",//use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "",
        DisplayName = "TDS_SOE",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class TDS_SOE : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string soe_name;
        
        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        private ServerLogger logger;
        private IRESTRequestHandler reqHandler;

        public TDS_SOE()
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            configProps = props;
        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new RestResource(soe_name, false, RootResHandler);

            RestOperation MetaSolvOper = new RestOperation("MetaSolveOperation",
                                                      new string[] { "CableName", "FiberName" },
                                                      new string[] { "json" },
                                                      GetMetaSolveDataOperHandler);

            rootRes.operations.Add(MetaSolvOper);

            RestOperation SetZoomOper = new RestOperation("SetMetaSolveZoomOperation",
                                          new string[] { "Pedestal","FieldToQuery", "FeatureClassName" },
                                          new string[] { "json" },
                                          SetMetaSolveZoomOperHandler);

            RestOperation GetZoomOper = new RestOperation("GetMetaSolveZoomOperation",
                              new string[] { "Ignored" },
                              new string[] { "json" },
                              GetMetaSolveZoomOperHandler);

            rootRes.operations.Add(SetZoomOper);
            rootRes.operations.Add(GetZoomOper);

            return rootRes;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            JsonObject result = new JsonObject();
            result.AddString("h ello", "world");

            return Encoding.UTF8.GetBytes(result.ToJson());
        }
        
        private byte[] SetMetaSolveZoomOperHandler(NameValueCollection boundVariables,
                                  JsonObject operationInput,
                                      string outputFormat,
                                      string requestProperties,
                                  out string responseProperties)
        {
            responseProperties = null;
            string pedestal;
            bool found = operationInput.TryGetString("Pedestal", out pedestal);
            if (!found || string.IsNullOrEmpty(pedestal))
                throw new ArgumentNullException("Pedestal");

            string fieldToQuery;
            found = operationInput.TryGetString("FieldToQuery", out fieldToQuery);
            if (!found || string.IsNullOrEmpty(pedestal))
                throw new ArgumentNullException("FieldToQuery");

            string featureClassName;
            found = operationInput.TryGetString("FeatureClassName", out featureClassName);
            if (!found || string.IsNullOrEmpty(pedestal))
                throw new ArgumentNullException("FeatureClassName");


            MetaSolveZoomData mszd = new MetaSolveZoomData();
            //IFeatureClass fc = GetFeatureClass(featureClassName);
            //IQueryFilter qf = new QueryFilterClass();
            //qf.WhereClause = fieldToQuery + " = '" + pedestal + "'";
            //IFeatureCursor feCur = fc.Search(qf, false);
            //IFeature fe = feCur.NextFeature();
            //IPoint pnt = (IPoint)fe.Shape;
            double x = 0;// pnt.X;
            double y = 0;// pnt.Y;
            mszd.Pedestal = pedestal;
            mszd.X = x;
            mszd.Y = y;
            mszd.RequestedTime = DateTime.Now;
            Random rand = new Random();
            mszd.RandomID = rand.Next();
            //Marshal.FinalReleaseComObject(feCur);
            _mszd = mszd;
            String jsonString = JsonConvert.SerializeObject(_mszd);
            return Encoding.UTF8.GetBytes(jsonString);

        }
        private static MetaSolveZoomData _mszd = null;
        private byte[] GetMetaSolveZoomOperHandler(NameValueCollection boundVariables,
                                          JsonObject operationInput,
                                              string outputFormat,
                                              string requestProperties,
                                          out string responseProperties)
        {
            responseProperties = null;
            String jsonString = JsonConvert.SerializeObject(_mszd);
            return Encoding.UTF8.GetBytes(jsonString);

        }
        private byte[] GetMetaSolveDataOperHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            responseProperties = null;

            string cableNameValue;
            bool found = operationInput.TryGetString("CableName", out cableNameValue);
            if (!found || string.IsNullOrEmpty(cableNameValue))
                throw new ArgumentNullException("CableName");

            string fiberNameValue;
            found = operationInput.TryGetString("FiberName", out fiberNameValue);
            //if (!found || string.IsNullOrEmpty(fiberNameValue))
            //    throw new ArgumentNullException("FiberName");

            string csvFile = System.IO.Path.Combine(Environment.CurrentDirectory, "MetaSolvPONDistribution.csv");
            List<MetaSolveData> metaSolveDatas = new List<MetaSolveData>();
            using (StreamReader sr = File.OpenText(csvFile) )
            {
                
                while (! sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (metaSolveDatas.Count == 0) { line = sr.ReadLine(); }//Skip column headers
                    string[] lineSplit = line.Split(',');
                    MetaSolveData msd = new MetaSolveData();
                    msd.Exchange = lineSplit[0];
                    #region Set CableName,FiberPair...and all the other properties for MetaSolveData
                    msd.CableName = lineSplit[1];
                    msd.FiberPair = lineSplit[2];
                    msd.Remarks = lineSplit[3];
                    msd.Status = lineSplit[4];
                    msd.CableType = lineSplit[5];
                    msd.DSANum = lineSplit[6];
                    msd.AccessPoint = lineSplit[7];
                    msd.Route = lineSplit[8];
                    msd.Pedestal = lineSplit[9];
                    msd.ApAddress = lineSplit[10];
                    msd.Map = lineSplit[11];
                    msd.ConditionCode = lineSplit[12];
                    msd.Comments = lineSplit[13];
                    msd.CircuitID = lineSplit[14];
                    msd.CircuitDesignID = lineSplit[15];
                    msd.EULName = lineSplit[16];
                    msd.StreetAddress = lineSplit[17];
                    msd.AddlInfo = lineSplit[18];
                    msd.CLLICode = lineSplit[19];
                    msd.X = 0;
                    msd.Y = 0;
                    #endregion
                    metaSolveDatas.Add(msd);
                    //line = sr.ReadLine();
                }
            }
            String jsonString = JsonConvert.SerializeObject(metaSolveDatas);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        //private static Dictionary<string, IFeatureClass> _cachedFeatureClass = new Dictionary<string, IFeatureClass>();
        /*      public  IFeatureClass GetFeatureClass(string nameToQuery)
               {
                   IMapServer3 mapServer = (IMapServer3)serverObjectHelper.ServerObject;
                   string mapName = mapServer.DefaultMapName;
                   IMapLayerInfo layerInfo;
                   IMapLayerInfos layerInfos = mapServer.GetServerInfo(mapName).MapLayerInfos;
                   // Find the index position of the map layer to query.
                   int c = layerInfos.Count;
                   int layerIndex = 0;
                   for (int i = 0; i < c; i++)
                   {
                       try
                       {
                           layerInfo = layerInfos.get_Element(i);
                           IMapServerDataAccess dataAccess = (IMapServerDataAccess)mapServer;
                           IFeatureClass fc = (IFeatureClass)dataAccess.GetDataSource(mapName, i);
                           string fcName = ((IDataset)fc).Name;
                           if (fcName.ToUpper() == nameToQuery.ToUpper())
                           {
                               return fc;
                           }
                       }
                       catch { }
                   }
                   return null;
               }*/

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
