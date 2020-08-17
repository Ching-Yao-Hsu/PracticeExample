using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
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
            public static async Task<IList<KeyValuePair<string, string>>> GetHtmlDocument(string str_Document,string QuerySelectorAll) 
            {
                IList<KeyValuePair<string, string>> input = new List<KeyValuePair<string, string>>();

                var document = await context.OpenAsync(res => res.Content(str_Document));

                if (document != null)
                {
                    var contents = document.QuerySelectorAll("form input");

                    if (contents != null)
                    {
                        foreach (var item in contents)
                        {
                            input.Add(new KeyValuePair<string, string>(item.GetAttribute("name"), item.GetAttribute("value")));
                        }
                    }
                }

                return input;
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
