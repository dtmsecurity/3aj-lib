using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _3aj_lib
{
    public class _3aj
    {
        public static string shimServer = "http://localhost:8666/3aj/";
        public static Queue<Dictionary<string, string>> queries = new Queue<Dictionary<string, string>>();
        public static List<string> queryDone = new List<string>();
        public static Dictionary<string, Dictionary<int, string>> responses = new Dictionary<string, Dictionary<int, string>>();
        public static WebServer ws;

        public static string SendResponse(HttpListenerRequest request)
        {
            if (request.Url.PathAndQuery.Contains("token="))
            {
                var getParams = request.QueryString;
                string token = getParams["token"];
                string txt = getParams["txt"];
                int pos = int.Parse(getParams["pos"]);
                if (responses.ContainsKey(token))
                {
                    responses[token][pos] = txt;
                }
                return "";
            }
            else if (request.Url.PathAndQuery.Contains("done="))
            {
                var getParams = request.QueryString;
                string token = getParams["done"];
                queryDone.Add(token);
                return "";
            }
            else if (request.Url.PathAndQuery.Contains(".json"))
            {
                if (queries.Count > 0)
                {
                    var lookup = queries.Dequeue();
                    string json = "{\"token\":\"" + lookup["token"] + "\",\"url\":\"" + lookup["url"] + "\"}";
                    responses[lookup["token"]] = new Dictionary<int, string>();
                    return json;
                }
                else
                {
                    return "{\"token\":\"nop\",\"url\":\"\"}";
                }
            }
            else
            {
                string response = "<!DOCTYPE html><head><title></title><script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script></head><body><script type=\"text/javascript\"> shimUrl = \"" + shimServer + "\";c1= 0;function run(){ c1++; shimControlUrl = shimUrl + \"3aj.json?c1=\" + c1; $.getJSON( shimControlUrl, function(obj){    token = obj[\"token\"];    url = obj[\"url\"];    if(token != \"nop\"){ n = 0;    $.getJSON( url, function( data ) {  n = 0; $.each(  data[\"Answer\"], function( key, val ) { shimUrlGet = shimUrl + \"?txt=\" + encodeURI(val[\"data\"]) + \"&pos=\" + n + \"&token=\" + token + \"&c1=\" + c1; n++; $.getJSON(shimUrlGet);}); doneUrl = shimUrl + \"?done=\" + token; $.getJSON(doneUrl); });}}); } setInterval(run, 1000); </script></body></html>";
                return response;
            }
        }
        public static Dictionary<int, string> _3ajclient(string url)
        {
            Dictionary<string, string> lookup = new Dictionary<string, string>();
            lookup["token"] = Guid.NewGuid().ToString();
            lookup["url"] = url;

            Console.WriteLine("[Request] {0}", url);

            queries.Enqueue(lookup);

            int timeout = 5000;
            while (!queryDone.Contains(lookup["token"]) && (timeout > 0))
            {
                int sleepTime = 100;
                System.Threading.Thread.Sleep(sleepTime);
                timeout -= sleepTime;
            }

            var result = new Dictionary<int, string>();
            if (responses.ContainsKey(lookup["token"]))
            {
                result = responses[lookup["token"]];
            }
            return result;
        }

        public static void _3ajinit()
        {
            ws = new WebServer(SendResponse, shimServer);
            ws.Run();
            Process p = Process.Start("IExplore.exe", shimServer);
            //Process p = Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", shimServer);
            System.Threading.Thread.Sleep(2000);
        }

        public static void _3ajclose()
        {
            ws.Stop();
        }
    }
}
