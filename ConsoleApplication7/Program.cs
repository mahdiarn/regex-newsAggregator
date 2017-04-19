using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;
namespace HelloWorldApplication
{
    class HelloWorld
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private extern static bool InternetGetConnectedState(ref InternetConnectionState_e lpdwFlags, int dwReserved);

        [Flags]
        enum InternetConnectionState_e : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        // Return true or false if connecting through a proxy server
        public static bool connectingThroughProxy()
        {
            InternetConnectionState_e flags = 0;
            InternetGetConnectedState(ref flags, 0);
            bool hasProxy = false;

            if ((flags & InternetConnectionState_e.INTERNET_CONNECTION_PROXY) != 0)
            {
                hasProxy = true;
            }
            else
            {
                hasProxy = false;
            }

            return hasProxy;
        }

        static void Main(string[] args)
        {
            String titlelow = "Test1";
            String descriptionlow = "Test2";
            string url = "http://rss.detik.com/index.php/detikcom";

            //membaca kata kunci
            string input = Console.ReadLine();

            Console.WriteLine();

            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.UserAgent =
                "Googlebot/1.0 (googlebot@googlebot.com http://googlebot.com/)";

            // WWB: Use The Default Proxy
            if (connectingThroughProxy())
            {
                String username = Console.ReadLine();
                String password = Console.ReadLine();
                // WWB: Use The Default Proxy
                WebProxy myproxy = new WebProxy("http://cache.itb.ac.id:8080", false);
                httpWebRequest.Proxy = myproxy;

                // WWB: Use The Thread's Credentials (Logged In User's Credentials)
                if (httpWebRequest.Proxy != null)
                    httpWebRequest.Proxy.Credentials = new NetworkCredential(username, password);

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        using (XmlReader reader = XmlReader.Create(responseStream))
                        {
                            SyndicationFeed feed = SyndicationFeed.Load(reader);
                            reader.Close();
                            foreach (SyndicationItem item in feed.Items)
                            {
                                titlelow = item.Title.Text.ToLower();
                                descriptionlow = item.Summary.Text.ToLower();
                                if (Regex.IsMatch(titlelow, input) || Regex.IsMatch(descriptionlow, input))
                                {
                                    Console.WriteLine(item.Title.Text + "\n");
                                    Console.WriteLine(item.Summary.Text + "\n");
                                    Console.WriteLine(item.Id + "\n");
                                }
                            }
                            Console.ReadKey();
                        }
                    }
                }
            }
            else
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        using (XmlReader reader = XmlReader.Create(responseStream))
                        {
                            SyndicationFeed feed = SyndicationFeed.Load(reader);
                            reader.Close();
                            foreach (SyndicationItem item in feed.Items)
                            {
                                titlelow = item.Title.Text.ToLower();
                                descriptionlow = item.Summary.Text.ToLower();
                                if(Regex.IsMatch(titlelow,input) || Regex.IsMatch(descriptionlow,input))
                                {
                                    Console.WriteLine(item.Title.Text + "\n");
                                    Console.WriteLine(item.Summary.Text + "\n");
                                    Console.WriteLine(item.Id + "\n");
                                }                               
                            }
                            Console.ReadKey();
                        }
                    }
                }
                Console.ReadKey();
            }
        }
    }
}