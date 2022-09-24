using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Networking;
using nanoFramework.WebServer;

namespace CSharpLEDBar
{
    public class Program
    {
        private static bool _isConnected = false;
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            try
            {
                int connectRetry = 0;
                bool success;
                CancellationTokenSource cs = new(60000);
                success = WifiNetworkHelper.ConnectDhcp(Secret.ssid,Secret.wifiPassword,requiresDateTime: true,token: cs.Token);
                if (!success)
                {
                    Debug.WriteLine($"WiFiê⁄ë±ÉGÉâÅ[:{WifiNetworkHelper.Status}");
                    return;
                }
                else
                {
                    
                }
                using(WebServer server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(IOController) }))
                {
                    server.Start();
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
            

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
