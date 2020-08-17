using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace WhatsappBroadcastHomeshop
{
    public class CommonService
    {
        public static string SendApiRequest(string url, string Request)
        {
            string strresponse = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (!string.IsNullOrEmpty(Request))
                        streamWriter.Write(Request);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strresponse = streamReader.ReadToEnd();
                }
            }
            catch 
            {
                
            }

            return strresponse;

        }

        public static string SendParamsApiRequest(string url, NameValueCollection Request)
        {
            string strresponse = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                wc.QueryString = Request;

                var data = wc.UploadValues(url, "POST", wc.QueryString);

                // data here is optional, in case we recieve any string data back from the POST request.
                strresponse = UnicodeEncoding.UTF8.GetString(data);
            }
            catch (Exception)
            {

                throw;
            }

            return strresponse;


        }
    }
}
