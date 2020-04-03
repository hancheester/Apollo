using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm
{
    public partial class get_image_handler : Page
    {
        public MediaSettings MediaSettings { get; set; }
        public IProductService ProductService { get; set; }
        public ICampaignService CampaignService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            FileStream streamer = null;
            try
            {
                switch (Type)
                {
                    case ImageHandlerType.MEDIA:
                        if (ProductId != 0)
                        {
                            var medias = ProductService.GetProductMediaByProductId(ProductId);
                            if (medias != null && medias.Count > 0)
                            {
                                var media = medias.Where(x => x.Enabled == true).FirstOrDefault();
                                streamer = new FileStream(Path + media.ThumbnailFilename, FileMode.Open, FileAccess.Read);
                            }
                        }

                        if (string.IsNullOrEmpty(FileName) == false)
                        {
                            streamer = new FileStream(Path + FileName, FileMode.Open, FileAccess.Read);
                        }
                        break;
                    case ImageHandlerType.LARGE_BANNER:
                        if (Id != 0)
                        {
                            var largeBanner = CampaignService.GetLargeBannerById(Id);
                            if (largeBanner != null)
                            {
                                streamer = new FileStream(Path + largeBanner.MediaFilename, FileMode.Open, FileAccess.Read);
                            }
                        }
                        break;
                    case ImageHandlerType.BRAND:
                    case ImageHandlerType.COLOUR:
                    case ImageHandlerType.CATEGORY:
                    case ImageHandlerType.MINI_BANNER:
                    case ImageHandlerType.MEDIUM_BANNER:
                    case ImageHandlerType.OFFER_BANNER:
                    case ImageHandlerType.OFFER:
                    default:
                        streamer = new FileStream(Path + FileName, FileMode.Open, FileAccess.Read);
                        break;
                }
            }
            catch
            {
                if (File.Exists(MediaSettings.NoImageLocalPath))
                    streamer = new FileStream(MediaSettings.NoImageLocalPath, FileMode.Open, FileAccess.Read);
            }

            byte[] bytes = new byte[64];
            if (streamer != null)
            {
                BinaryReader reader = new BinaryReader(streamer);
                bytes = reader.ReadBytes((int)streamer.Length);
                reader.Close();
                streamer.Close();
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = string.Empty;
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.ContentType = "image/";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
            HttpContext.Current.Response.BinaryWrite(bytes);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        private string FileName
        {
            get
            {
                if (HttpContext.Current.Request.QueryString[QueryKey.IMG] == null)
                    return string.Empty;
                else
                    return HttpContext.Current.Request.QueryString[QueryKey.IMG];
            }
        }

        private int ProductId
        {
            get
            {
                if (HttpContext.Current.Request.QueryString[QueryKey.PRODUCT_ID] == null)
                    return 0;
                else
                    return Convert.ToInt32(HttpContext.Current.Request.QueryString[QueryKey.PRODUCT_ID]);
            }
        }

        private int Id
        {
            get
            {
                if (HttpContext.Current.Request.QueryString[QueryKey.ID] == null)
                    return 0;
                else
                    return Convert.ToInt32(HttpContext.Current.Request.QueryString[QueryKey.ID]);
            }
        }

        private string Type
        {
            get
            {
                if (HttpContext.Current.Request.QueryString[QueryKey.TYPE] == null)
                    return string.Empty;
                else
                    return HttpContext.Current.Request.QueryString[QueryKey.TYPE];
            }
        }

        private string Path
        {
            get
            {
                if (HttpContext.Current.Request.QueryString[QueryKey.TYPE] == null)
                    return string.Empty;
                else
                {
                    switch (HttpContext.Current.Request.QueryString[QueryKey.TYPE])
                    {
                        case ImageHandlerType.BRAND:
                            return MediaSettings.BrandMediaLocalPath;
                        case ImageHandlerType.COLOUR:
                            return MediaSettings.ProductColourLocalPath;
                        case ImageHandlerType.MEDIA:
                            return MediaSettings.ProductMediaLocalPath;
                        case ImageHandlerType.CATEGORY:
                            return MediaSettings.CategoryMediaLocalPath;
                        case ImageHandlerType.LARGE_BANNER:
                            return MediaSettings.LargeBannerLocalPath;
                        case ImageHandlerType.MINI_BANNER:
                            return MediaSettings.MiniBannerLocalPath;
                        case ImageHandlerType.MEDIUM_BANNER:
                            return MediaSettings.MediumBannerLocalPath;
                        case ImageHandlerType.OFFER_BANNER:
                            return MediaSettings.OfferBannerLocalPath;
                        case ImageHandlerType.OFFER:
                            return MediaSettings.OfferMediaLocalPath;
                        default:
                            return string.Empty;
                    }
                }
            }
        }
    }
}