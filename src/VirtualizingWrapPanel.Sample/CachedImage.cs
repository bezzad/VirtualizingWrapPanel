using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using Size = System.Drawing.Size;

namespace VirtualizingWrapPanel.Sample
{
    /// <summary>
    /// Represents a control that is a wrapper on System.Windows.Controls.Image for enabling filesystem-based caching
    /// </summary>
    public class CachedImage : System.Windows.Controls.Image
    {
        public static readonly DependencyProperty CreateOptionsProperty = DependencyProperty.Register(nameof(CreateOptions), typeof(BitmapCreateOptions), typeof(CachedImage), new PropertyMetadata(default(BitmapCreateOptions)));
        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(nameof(ImageUrl), typeof(string), typeof(CachedImage), new PropertyMetadata(default(string), ImageUrlPropertyChanged));
        public static readonly DependencyProperty AvatarNameProperty = DependencyProperty.Register(nameof(AvatarName), typeof(string), typeof(CachedImage), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(int), typeof(CachedImage), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(int), typeof(CachedImage), new PropertyMetadata(default(int)));

        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }
        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public string AvatarName
        {
            get => (string)GetValue(AvatarNameProperty);
            set => SetValue(AvatarNameProperty, value);
        }
        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }
        public BitmapCreateOptions CreateOptions
        {
            get => (BitmapCreateOptions)GetValue(CreateOptionsProperty);
            set => SetValue(CreateOptionsProperty, value);
        }

        private static async void ImageUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var cachedImage = (CachedImage)obj;
                Debug.WriteLine($"CacheImage.ImageUrlPropertyChanged --> CachedImage: {e.NewValue}");

                if (e.NewValue is string url && string.IsNullOrWhiteSpace(url) == false)
                {

                    switch (FileCache.AppCacheMode)
                    {
                        case FileCache.CacheMode.WinINet:
                            SetWinINetCachedImage(cachedImage, url);
                            break;

                        case FileCache.CacheMode.Dedicated:
                            await SetDedicatedCachedImage(cachedImage, url);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    cachedImage.Source = null;
                }
            }
            catch
            {
                // ignored, in case the downloaded file is a broken or not an image.
                //Debugger.Break();
            }
        }

        private static void SetWinINetCachedImage(CachedImage cachedImage, string url)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = cachedImage.CreateOptions;
            bitmapImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            // Enable IE-like cache policy.
            bitmapImage.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
            bitmapImage.EndInit();
            cachedImage.Source = bitmapImage;
        }
        private static async Task SetDedicatedCachedImage(CachedImage cachedImage, string url)
        {
            var bitmapImage = new BitmapImage();
            var memoryStream = await FileCache.HitAsync(url);
            if (memoryStream == null && string.IsNullOrWhiteSpace(cachedImage.AvatarName) == false)
            {
                cachedImage.ImageWidth = cachedImage.ImageWidth > 1 ? cachedImage.ImageWidth : 256;
                cachedImage.ImageHeight = cachedImage.ImageHeight > 1 ? cachedImage.ImageHeight : cachedImage.ImageWidth;

                memoryStream = GenerateAvatarImage(cachedImage.AvatarName,
                    new Size(cachedImage.ImageWidth, cachedImage.ImageHeight),
                    (float)(cachedImage.ImageWidth / 2.66), Color.Teal, Color.White);
            }

            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = cachedImage.CreateOptions;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            cachedImage.Source = bitmapImage;
        }
        private static MemoryStream GenerateAvatarImage(string name, Size imgSize, float emSize, Color background, Color foreground)
        {
            var avatarString = name[0].ToString().ToUpper();

            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.Character, FormatFlags = StringFormatFlags.NoWrap };
            using var fontFamily = new FontFamily("Arial");
            using var font = new Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
            using Brush textBrush = new SolidBrush(foreground);
            using var bmp = new Bitmap(imgSize.Width, imgSize.Height);
            using var graphics = Graphics.FromImage(bmp);

            //measure the string to see how big the image needs to be  
            //var textSize = graphics.MeasureString(avatarString, font);

            graphics.Clear(background);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            graphics.DrawString(avatarString, font, textBrush,
                new RectangleF(0, 0, imgSize.Width, imgSize.Height),
                sf);

            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            return ms;
        }
    }
}
