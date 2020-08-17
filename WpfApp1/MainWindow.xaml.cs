using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using AngleSharp;
using RyanExample;
using System.Linq;
using WpfApp1.Libarary;
using System.Net;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Text.Encodings.Web;
using System.Web;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btn_RunClick(object sender, RoutedEventArgs e)
        {
            var CookieContainer = new CookieContainer();
            HttpClientHandler httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            httpHandler.UseCookies = true;
            httpHandler.CookieContainer = CookieContainer;

            HttpClient httpClient = new HttpClient(httpHandler);
            //Libarary.Use_HttpClient.SetHeader(ref httpClient);
            HttpResponseMessage httpResMsg = new HttpResponseMessage();

            string url_Login = "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx";
            string url_ValCodeImage = "https://amis.afa.gov.tw/CreateValidationCodeImage.aspx";

            //string url_Login = "http://127.0.0.1:81/";
            string FormData_Name_SupplyNo = "ctl00$contentPlaceHolder$txtSupplyNo";
            string FormData_Name_Password = "ctl00$contentPlaceHolder$txtPassword";
            string FormData_Name_ValCode = "ctl00$contentPlaceHolder$txtValCode";
            string ValCode = string.Empty;
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();

            try
            {
                #region -- 取得登入Post的資料組合 --
                //網頁html
                httpResMsg = await Use_HttpClient.Get_HtmlDocAsync(httpClient, url_Login);
                if (httpResMsg.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Url url = new Url(url_Login);
                    List<Cookie> cookies = CookieContainer.GetCookies(url).Cast<Cookie>().ToList();
                    throw new Exception("886");
                }

                string resResult1 = httpResMsg.Content.ReadAsStringAsync().Result;
                nameValueCollection = await Crawler.Todo_AngleSharp.GetHtmlDocument(resResult1, "form input");
                if (nameValueCollection.Count <= 0)
                {
                    throw new Exception("886");
                }
                #endregion

                #region -- 取得認證碼 --
                httpResMsg = await Use_HttpClient.Get_ImgCodeAsync(httpClient, url_ValCodeImage);
                if (httpResMsg.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("886");
                }
                byte[] image = await httpResMsg.Content.ReadAsByteArrayAsync();
                ValCode = Crawler.Todo_Tesseract3_3.GetImgText(image);
                lbl_Test.Content = ValCode;
                #endregion

                #region -- Post資料填入 --
                DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_SupplyNo, txt_SupplyNo.Text);
                DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_Password, txt_Password.Text);
                DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_ValCode, ValCode);
                nameValueCollection.Add(new KeyValuePair<string, string>(WebUtility.UrlEncode("__ASYNCPOST"), WebUtility.UrlEncode("true")));
                nameValueCollection.Add(new KeyValuePair<string, string>(WebUtility.UrlEncode("ctl00$ScriptManager_Master"), WebUtility.UrlEncode("ctl00$contentPlaceHolder$UpdatePanel1|ctl00$contentPlaceHolder$btnLogin")));
                //DataCollation.ListItemReplace(ref nameValueCollection, "abc", "1111111111111");
                #endregion

                httpResMsg = await Use_HttpClient.Post_LoginAsync(httpClient, url_Login, nameValueCollection);

                //httpResMsg = await httpClient.PostAsync(url_Login, new FormUrlEncodedContent(nameValueCollection));

                if (httpResMsg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string resResult2 = httpResMsg.Content.ReadAsStringAsync().Result;
                    lbl_InfoMsg.Content = resResult2;
                }

                //string text_Url = "https://amis.afa.gov.tw/coop1/CoopVegSupplierTransInfoQuery.aspx";
                //httpResMsg = await Use_HttpClient.Get_HtmlDocAsync(httpClient, text_Url);
                //if (httpResMsg.StatusCode == System.Net.HttpStatusCode.OK)
                //{
                //    string resResult2 = httpResMsg.Content.ReadAsStringAsync().Result;
                //}

            }
            catch (Exception ex)
            {
                string a = string.Empty;
            }
            finally
            {
                httpResMsg.Dispose();
                httpClient.Dispose();
            }
        }

        private async void btn_CloseClick(object sender, RoutedEventArgs e)
        {
            string ValCode;
            string resHtml;
            HttpWebRequest request;
            IList<KeyValuePair<string, string>> nameValueCollection;
            CookieContainer cookieContainer = new CookieContainer();
            string FormData_Name_SupplyNo = "ctl00$contentPlaceHolder$txtSupplyNo";
            string FormData_Name_Password = "ctl00$contentPlaceHolder$txtPassword";
            string FormData_Name_ValCode = "ctl00$contentPlaceHolder$txtValCode";

            #region -- 登入帳號Flow --

            #region -- 1.Post登入資料組成 --

            #region -- A.導覽登入頁面，並且取得相關Cookie資訊 以及 組成Post資料相關資訊的物件 --

            request = (HttpWebRequest)HttpWebRequest.Create("https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx");
            request.CookieContainer = cookieContainer;
            //set the user agent and accept header values, to simulate a real web browser
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            //SET AUTOMATIC DECOMPRESSION
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);
            nameValueCollection = await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "form input");

            #endregion

            #region -- B.取得驗證碼 --

            request = (HttpWebRequest)HttpWebRequest.Create("https://amis.afa.gov.tw/CreateValidationCodeImage.aspx");
            request.CookieContainer = cookieContainer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var img_byte = Use_WebClient.Get_ResponseImg(ref request, 4);
            ValCode = Crawler.Todo_Tesseract3_3.GetImgText(img_byte);
            ValCode = (ValCode.Length > 4) ? ValCode.Substring(0, 4) : ValCode;

            #endregion

            #region -- C.填寫帳號登入等相關資訊 --

            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_SupplyNo, txt_SupplyNo.Text);
            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_Password, txt_Password.Text);
            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_ValCode, ValCode);
            //__ASYNCPOST 以及 ctl00$ScriptManager_Master無法從html得知，所以才用手動寫入的方式填寫
            nameValueCollection.Add(new KeyValuePair<string, string>("__ASYNCPOST", "true"));
            nameValueCollection.Add(new KeyValuePair<string, string>("ctl00$ScriptManager_Master", "ctl00$contentPlaceHolder$UpdatePanel1|ctl00$contentPlaceHolder$btnLogin"));

            #endregion

            #endregion

            #region -- 2.登入Post請求發送 --

            request = (HttpWebRequest)HttpWebRequest.Create("https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx");
            request.CookieContainer = cookieContainer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
            request.Accept = "*/*";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            string data = string.Join("&", nameValueCollection.Select(x => string.Format("{0}={1}", WebUtility.UrlEncode(x.Key), WebUtility.UrlEncode(x.Value))));
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            Use_WebClient.WriteFormDataIntoRequest(ref request, bytes);
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);
            resHtml = WebUtility.UrlDecode(resHtml);

            #endregion

            #endregion

            #region -- 取得水果相關資訊 --

            #region -- 1.進入登入後的頁面 --

            request = (HttpWebRequest)HttpWebRequest.Create("https://amis.afa.gov.tw/coop1/CoopVegSupplierTransInfoQuery.aspx");
            request.CookieContainer = cookieContainer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);

            #endregion

            #endregion

        }

        public class CookieAwareWebClient : WebClient
        {
            public void Login(string loginPageAddress, NameValueCollection loginData)
            {
                CookieContainer container;

                var request = (HttpWebRequest)WebRequest.Create(loginPageAddress);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                var query = string.Join("&",
                  loginData.Cast<string>().Select(key => $"{key}={loginData[key]}"));

                var buffer = Encoding.ASCII.GetBytes(query);
                request.ContentLength = buffer.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                container = request.CookieContainer = new CookieContainer();

                var response = request.GetResponse();
                response.Close();
                CookieContainer = container;
            }

            public CookieAwareWebClient(CookieContainer container)
            {
                CookieContainer = container;
            }

            public CookieAwareWebClient()
              : this(new CookieContainer())
            { }

            public CookieContainer CookieContainer { get; private set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.CookieContainer = CookieContainer;
                return request;
            }
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            var loginAddress = "https://github.com/login";
            var loginData = new NameValueCollection
            {
              { "login", "ryan511238@gmail.com" },
              { "password", "xd072597ya" }
            };

            var client = new CookieAwareWebClient();
            client.Login(loginAddress, loginData);
        }
    }
}
