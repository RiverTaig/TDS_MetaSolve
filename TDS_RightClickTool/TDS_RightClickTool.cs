using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
using Miner.Interop;
using Miner.ComCategories;

namespace TDS_RightClickTool
{
    [ComVisible(true)]
    [Guid("410cb4b3-6580-4d77-8ea7-15788f7de802")]
    [ProgId("TDS.TDS_Right_ClickTool")]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class TDS_Right_ClickTool : IMMTreeTool
    {

        #region Private Fields

        Bitmap _bitmap = null;
        private IntPtr _hBitmap;

        #endregion

        #region Constructors / Destructors
        public TDS_Right_ClickTool()
        {
            // set the tool's bitmap. See base class for handling of bitmap to avoid memory leaks.
            try
            {
                Bitmap aBitMap = new Bitmap(GetType().Assembly.GetManifestResourceStream("TDS_RightClickTool.tdsRightClick.bmp"));
                _bitmap = aBitMap;
                _bitmap.MakeTransparent(_bitmap.GetPixel(0, 0));
                _hBitmap = _bitmap.GetHbitmap();
            }
            catch (Exception e)
            {
                MessageBox.Show("Bitmap error" + e.ToString());
            }
        }

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        ~TDS_Right_ClickTool()
        {
            // Must de-allocate the UI resources with Windows.DeleteObject
            if (_hBitmap.ToInt32() != 0)
            {
                DeleteObject(_hBitmap);
            }
        }

        #endregion

        #region IMMTreeTool Members

        public void Execute(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                pEnumItems.Reset();
                ID8GeoAssoc ga = (ID8GeoAssoc) pEnumItems.Next();
                IRow r = ga.AssociatedGeoRow;
                Form1 f1 = new Form1(r.get_Value(r.Fields.FindField( ConfigHelper.GetStringValue("FieldOnCabinetMatchingMetaSolvDataCableNameField")  )).ToString());
                f1.Show();
                //MessageBox.Show("Hello world");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ERROR " + ex.ToString());
            }
        }

        public int get_Enabled(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                int retValue = (int)(mmToolState.mmTSVisible | mmToolState.mmTSEnabled);
                return (int)(mmToolState.mmTSVisible | mmToolState.mmTSEnabled);
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0;
            }
        }

        public mmShortCutKey ShortCut
        {
            get
            {
                //return new mmShortCutKey();
                return mmShortCutKey.mmCtrlM;
            }
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public int Bitmap
        {
            get
            {
                return _hBitmap.ToInt32();
            }
        }

        public bool AllowAsDefault
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return ConfigHelper.GetStringValue("ToolName");
            }
        }

        public int Category
        {
            get
            {
                return 0;
            }
        }

        #endregion

    }
}
