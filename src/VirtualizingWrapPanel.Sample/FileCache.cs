using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualizingWrapPanel.Sample
{
    public static class FileCache
    {
        public enum CacheMode
        {
            WinINet,
            Dedicated
        }

        static FileCache()
        {
            // default cache directory - can be changed if needed from App.xaml
            AppCacheDirectory = Path.Combine(Path.GetTempPath(), nameof(VirtualizingWrapPanel));
            AppCacheMode = CacheMode.Dedicated;
        }


        // Record whether a file is being written.
        private static readonly Dictionary<string, bool> IsWritingFile = new Dictionary<string, bool>();
        private static readonly string DownloadingFileExtension = ".download";
        private static readonly SemaphoreSlim SyncObject = new SemaphoreSlim(5);
        public static int DownloadTimeoutMillisecond { get; set; } = 20000; // 20sec


        /// <summary>
        ///     Gets or sets the path to the folder that stores the cache file. Only works when AppCacheMode is
        ///     CacheMode.Dedicated.
        /// </summary>
        public static string AppCacheDirectory { get; set; }

        /// <summary>
        ///     Gets or sets the cache mode. WinINet is recommended, it's provided by .Net Framework and uses the Temporary Files
        ///     of IE and the same cache policy of IE.
        /// </summary>
        public static CacheMode AppCacheMode { get; set; }


        public static async Task<MemoryStream> HitAsync(string url)
        {
            if (!Directory.Exists(AppCacheDirectory))
            {
                Directory.CreateDirectory(AppCacheDirectory);
            }
            var uri = new Uri(url);
            var fileNameBuilder = new StringBuilder();
            using (var sha1 = new SHA1Managed())
            {
                var canonicalUrl = uri.ToString();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(canonicalUrl));
                fileNameBuilder.Append(BitConverter.ToString(hash).Replace("-", "").ToLower());
                if (Path.HasExtension(canonicalUrl))
                    fileNameBuilder.Append(Path.GetExtension(canonicalUrl.RemoveQueryParams()));
            }

            var fileName = fileNameBuilder.ToString();
            var localFile = $"{AppCacheDirectory}\\{fileName}";
            var memoryStream = new MemoryStream();

            FileStream fileStream = null;
            if (!IsWritingFile.ContainsKey(fileName) && File.Exists(localFile))
            {
                await using (fileStream = new FileStream(localFile, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }

            var request = WebRequest.Create(uri);
            request.Timeout = DownloadTimeoutMillisecond;
            try
            {
                await SyncObject.WaitAsync();
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return null;
                if (!IsWritingFile.ContainsKey(fileName))
                {
                    IsWritingFile[fileName] = true;
                    localFile = localFile + DownloadingFileExtension;
                    fileStream = new FileStream(localFile, FileMode.Create, FileAccess.Write);
                }

                await using (responseStream)
                {
                    var byteBuffer = new byte[100];
                    int bytesRead;
                    do
                    {
                        bytesRead = await responseStream.ReadAsync(byteBuffer, 0, 100);
                        if (fileStream != null)
                            await fileStream.WriteAsync(byteBuffer, 0, bytesRead);
                        await memoryStream.WriteAsync(byteBuffer, 0, bytesRead);
                    } while (bytesRead > 0);

                    if (fileStream != null)
                    {
                        await fileStream.FlushAsync();
                        await fileStream.DisposeAsync();
                        IsWritingFile.Remove(fileName);
                        File.Move(localFile, localFile.Replace(DownloadingFileExtension, ""));
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch
            {
                // ignore exception
                return null;
            }
            finally
            {
                SyncObject.Release(1);
            }
        }

        [DebuggerHidden]
        public static string RemoveQueryParams(this string url)
        {
            var qIndex = url.IndexOf('?');
            if (qIndex < 0)
                return url;
            return url.Remove(qIndex);
        }
    }
}
