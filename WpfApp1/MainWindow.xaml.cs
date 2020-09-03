using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using RyanExample;
using System.Linq;
using WpfApp1.Libarary;
using System.Net;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WpfApp1.Data.DTO;

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
            dap_Start.SelectedDate = DateTime.Now;
            dap_End.SelectedDate = DateTime.Now;
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
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();

            try
            {
                #region -- 取得登入Post的資料組合 --
                //網頁html
                httpResMsg = await Use_HttpClient.Get_HtmlDocAsync(httpClient, url_Login);
                if (httpResMsg.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    System.Uri url = new System.Uri(url_Login);
                    List<Cookie> cookies = CookieContainer.GetCookies(url).Cast<Cookie>().ToList();
                    throw new Exception("886");
                }

                string resResult1 = httpResMsg.Content.ReadAsStringAsync().Result;
                nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resResult1, "form input"));
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
                lbl_Code.Content = ValCode;
                #endregion

                #region -- Post資料填入 --
                DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_SupplyNo, txt_SupplyNo.Text);
                DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_Password, pad_Password.Password);
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
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string,string>>();
            CookieContainer cookieContainer = new CookieContainer();
            string FormData_Name_SupplyNo = "ctl00$contentPlaceHolder$txtSupplyNo";
            string FormData_Name_Password = "ctl00$contentPlaceHolder$txtPassword";
            string FormData_Name_ValCode = "ctl00$contentPlaceHolder$txtValCode";
            string Page_Login = "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx";
            string Page_LoginImg = "https://amis.afa.gov.tw/CreateValidationCodeImage.aspx";
            string Page_QueryPrice = "https://amis.afa.gov.tw/coop1/CoopVegSupplierTransInfoQuery.aspx";
            string Check_PageIsQueryPrice = "CoopVegSupplierTransInfoQuery";
            string Request_UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
            string Request_Accept = "*/*";
            string Request_ContentType = "application/x-www-form-urlencoded";

            #region -- 登入帳號Flow --

            #region -- 1.Post登入資料組成 --

            Page_LoginPost:

            #region -- A.導覽登入頁面，並且取得相關Cookie資訊 以及 組成Post資料相關資訊的物件 --

            request = (HttpWebRequest)HttpWebRequest.Create(Page_Login);
            request.CookieContainer = cookieContainer;
            //set the user agent and accept header values, to simulate a real web browser
            request.UserAgent = Request_UserAgent;
            request.Accept = Request_Accept;
            //SET AUTOMATIC DECOMPRESSION
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);

            nameValueCollection.Clear();
            nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "form input"));
            #endregion

            #region -- B.取得驗證碼 --

            request = (HttpWebRequest)HttpWebRequest.Create(Page_LoginImg);
            request.CookieContainer = cookieContainer;
            request.UserAgent = Request_UserAgent;
            request.Accept = Request_Accept;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var img_byte = Use_WebClient.Get_ResponseImg(ref request);
            ValCode = Crawler.Todo_Tesseract3_3.GetImgText(img_byte);
            ValCode = (ValCode.Length > 4) ? ValCode.Substring(0, 4) : ValCode;
            lbl_Code.Content = ValCode;
            using (var ms = new MemoryStream(img_byte))
            {
                Img_Code.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad); ;
            }
            

            #endregion

            #region -- C.填寫帳號登入等相關資訊 --
                        
            //__ASYNCPOST 以及 ctl00$ScriptManager_Master無法從html得知，所以才用手動寫入的方式填寫            
            DataCollation.ListItemReplace(ref nameValueCollection, "__ASYNCPOST", "true");
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$ScriptManager_Master", "ctl00$contentPlaceHolder$UpdatePanel1|ctl00$contentPlaceHolder$btnLogin");
            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_SupplyNo, txt_SupplyNo.Text);
            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_Password, pad_Password.Password);
            DataCollation.ListItemReplace(ref nameValueCollection, FormData_Name_ValCode, ValCode);

            #endregion

            #endregion

            #region -- 2.登入Post請求發送 --

            request = (HttpWebRequest)HttpWebRequest.Create(Page_Login);
            request.CookieContainer = cookieContainer;
            request.UserAgent = Request_UserAgent;
            request.Accept = Request_Accept;
            request.Method = "POST";
            request.ContentType = Request_ContentType;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            string data = string.Join("&", nameValueCollection.Select(x => string.Format("{0}={1}", WebUtility.UrlEncode(x.Key), WebUtility.UrlEncode(x.Value))));
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            Use_WebClient.WriteFormDataIntoRequest(ref request, bytes);
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);
            resHtml = WebUtility.UrlDecode(resHtml);

            if (resHtml.IndexOf(Check_PageIsQueryPrice) < 0)
            {
                goto Page_LoginPost;
            }

            #endregion

            #endregion

            #region -- 取得水果相關資訊 --

            #region -- 1.進入登入後的頁面 --

            request = (HttpWebRequest)HttpWebRequest.Create(Page_QueryPrice);
            request.CookieContainer = cookieContainer;
            request.UserAgent = Request_UserAgent;
            request.Accept = Request_Accept;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);
            nameValueCollection.Clear();
            nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "form input[type='text']"));
            nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "form input[type='hidden']"));
            nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "form select"));
            nameValueCollection.AddRange(await Crawler.Todo_AngleSharp.GetHtmlDocument(resHtml, "span#ctl00_contentPlaceHolder_ucCoopVegFruitMarket_chklMarket", "104,109"));
            DataCollation.ListItemReplace(ref nameValueCollection, "__ASYNCPOST", "true");
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$contentPlaceHolder$btnQuery", "查詢");
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$contentPlaceHolder$txtProductNo", "O");
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$ScriptManager_Master", "ctl00$ScriptManager_Master|ctl00$contentPlaceHolder$btnQuery");
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00_contentPlaceHolder_ucCoopVegFruitMarket","109,104");
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$contentPlaceHolder$txtStartDate", dap_Start.SelectedDate.Value.ToString("yyy/MM/dd", culture));
            DataCollation.ListItemReplace(ref nameValueCollection, "ctl00$contentPlaceHolder$txtEndDate", dap_End.SelectedDate.Value.ToString("yyy/MM/dd", culture));
            #endregion

            #region -- 2.Post查詢資料 --

            request = (HttpWebRequest)HttpWebRequest.Create(Page_QueryPrice);
            request.CookieContainer = cookieContainer;
            request.UserAgent = Request_UserAgent;
            request.Accept = Request_Accept;
            request.Method = "POST";
            request.ContentType = Request_ContentType;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            string data1 = string.Join("&", nameValueCollection.Select(x => string.Format("{0}={1}", WebUtility.UrlEncode(x.Key), WebUtility.UrlEncode(x.Value))));
            byte[] bytes1 = Encoding.UTF8.GetBytes(data1);
            request.ContentLength = bytes1.Length;
            Use_WebClient.WriteFormDataIntoRequest(ref request, bytes1);
            resHtml = Use_WebClient.Get_ResponseHtml(ref request);
            resHtml = WebUtility.HtmlDecode(resHtml);

            #endregion

            #region -- 3.
            List<VegFruitMarketSupplyPrice> supplyPrices = new List<VegFruitMarketSupplyPrice>();
            var table_data = await Crawler.Todo_AngleSharp.GetVegFruitMarketData(resHtml, "table tr.main_main");
            
            foreach (var tr in table_data)
            {
                var temp_tr = new VegFruitMarketSupplyPrice();
                var prop = temp_tr.GetType().GetProperties();

                for (int i = 0; i < prop.Length; i++)
                {
                    var temp_count = tr.Count;
                    var temp_val = (temp_count < i) ? string.Empty : tr[i];
                    prop[i].SetValue(temp_tr, temp_val);
                }
                supplyPrices.Add(temp_tr);
            }

            dag_Show.ItemsSource = supplyPrices;
            #endregion

            #endregion

        }
        private void dag_Show_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            var result = e.PropertyName;
            var p = (e.PropertyDescriptor as PropertyDescriptor).ComponentType.GetProperties().FirstOrDefault(x => x.Name == e.PropertyName);

            if (p != null)
            {
                var found = p.GetCustomAttribute<DisplayAttribute>();
                if (found != null) result = found.Name;
            }

            e.Column.Header = result;
        }
    }
}
