using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TDS_MetaSolve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MetaSolveWebService.PonDistributionClient pdc = new MetaSolveWebService.PonDistributionClient();
            string metaSolveData =  pdc.GetData("FDHAJ", 2);
            //MessageBox.Show ("river " + metaSolveData);
            string[] splitMetaSolveData = metaSolveData.Split('~');
            //skip the first
            StringBuilder jsArray1 = new StringBuilder();
            
            jsArray1.Append("var metaSolve1 = [");
//int splitMarker = 200; //works with 200 & 96, but breaks at 97
//int numInGroup2 = 95;
for (int i = 0; i < splitMetaSolveData.Length; i++)
            {
                string line = splitMetaSolveData[i];
                if (i > 0)
                {
                    jsArray1.Append(",");
                }
                jsArray1.Append("'" + line + "'");
            }
            jsArray1.Append("];");
            _jsArray1 = jsArray1.ToString();
            /*StringBuilder jsArray2 = new StringBuilder();
            jsArray2.Append("var metaSolve2 = [");
            for (int i = splitMarker ; i < splitMarker  + numInGroup2; i++)
            {

                    string line = splitMetaSolveData[i];
                    if (i > splitMarker)
                    {
                        jsArray2.Append(",");
                    }
                    jsArray2.Append("'" + line + "'");
                
            }
            jsArray2.Append("];");   

            _jsArray1 = jsArray1.ToString();
            //MessageBox.Show(_jsArray1);
            System.Diagnostics.Trace.WriteLine(_jsArray1);
            _jsArray2 = jsArray2.ToString(); */
            _jsArray2 = "[];";
            //MessageBox.Show(_jsArray2);
            System.Diagnostics.Trace.WriteLine(_jsArray2);
            MessageBox.Show("done");
            //_jsArray = "var cars = ['Saab','Volvo','BMW'];";
            //int len = _jsArray.Length;
            //MessageBox.Show(len.ToString());
        }
        private int x = 0;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
        private string _jsArray1 = "";
        private string _jsArray2 = "";
        private void button2_Click(object sender, EventArgs e)
        {
            
            string html = @"
                <!DOCTYPE html>
                <html>
                <script>

                </script>
                <body onload='myFunction()'>
                <canvas id='myCanvas' width='437'  height='581' style='border:1px solid #d3d3d3;'>
                Your browser does not support the HTML5 canvas tag.</canvas>
                <script>" +
                    "function myFunction(){try{" +
                    _jsArray1 + ";" + _jsArray2 +
                    ";" +
                    @"var c = document.getElementById('myCanvas');
                    c.addEventListener('mousedown', function(evt) {
                    alert(metaSolve1[0]);
                    }, false);
                    c.addEventListener('mousemove', function(evt) {
                    var mousePos = getMousePos(c, evt);
                    var message = 'Mouse position: ' + mousePos.x + ',' + mousePos.y;

                    var canvasWidth = document.getElementById('myCanvas').width - 5;
                    var canvasHeight = document.getElementById('myCanvas').height - 5;
                    var columnWidth = (canvasWidth / 18)   ;
                    var rowHeight = (canvasHeight / 24)   ;
                    var col = Math.round(mousePos.x / columnWidth,0);
                    var row = Math.round(mousePos.y / rowHeight,0);
                    var portNumber = ((row - 1) * 18 ) + col;
//alert('mm');
//alert(portNumber);
                    message = 'Port: ' + portNumber;
if(portNumber < 5000){                    
document.getElementById('dynamicContent').innerHTML = message + ' ' + metaSolve1[portNumber];
}
else
{
document.getElementById('dynamicContent').innerHTML = '2: ' + message + ' ' + metaSolve2[portNumber-290];
}
                    //alert( message);
                    }, false);
                    function getMousePos(canvas, evt) {
                        return {
                            x: evt.clientX ,
                            y: evt.clientY 
                            };
                    }
                    var ctx = c.getContext('2d');
                    var counter = 0;
                    for(c = 0; c < 24; c++){
                        for(r=0; r<18; r++){
                            counter ++;
                            ctx.beginPath();
                            var x = 5 + (r * 24);
                            var y = 5 + (c*24);
                            //ctx.arc(X,20 + (c*30),10,0,2*Math.PI);
                            ctx.rect(x, y, 20,20);
                            var num = Math.random()
                            if(num > .5){
                                ctx.fillStyle = 'yellow';
                            }
                            else
                            {
                                ctx.fillStyle = '#cccccc';
                            }
                            ctx.fill();
                            ctx.lineWidth = 1;
                            ctx.strokeStyle = '#000000';
                            ctx.stroke();
                            ctx.fillStyle = 'black';
                            ctx.font = 'bold 10px Arial';
                            var offset = 2;
                            if(counter < 200) {offset = 1;} 
                            if(counter < 100) {offset = 4;}                            
                            if(counter < 10) {
                                offset = 7;
                            }
                            var xOff = x+offset;
                            ctx.fillText(counter, xOff, y+ 14);
                        }
                    }
                }catch(Exception){alert(Exception);}}
                </script> 
                <div id='dynamicContent'> - </div>
                </body>
                </html>";
            string filePath = @"c:\TDSHtml.html";
            File.WriteAllText(filePath,html);
            webControl1.Source = new Uri(filePath);
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
           /* $.ajax({
                url: "http://63.253.242.153/arcgis/rest/services/ArcFM_Silverlight_Schneiderville/Electric/MapServer/exts/ArcFMMapServer/Electric%20Trace?"
                + "startPoint=" + x + "," + y
                + "&traceType=Downstream&protectiveDevices=&phasesToTrace=&drawComplexEdges=&includeEdges=True&includeJunctions=True&returnAttributes=True&returnGeometries=True&tolerance=5&spatialReference=&currentStatusProgID=&f=pjson"
              }).then(function(data) {
                DrawTraceResults(data);
              });*/
        }
    }
}
