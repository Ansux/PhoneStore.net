using PhoneStoreSystem.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NewsSystem.Controllers
{
    [ValidateLogin]
    public class PublicController : Controller
    {
        //
        // GET: /Public/


        /*
        public void uploadFiles(string fileName, string safeFileName)
        {
            string lj1 = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString();
            string lj2 = System.IO.Directory.GetParent(lj1) + "\\images\\";

            string soursefilename = fileName;
            string desname = lj2 + safeFileName;

            if (soursefilename == "" || desname == "")
            {
                MessageBox.Show("请选择文件和保存位置");
                return;
            }
            Image img = Image.FromFile(soursefilename);
            Bitmap b = SizeImage(img, 200, 300);
            b.Save(desname);
            b.Dispose();
            //File.Copy(soursefilename, desname, true); 
            MessageBox.Show("图片上传成功！");

        }
        */

        public static bool SaveFormatJpeg(Bitmap b, string path, long quality)
        {
            try
            {
                EncoderParameters eptS = new EncoderParameters(1);
                EncoderParameter en = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                eptS.Param[0] = en;
                ImageCodecInfo ici = PublicController.GetEncoderInfo("image/jpeg");
                b.Save(path, ici, eptS);
                b.Dispose();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        /*
         * 设置图片的格式  GetEncoderInfo("image/jpeg");
         */
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            return null;
        }


        //获取时间戳
        public static long TimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));

            System.Threading.Thread.Sleep(1);
            DateTime nowTime = DateTime.Now;

            long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);

            return unixTime;
        }

        /// <summary>
        /// 按百分比缩放图片 Bitmap b = PercentImage(img, 0.5); b.Save(路径+"xx.png");  b.Dispose();
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Bitmap PercentImage(Image srcImage, double percent)
        {
            // 缩小后的高度
            int newH = int.Parse(Math.Round(srcImage.Height * percent).ToString());
            // 缩小后的宽度
            int newW = int.Parse(Math.Round(srcImage.Width * percent).ToString());
            try
            {
                // 要保存到的图片
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.Default;
                g.DrawImage(srcImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 按照指定像素大小截图 Bitmap b = SizeImage(img, 400,200);b.Save(lj2+"xx.png"); b.Dispose();
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <returns></returns>
        public static Bitmap SizeImage(Image srcImage, int iWidth, int iHeight)
        {
            try
            {
                // 要保存到的图片
                Bitmap b = new Bitmap(iWidth, iHeight);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(srcImage, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}


namespace System.Web.Mvc.Html
{
    public static class MYFormExtensions
    {
        public static HtmlString ShowPageNavigate(this HtmlHelper htmlHelper, int currentPage, int pageSize, int totalCount)
        {
            var redirectTo = htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
            pageSize = pageSize == 0 ? 3 : pageSize;
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数
            var output = new StringBuilder();
            if (totalPages > 1)
            {
                output.AppendFormat("<a class='pageLink' href='{0}?pageIndex=1&pageSize={1}'>首页</a> ", redirectTo, pageSize);
                if (currentPage > 1)
                {//处理上一页的连接
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}'>上一页</a> ", redirectTo, currentPage - 1, pageSize);
                }

                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {//一共最多显示10个页码，前面5个，后面5个
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                    {
                        if (currint == i)
                        {//当前页处理                           
                            output.AppendFormat("<a class='cpb' href='{0}?pageIndex={1}&pageSize={2}'>{3}</a> ", redirectTo, currentPage, pageSize, currentPage);
                        }
                        else
                        {//一般页处理
                            output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}'>{3}</a> ", redirectTo, currentPage + i - currint, pageSize, currentPage + i - currint);
                        }
                    }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {//处理下一页的链接
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}'>下一页</a> ", redirectTo, currentPage + 1, pageSize);
                }

                output.Append(" ");
                if (currentPage != totalPages)
                {
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}'>末页</a> ", redirectTo, totalPages, pageSize);
                }
                output.Append(" ");
            }
            output.AppendFormat("<label>第{0}页 / 共{1}页</label>", currentPage, totalPages);//这个统计加不加都行

            return new HtmlString(output.ToString());
        }
    }
}