using System;
using System.IO;
using System.Net;

namespace WpfApp1.Libarary
{
    class Use_WebClient
    {
        public static string Get_ResponseHtml(ref HttpWebRequest _request) 
        {
            string doc_html = string.Empty;

            using (WebResponse response = _request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    doc_html = sr.ReadToEnd();
                }
            }

            return doc_html;
        }

        public static Byte[] Get_ResponseImg(ref HttpWebRequest _request)
        {
            Byte[] ValCode;

            using (WebResponse response = _request.GetResponse())
            {
                using (BinaryReader sr = new BinaryReader(response.GetResponseStream()))
                {
                    ValCode = sr.ReadBytes(1 * 1024 * 1024 * 10);
                }
            }

            return ValCode;
        }
        public static void WriteFormDataIntoRequest(ref HttpWebRequest _request,byte[] data) 
        {
            using (Stream dataStream = _request.GetRequestStream())
            {
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
            }
        }
    }
}
