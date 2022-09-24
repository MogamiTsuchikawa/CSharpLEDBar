using nanoFramework.WebServer;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Diagnostics;
using System.Net;

namespace CSharpLEDBar
{
    public class IOController
    {
        private static GpioController _controller = new();
        private enum LEDKind
        {
            Red=21,Yellow=22,Green=23,Other=0
        }
        private static int[] leds = new int[] { (int)LEDKind.Red, (int)LEDKind.Yellow, (int)LEDKind.Green };

        [Route("/")]
        public void ChangeIOStatus(WebServerEventArgs e)
        {
            try
            {
                foreach (int i in leds)
                {
                    if (_controller.IsPinOpen(i)) continue;
                    _controller.OpenPin(i, PinMode.Output);
                }
                string rawUrl = e.Context.Request.RawUrl.TrimStart('/');
                Debug.WriteLine(rawUrl);
                string[] qs = rawUrl.Split('?');
                if(qs.Length != 2)
                {
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.Unauthorized);
                    return;
                }
                string[] querys = qs[1].Split('=');
                if (querys.Length != 2)
                {
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadGateway);
                    return;
                }
                string ledArg = querys[1];
                LEDKind ledKind = 
                    ledArg == "red" ? 
                    LEDKind.Red : 
                    ledArg == "yellow" ? 
                    LEDKind.Yellow : 
                    ledArg == "green" ? 
                    LEDKind.Green : 
                    LEDKind.Other;
                if(ledKind == LEDKind.Other)
                {
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadGateway);
                    return;
                }
                bool result = _controller.Read((int)ledKind) == PinValue.High;
                e.Context.Response.ContentType = "application/json";
                string json = "";
                string httpMethod = e.Context.Request.HttpMethod.ToUpper();
                switch (httpMethod)
                {
                    case "GET":
                        json = $"{{\"ledKind\":\"{ledKind.ToString()}\",\"ledStatus\":{result.ToString().ToLower()} }}";
                        WebServer.OutPutStream(e.Context.Response, json);
                        break;
                    case "POST":
                        _controller.Write((int)ledKind, PinValue.High);
                        json = $"{{\"ledKind\":\"{ledKind.ToString()}\",\"ledStatus\":true }}";
                        WebServer.OutPutStream(e.Context.Response, json);
                        break;
                    case "DELETE":
                        _controller.Write((int)ledKind, PinValue.Low);
                        json = $"{{\"ledKind\":\"{ledKind.ToString()}\",\"ledStatus\":false }}";
                        WebServer.OutPutStream(e.Context.Response, json);
                        break;
                    default:
                        WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                        return;
                }

            }catch(Exception ex)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.InternalServerError);
                return;
            }
        }
    }
}
