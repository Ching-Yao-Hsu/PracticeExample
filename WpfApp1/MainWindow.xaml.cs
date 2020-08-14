using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using AngleSharp;
using WpfApp1.Model.dto;
using System.Configuration;
using System.IO;
using System.Drawing;
using Tesseract;
using OpenCvSharp;

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
            HttpClient httpClient = new HttpClient();
            var config = AngleSharp.Configuration.Default;
            var context = BrowsingContext.New(config);
            string url_Login = "https://amis.afa.gov.tw/coop1/CoopVegLogin1.aspx";
            string url_ValCodeImage = "https://amis.afa.gov.tw/CreateValidationCodeImage.aspx";
            string FormData_Name_SupplyNo = "ctl00$contentPlaceHolder$txtSupplyNo";
            string FormData_Name_Password = "ctl00$contentPlaceHolder$txtPassword";
            string FormData_Name_ValCode = "ctl00$contentPlaceHolder$txtValCode";
            List<CoopVegLogin1_FormData> FormData_dto;

            #region -- Post --
            //IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
            //    { new KeyValuePair<string, string>("ctl00$contentPlaceHolder$txtSupplyNo", "K01792") },
            //    { new KeyValuePair<string, string>("ctl00$contentPlaceHolder$txtPassword", "536468") },
            //    { new KeyValuePair<string, string>("ctl00$contentPlaceHolder$txtValCode", "2831") },
            //    { new KeyValuePair<string, string>("ctl00$ScriptManager_Master","ctl00$contentPlaceHolder$UpdatePanel1|ctl00$contentPlaceHolder$btnLogin")},
            //    { new KeyValuePair<string, string>("__LASTFOCUS", string.Empty) },
            //    { new KeyValuePair<string, string>("__EVENTTARGET", string.Empty) },
            //    { new KeyValuePair<string, string>("__EVENTARGUMENT", string.Empty) },
            //    { new KeyValuePair<string, string>("__VIEWSTATE", "K01792") },
            //    { new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "K01792") },
            //    { new KeyValuePair<string, string>("__EVENTVALIDATION", "K01792") },
            //    { new KeyValuePair<string, string>("__ASYNCPOST", "true") },
            //    { new KeyValuePair<string, string>("ctl00$contentPlaceHolder$btnLogin", "登入") }
            //};

            //var resMsg = await httpClient.PostAsync(url, new FormUrlEncodedContent(nameValueCollection));

            //if (resMsg.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    string resResult = resMsg.Content.ReadAsStringAsync().Result;
            //    lbl_InfoMsg.Content = resResult;
            //}
            #endregion

            #region -- Get --
            HttpResponseMessage resMsg = new HttpResponseMessage();

            try
            {
                //resMsg = await httpClient.GetAsync(url_Login);
                //if (resMsg.StatusCode != System.Net.HttpStatusCode.OK)
                //{
                //    throw new Exception("886");
                //}

                //string resResult = resMsg.Content.ReadAsStringAsync().Result;

                //var document = await context.OpenAsync(res => res.Content(resResult));
                //var contents = document.QuerySelectorAll("form input");

                //if (contents.Length <= 0)
                //{
                //    throw new Exception("886");
                //}

                //FormData_dto = new List<CoopVegLogin1_FormData>();
                //foreach (var item in contents)
                //{
                //    FormData_dto.Add(new CoopVegLogin1_FormData
                //    {
                //        Name = item.GetAttribute("name"),
                //        Value = item.GetAttribute("value")
                //    });
                //}

                resMsg = await httpClient.GetAsync(url_ValCodeImage);

                //if (!resMsg.IsSuccessStatusCode)
                //{
                //    throw new Exception("886");
                //}

                byte[] image = await resMsg.Content.ReadAsByteArrayAsync();
                //Mat src = Cv2.ImDecode(image, ImreadModes.Color);
                //using (new OpenCvSharp.Window("asdf", src))
                //{

                //}

                //Mat src = new Mat("lenna.png", ImreadModes.Grayscale);
                Mat src = Cv2.ImDecode(image, ImreadModes.Grayscale);
                //Mat dst = new Mat();

                //Cv2.Canny(src, dst, 50, 200);
                using (new OpenCvSharp.Window("src image", src))
                {
                    Cv2.WaitKey();
                }


                using (var inms = new MemoryStream(src.ToBytes()))
                using (var outms = new MemoryStream())
                {
                    System.Drawing.Bitmap.FromStream(inms).Save(outms, System.Drawing.Imaging.ImageFormat.Tiff);
                    var pix = Pix.LoadTiffFromMemory(outms.ToArray());

                    ImageSource result;
                    result = BitmapFrame.Create(outms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    Img_Test.Source = result;


                    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) 
                    {
                        Tesseract.Page page = engine.Process(pix);

                        string res = page.GetText();
                        lbl_Test.Content = res;
                    }
                }



                





            }
            catch (Exception ex) 
            {
                throw;
            }
            finally
            {
                resMsg.Dispose();
            }
            #endregion
            }

        private void btn_CloseClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
