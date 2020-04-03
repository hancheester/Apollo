using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class ImageUtility
    {
        private const int PRODUCT_IMG_WIDTH = 300;
        private const int PRODUCT_IMG_HEIGHT = 300;
        private const int PRODUCT_THUMB_WIDTH = 100;
        private const int PRODUCT_THUMB_HEIGHT = 100;
        private const int PRODUCT_HIGH_RES_WIDTH = 1200;
        private const int PRODUCT_HIGH_RES_HEIGHT = 1200;
        public const string PRODUCT_IMG_SUCCESS = "Image uploaded successfully!";
        public const string PRODUCT_IMG_INVALID = "The file wasn't a valid jpg file.";
        public const string PRODUCT_IMG_WRONG_EXT = "The file must have an extension of JPG / JPEG.";
        public const string PRODUCT_IMG_NO_FILE = "No file was uploaded.";
        public const string PRODUCT_IMG_INVALID_DIMENSION = "Invalid image size.";
        public const string PRODUCT_IMAGE_PATH_NOT_FOUND_OR_INVALID = "Due to product image path not found or invalid";
        private const string PRODUCT_THUMB_SUFFIX = "-t";
        private const string PRODUCT_HIGH_RES_SUFFIX = "-hr";
        private const string JPG = ".jpg";
        private const string JPEG = ".jpeg";
        private const long JPG_QUALITY = 90;

        private const int COLOUR_IMG_WIDTH = 50;
        private const int COLOUR_IMG_HEIGHT = 50;
        private const int COLOUR_THUMB_WIDTH = 10;
        private const int COLOUR_THUMB_HEIGHT = 10;
        public const string COLOUR_IMG_SUCCESS = "Colour uploaded successfully!";

        private ILogger _logger;
        private AdminStoreUtility _storeUtility;
        private MediaSettings _mediaSettings;

        public ImageUtility(ILogBuilder logBuilder, AdminStoreUtility storeUtility, MediaSettings mediaSettings)
        {
            if (logBuilder == null) throw new ArgumentException("logBuilder");

            this._logger = logBuilder.CreateLogger(typeof(ImageUtility).FullName);
            this._storeUtility = storeUtility;
            this._mediaSettings = mediaSettings;
        }

        public string ProductMediaPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_mediaSettings.ProductMediaLocalPath))
                    return _mediaSettings.ProductMediaLocalPath;
                else
                    return string.Empty;
            }
        }

        public string ProductColourPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_mediaSettings.ProductColourLocalPath))
                    return _mediaSettings.ProductColourLocalPath;
                else
                    return string.Empty;
            }
        }

        public string UploadProductImage(HttpPostedFile imageFile, string filename, int productId, bool primary, out ProductMedia media)
        {
            media = null;
            int _fileSize = imageFile.ContentLength;

            // check availability
            if (_fileSize == 0)
                return PRODUCT_IMG_NO_FILE;

            // check file extension
            if ((Path.GetExtension(imageFile.FileName).ToLower() != JPG) && (Path.GetExtension(imageFile.FileName).ToLower() != JPEG))
                return PRODUCT_IMG_WRONG_EXT;

            // check dimensions
            Image oImg = Image.FromStream(imageFile.InputStream);
            if (oImg.Height != PRODUCT_HIGH_RES_HEIGHT && oImg.Width != PRODUCT_HIGH_RES_WIDTH)            
                return PRODUCT_IMG_INVALID_DIMENSION;
            
            byte[] byteData = new Byte[_fileSize];
            imageFile.InputStream.Read(byteData, 0, _fileSize);
            if (ProductMediaPath == string.Empty)
                return PRODUCT_IMAGE_PATH_NOT_FOUND_OR_INVALID;

            string sFilename = GetUniqueFileName(filename, string.Empty, ProductMediaPath);
            //string sThumbFile = GetUniqueFileName(filename, PRODUCT_THUMB_SUFFIX, ProductMediaPath);
            //string sHighResFile = GetUniqueFileName(filename, PRODUCT_HIGH_RES_SUFFIX, ProductMediaPath);
            string sThumbFile = sFilename.Replace(JPG, PRODUCT_THUMB_SUFFIX + JPG);
            string sHighResFile = sFilename.Replace(JPG, PRODUCT_HIGH_RES_SUFFIX + JPG);

            bool imgSuccess = SaveFromStream(oImg, PRODUCT_IMG_WIDTH, PRODUCT_IMG_HEIGHT, ProductMediaPath + sFilename);
            bool thumbSuccess = SaveFromStream(oImg, PRODUCT_THUMB_WIDTH, PRODUCT_THUMB_HEIGHT, ProductMediaPath + sThumbFile);
            bool highResSuccess = SaveFromStream(oImg, PRODUCT_HIGH_RES_WIDTH, PRODUCT_HIGH_RES_HEIGHT, ProductMediaPath + sHighResFile);

            oImg.Dispose();

            if (imgSuccess && thumbSuccess && highResSuccess)
            {
                media = new ProductMedia();
                media.ProductId = productId;
                media.MediaType = "Image";
                media.ThumbnailFilename = sThumbFile;
                media.MediaFilename = sFilename;
                media.HighResFilename = sHighResFile;
                media.PrimaryImage = primary;

                //ProductDataProxy.InsertProductMedia(media);
                //if (primary) ProductDataProxy.SetPrimaryImage(media.Id, productId);
                return string.Empty;
            }
            else
            {
                media = null;
                return PRODUCT_IMG_INVALID;
            }
        }

        public string UploadProduct300Image(HttpPostedFile imageMainFile, HttpPostedFile imageThumbFile, string filename, int productId, bool primary, out ProductMedia media)
        {
            media = null;

            int _fileMainSize = imageMainFile.ContentLength;
            int _fileThumbSize = imageThumbFile.ContentLength;

            // check availability
            if (_fileMainSize == 0 || _fileThumbSize == 0)
                return PRODUCT_IMG_NO_FILE;

            // check file extension
            if ((Path.GetExtension(imageMainFile.FileName).ToLower() != JPG) && (Path.GetExtension(imageMainFile.FileName).ToLower() != JPEG) &&
                (Path.GetExtension(imageThumbFile.FileName).ToLower() != JPG) && (Path.GetExtension(imageThumbFile.FileName).ToLower() != JPEG))
                return PRODUCT_IMG_WRONG_EXT;

            // check dimensions
            Image oImgMain = Image.FromStream(imageMainFile.InputStream);
            Image oImgThumb = Image.FromStream(imageThumbFile.InputStream);

            if (oImgMain.Height != PRODUCT_IMG_HEIGHT && oImgMain.Width != PRODUCT_IMG_WIDTH)
                return PRODUCT_IMG_INVALID_DIMENSION;

            if (oImgThumb.Height != PRODUCT_THUMB_HEIGHT && oImgThumb.Width != PRODUCT_THUMB_WIDTH)
                return PRODUCT_IMG_INVALID_DIMENSION;

            byte[] byteMainData = new Byte[_fileMainSize];
            imageMainFile.InputStream.Read(byteMainData, 0, _fileMainSize);

            byte[] byteThumbData = new Byte[_fileThumbSize];
            imageThumbFile.InputStream.Read(byteThumbData, 0, _fileThumbSize);

            string sFilename = GetUniqueFileName(filename, string.Empty, ProductMediaPath);
            //string sThumbFile = GetUniqueFileName(filename, PRODUCT_THUMB_SUFFIX, ProductMediaPath);
            //string sHighResFile = GetUniqueFileName(filename, PRODUCT_HIGH_RES_SUFFIX, ProductMediaPath);
            string sThumbFile = sFilename.Replace(JPG, PRODUCT_THUMB_SUFFIX + JPG);
            //string sHighResFile = sFilename.Replace(JPG, PRODUCT_HIGH_RES_SUFFIX + JPG);
            string sHighResFile = string.Empty;

            bool imgSuccess = SaveFromStream(oImgMain, PRODUCT_IMG_WIDTH, PRODUCT_IMG_HEIGHT, ProductMediaPath + sFilename);
            bool thumbSuccess = SaveFromStream(oImgThumb, PRODUCT_THUMB_WIDTH, PRODUCT_THUMB_HEIGHT, ProductMediaPath + sThumbFile);
            //bool highResSuccess = SaveFromStream(oImg, PRODUCT_HIGH_RES_WIDTH, PRODUCT_HIGH_RES_HEIGHT, ProductMediaPath + sHighResFile);
            bool highResSuccess = true;

            oImgMain.Dispose();
            oImgThumb.Dispose();

            if (imgSuccess && thumbSuccess && highResSuccess)
            {
                media = new ProductMedia();
                media.ProductId = productId;
                media.MediaType = "Image";
                media.ThumbnailFilename = sThumbFile;
                media.MediaFilename = sFilename;
                media.HighResFilename = sHighResFile;
                media.PrimaryImage = primary;

                return string.Empty;
            }
            else
            {
                media = null;
                return PRODUCT_IMG_INVALID;
            }
        }

        public bool UploadColourImage(HttpPostedFile imageFile, string name, out string mainFilename, out string thumbFilename)
        {
            mainFilename = string.Empty;
            thumbFilename = string.Empty;

            int _fileSize = imageFile.ContentLength;
            string filename = name.Replace(" ", string.Empty);
            filename = filename.Replace("/", string.Empty);            

            if (_fileSize == 0)
                return false;

            string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (fileExtension != JPG && fileExtension != JPEG)
                return false;

            byte[] byteData = new Byte[_fileSize];
            imageFile.InputStream.Read(byteData, 0, _fileSize);

            mainFilename = GetUniqueFileName(filename, string.Empty, ProductColourPath);
            thumbFilename = GetUniqueFileName(filename, PRODUCT_THUMB_SUFFIX, ProductColourPath);

            bool imgSuccess = SaveFromStream(imageFile.InputStream, COLOUR_IMG_WIDTH, COLOUR_IMG_HEIGHT, ProductColourPath + mainFilename);
            bool thumbSuccess = SaveFromStream(imageFile.InputStream, COLOUR_THUMB_WIDTH, COLOUR_THUMB_HEIGHT, ProductColourPath + thumbFilename);

            return imgSuccess && thumbSuccess;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        public bool SaveFromStream(Image img, int width, int height, string filePath)
        {
            try
            {
                Size size = new Size(width, height);
                Image oTargetImg = new Bitmap(width, height);
                Graphics oGraphic = Graphics.FromImage(oTargetImg);
                oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                Rectangle oRectangle = new Rectangle(0, 0, size.Width, size.Height);
                oGraphic.DrawImage(img, oRectangle);

                // Encoder parameter for image quality 
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, JPG_QUALITY);
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;

                //Save File
                oTargetImg.Save(filePath, jpegCodec, encoderParams);

                oGraphic.Dispose();
                
                return true;
            }
            catch (Exception e)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("{0}, File path={{{1}}}", e.Message, filePath), e);
                return false;
            }
        }

        public bool SaveFromStream(Stream Buffer, int width, int height, string filePath)
        {
            try
            {
                Image oImg = Image.FromStream(Buffer);
                Size ThumbNailSize = new Size(width, height);
                Image oThumbNail = new Bitmap(width, height);
                Graphics oGraphic = Graphics.FromImage(oThumbNail);
                oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                
                Rectangle oRectangle = new Rectangle(0, 0, ThumbNailSize.Width, ThumbNailSize.Height);
                oGraphic.DrawImage(oImg, oRectangle);

                // Encoder parameter for image quality 
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, JPG_QUALITY);
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam; 

                //Save File
                oThumbNail.Save(filePath, jpegCodec, encoderParams);
                
                oGraphic.Dispose();
                oImg.Dispose();

                return true;
            }
            catch (Exception e)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("{0}, File path={{{1}}}", e.Message, filePath), e);
                return false;
            }
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        private string GetUniqueFileName(string desiredFileName, string suffix, string path)
        {
            desiredFileName = _storeUtility.GetFriendlyUrlKey(desiredFileName);
            string actualFileName = desiredFileName + suffix + JPG;
            
            int file_append = 0;
            while (File.Exists(path + actualFileName))
            {
                file_append++;
                actualFileName = desiredFileName + file_append.ToString() + suffix + JPG;
            }

            return actualFileName;
        }
    }
}