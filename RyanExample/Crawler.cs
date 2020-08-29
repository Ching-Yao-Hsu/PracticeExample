using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using OpenCvSharp;
using Tesseract;

namespace RyanExample
{
    public class Crawler
    {
        public Crawler() 
        {

        }
        /// <summary>
        /// AngleSharp 套件 => 針對html document做塞選資料
        /// </summary>
        public class Todo_AngleSharp
        {
            private static IBrowsingContext context;
            public Todo_AngleSharp() 
            {
                context = BrowsingContext.New(AngleSharp.Configuration.Default);
            }
            /// <summary>
            /// 取得HtmlDocument內的element資訊
            /// </summary>
            /// <param name="str_Document">HtmlDocument</param>
            /// <param name="QuerySelectorAll">Html的element選擇條件</param>
            /// <returns></returns>
            public static async Task<List<KeyValuePair<string, string>>> GetHtmlDocument(string str_Document, string QuerySelectorAll) 
            {
                List<KeyValuePair<string, string>> input = new List<KeyValuePair<string, string>>();

                var _document = await context.OpenAsync(res => res.Content(str_Document));

                if (_document != null)
                {
                    var contents = _document.QuerySelectorAll(QuerySelectorAll);

                    if (contents != null)
                    {
                        if (QuerySelectorAll.ToLower().Contains("form input"))
                        {
                            foreach (var item in contents)
                            {
                                var name = item.GetAttribute("name");
                                var value = item.GetAttribute("value");
                                value = string.IsNullOrEmpty(value) ? string.Empty : value;
                                input.Add(new KeyValuePair<string, string>(name, value));
                            }
                        }
                        else {
                            foreach (var item in contents)
                            {
                                AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement> option = item.QuerySelectorAll("option:checked");
                                if (option.Count() > 0)
                                {
                                    var name = item.GetAttribute("name");
                                    var value = option.FirstOrDefault().GetAttribute("value");
                                    value = string.IsNullOrEmpty(value) ? string.Empty : value;
                                    input.Add(new KeyValuePair<string, string>(name, value));
                                }
                                else {
                                    option = item.QuerySelectorAll("option");
                                    if (option.Count() > 0)
                                    {
                                        var name = item.GetAttribute("name");
                                        var value = option.FirstOrDefault().GetAttribute("value");
                                        value = string.IsNullOrEmpty(value) ? string.Empty : value;
                                        input.Add(new KeyValuePair<string, string>(name, value));
                                    }
                                }
                            }
                        }
                    }
                }

                return input;
            }
            /// <summary>
            /// 取得HtmlDocument內的element資訊
            /// </summary>
            /// <param name="str_Document">HtmlDocument</param>
            /// <param name="QuerySelectorAll">Html的element選擇條件</param>
            /// <param name="QueryCondition">選擇checkbox的條件</param>
            /// <returns></returns>
            public static async Task<List<KeyValuePair<string, string>>> GetHtmlDocument(string str_Document, string QuerySelectorAll,string QueryCondition) 
            {
                List<KeyValuePair<string, string>> element = new List<KeyValuePair<string, string>>();

                var _document = await context.OpenAsync(res => res.Content(str_Document));

                if (_document != null)
                {
                    var contents = _document.QuerySelector(QuerySelectorAll);

                    if (contents != null)
                    {
                        switch (QuerySelectorAll)
                        {
                            case "span#ctl00_contentPlaceHolder_ucCoopVegFruitMarket_chklMarket":
                                string[] QueryCondition_Ary = QueryCondition.Split(',');
                                var span_chk = contents.QuerySelectorAll(QuerySelectorAll + ">span");

                                foreach (var item in span_chk)
                                {
                                    var attr = item.GetAttribute("marketno");
                                    if (attr != null) 
                                    {
                                        if (QueryCondition_Ary.Any(m => m.Equals(attr)))
                                        {
                                            var input = item.QuerySelector("input");
                                            if (input != null)
                                            {
                                                var name = input.GetAttribute("name");
                                                var value = "on";
                                                element.Add(new KeyValuePair<string, string>(name, value));
                                            }

                                            
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                return element;
            }
            public static async Task<List<List<string>>> GetVegFruitMarketData(string str_HtmlDataTable,string QuerySelectorAll) 
            {
                List<List<string>> data_tr = new List<List<string>>();

                var html_table = await context.OpenAsync(res => res.Content(str_HtmlDataTable));

                if (html_table != null)
                {
                    var temp_table_tr = html_table.QuerySelectorAll(QuerySelectorAll);
                    var table = temp_table_tr.Parent("table").FirstOrDefault();
                    var table_tr = table.QuerySelectorAll("tr:not(.main_title)");

                    foreach (var tr in table_tr)
                    {
                        var table_td = tr.QuerySelectorAll("td");
                        if (table_td.Count() > 0)
                        {
                            var data_td = new List<string>();
                            foreach (var td in table_td)
                            {
                                data_td.Add(td.TextContent);
                            }
                            data_tr.Add(data_td);
                        }
                    }
                }

                return data_tr;
            }
        }

        /// <summary>
        /// OpenCvSharp4 套件 => 它由一系列 C 函數和少量 C++ 類構成，實現了圖像處理和電腦視覺方面的很多通用演算法
        /// </summary>
        public class Todo_OpenCvSharp4
        {
            /// <summary>
            /// 圖檔格式之轉換
            /// </summary>
            /// <param name="img">圖片檔</param>
            /// <returns></returns>
            public static Mat ImgByteArrayToMat(byte[] img)
            {

                Mat mat = Cv2.ImDecode(img, ImreadModes.Grayscale);
                return mat;
            }
        }

        /// <summary>
        /// Tesseract3_3 套件 => 光學字元辨識，抓取圖片中文字的資訊
        /// </summary>
        public class Todo_Tesseract3_3 
        {
            /// <summary>
            /// 取得圖片中的文字
            /// </summary>
            /// <param name="ImgData">圖片檔</param>
            /// <param name="TessData_Path">圖片文字識別資源檔之路徑</param>
            /// <param name="TessData_Language">選擇的文字語言</param>
            /// <returns></returns>
            public static string GetImgText(byte[] ImgData, string TessData_Path = @"./tessdata", string TessData_Language = "eng")
            {
                string ImgText;

                Mat MatImg = Todo_OpenCvSharp4.ImgByteArrayToMat(ImgData);

                using (var inms = new MemoryStream(MatImg.ToBytes()))
                using (var outms = new MemoryStream())
                {
                    System.Drawing.Bitmap.FromStream(inms).Save(outms, System.Drawing.Imaging.ImageFormat.Tiff);
                    var pix = Pix.LoadTiffFromMemory(outms.ToArray());

                    using (var engine = new TesseractEngine(TessData_Path, TessData_Language, EngineMode.Default))
                    {
                        Tesseract.Page page = engine.Process(pix);
                        ImgText = page.GetText();
                    }
                }

                //Mat src = Cv2.ImDecode(image, ImreadModes.Color);
                //using (new OpenCvSharp.Window("asdf", src))
                //{

                //}

                ////Mat src = new Mat("lenna.png", ImreadModes.Grayscale);
                //Mat src = Cv2.ImDecode(image, ImreadModes.Grayscale);
                ////Mat dst = new Mat();

                ////Cv2.Canny(src, dst, 50, 200);
                //using (new OpenCvSharp.Window("src image", src))
                //{
                //    Cv2.WaitKey();
                //}

                //using (var inms = new MemoryStream(src.ToBytes()))
                //using (var outms = new MemoryStream())
                //{
                //    System.Drawing.Bitmap.FromStream(inms).Save(outms, System.Drawing.Imaging.ImageFormat.Tiff);
                //    var pix = Pix.LoadTiffFromMemory(outms.ToArray());

                //    ImageSource result;
                //    result = BitmapFrame.Create(outms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                //    Img_Test.Source = result;


                //    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) 
                //    {
                //        Tesseract.Page page = engine.Process(pix);

                //        string res = page.GetText();
                //        lbl_Test.Content = res;
                //    }
                //}
                return ImgText;
            }
        }
    }
}
