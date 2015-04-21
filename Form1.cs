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
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            WebClient wc = new WebClient();

            Stream data = wc.OpenRead("http://localhost:6080/arcgis/rest/services/TDS/MapServer/exts/TDS_SOE/MetaSolveOperation?CableName=asdf&FiberName=asdf&f=pjson");
            StreamReader reader = new StreamReader(data);
            string json = reader.ReadToEnd();
            _jsArray1 = json;

            List<MetaSolveData> msLines = JsonConvert.DeserializeObject<List<MetaSolveData>>(json);


        }
        private int x = 0;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
        private string _jsArray1 = "";
        private string _jsArray2 = "";
        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
              "file:///C:/test2.html");
            //string text = File.ReadAllText("c:\\test2.html");
            //webControl1.LoadHTML(text);
            return;
            string path = @"C:\test2.html"; //C:\Code\Git\TDS_MetaSolve\TDS_RightClickTool\bin\Debug";
            webControl1.Source = new Uri(path);
            return;
            string html = @"
                <!DOCTYPE html>
                <html>
                <script>
                </script>
                <body onload='myFunction()'>
                <canvas id='myCanvas' width='437'  height='581' style='border:1px solid #d3d3d3;'>
                Your browser does not support the HTML5 canvas tag.</canvas>
                <script>" +
_jsArray1 +
@"
function GetStringInArray(portNumber)
{
    //alert('In get stringin arrray: ' + metaSolve1.length);

	for(var i = 0; i < metaSolve1.length; i++)
    {
        var splitter = metaSolve1[i].split(',');
        //alert(splitter[2] + '    ' + portNumber);
        if(splitter[2] == portNumber){
            return metaSolve1[i];
        }
    }
    return 'No data for this port!';
}
"
                    + "function myFunction(){try{" +
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
                    document.getElementById('dynamicContent').innerHTML = message + ' ' + GetStringInArray(portNumber);
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

        private void Awesomium_Windows_Forms_WebControl_ShowCreatedWebView(object sender, Awesomium.Core.ShowCreatedWebViewEventArgs e)
        {
            string path = @"C:\test2.html"; //C:\Code\Git\TDS_MetaSolve\TDS_RightClickTool\bin\Debug";
            webControl1.Source = new Uri(path);
        }
    }
}
