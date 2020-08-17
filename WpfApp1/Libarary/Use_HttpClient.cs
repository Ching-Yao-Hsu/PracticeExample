using AngleSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WpfApp1.Libarary
{
    class Use_HttpClient
    {
        public static void SetHeader(ref HttpClient client) 
        {
            client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            //client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=xlvt1o1do2enthbopgjovpkq");
            //client.DefaultRequestHeaders.Add("Host", "amis.afa.gov.tw");
            //client.DefaultRequestHeaders.Add("Referer", "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx");
            //client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "image");
            //client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "no-cors");
            //client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");

            HttpRequestMessage httpReq = new HttpRequestMessage();
        }

        public static async Task<HttpResponseMessage> Get_HtmlDocAsync(HttpClient client, string Url_des)
        {
            HttpResponseMessage resMsg;

            using (var httpReq = new HttpRequestMessage(HttpMethod.Get,Url_des))
            {
                httpReq.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpReq.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                httpReq.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
                httpReq.Headers.Add("Cache-Control", "max-age=0");
                httpReq.Headers.Add("Connection", "keep-alive");
                httpReq.Headers.Add("Cookie", "ASP.NET_SessionId=l5ajcid4lsz4d55awyhdlff5");
                httpReq.Headers.Add("Host", "amis.afa.gov.tw");
                httpReq.Headers.Add("Referer", "https://amis.afa.gov.tw/menu/CoopMenuVegSupplierTransInfo.aspx");
                httpReq.Headers.Add("Sec-Fetch-Dest", "document");
                httpReq.Headers.Add("Sec-Fetch-Mode", "navigate");
                httpReq.Headers.Add("Sec-Fetch-Site", "same-origin");
                httpReq.Headers.Add("Sec-Fetch-User", "?1");
                httpReq.Headers.Add("Upgrade-Insecure-Requests", "1");
                httpReq.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                resMsg = await client.SendAsync(httpReq);
            }
            
            return resMsg;
        }

        public static async Task<HttpResponseMessage> Get_ImgCodeAsync(HttpClient client, string Url_des)
        {
            HttpResponseMessage resMsg;

            using (var httpReq = new HttpRequestMessage(HttpMethod.Get, Url_des))
            {
                httpReq.Headers.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
                httpReq.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                httpReq.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
                httpReq.Headers.Add("Connection", "keep-alive");
                httpReq.Headers.Add("Cookie", "ASP.NET_SessionId=l5ajcid4lsz4d55awyhdlff5");
                httpReq.Headers.Add("Host", "amis.afa.gov.tw");
                httpReq.Headers.Add("Referer", "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx");
                httpReq.Headers.Add("Sec-Fetch-Dest", "image");
                httpReq.Headers.Add("Sec-Fetch-Mode", "no-cors");
                httpReq.Headers.Add("Sec-Fetch-Site", "same-origin");
                httpReq.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                resMsg = await client.SendAsync(httpReq);
            }

            return resMsg;
        }

        public static async Task<HttpResponseMessage> Post_LoginAsync(HttpClient client, string Url_des,IList<KeyValuePair<string,string>> data)
        {
            HttpResponseMessage resMsg;

            using (var httpReq = new HttpRequestMessage(HttpMethod.Get, Url_des))
            {
                httpReq.Headers.Add("Accept", "*/*");
                httpReq.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                httpReq.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
                httpReq.Headers.Add("Cache-Control", "no-cache");
                httpReq.Headers.Add("Connection", "keep-alive");
                httpReq.Headers.Add("Cookie", "ASP.NET_SessionId=xlvt1o1do2enthbopgjovpkq");
                httpReq.Headers.Add("Host", "amis.afa.gov.tw");
                httpReq.Headers.Add("Origin", "https://amis.afa.gov.tw");
                httpReq.Headers.Add("Referer", "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx");
                httpReq.Headers.Add("Sec-Fetch-Dest", "empty");
                httpReq.Headers.Add("Sec-Fetch-Mode", "cors");
                httpReq.Headers.Add("Sec-Fetch-Site", "same-origin");
                httpReq.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpReq.Headers.Add("X-MicrosoftAjax", "Delta=true");
                httpReq.Headers.Add("X-Requested-With", "XMLHttpRequest");
                httpReq.Content = new FormUrlEncodedContent(data);
                resMsg = await client.SendAsync(httpReq);

            }

            return resMsg;
        }

    }
}
