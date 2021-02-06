using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
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
            AppCacheDirectory = "D:\\TestVirtualizing\\";
            AppCacheMode = CacheMode.Dedicated;
        }

        // Record whether a file is being written.
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> WritingSemaphore = new ConcurrentDictionary<string, SemaphoreSlim>();
        private static readonly string DownloadingFileExtension = ".download";

        public static int TimeoutMillisecond { get; set; } = 20000; // 20sec

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
            SemaphoreSlim syncSemaphoreSlim = null;
            try
            {
                if (!Directory.Exists(AppCacheDirectory))
                {
                    Directory.CreateDirectory(AppCacheDirectory);
                }

                var fileName = GetUniqueFileName(url);
                var localFile = GetFilePath(fileName);

                syncSemaphoreSlim = WritingSemaphore.GetOrAdd(fileName, new SemaphoreSlim(1));
                await syncSemaphoreSlim.WaitAsync(TimeoutMillisecond);
                if (File.Exists(localFile))
                {
                    return await GetFileFromDisk(localFile);
                }

                return await DownloadFile(url, fileName);
            }
            finally
            {
                syncSemaphoreSlim?.Release(1);
            }
        }

        private static string GetUniqueFileName(string url)
        {
            var fileName = url.Hash();
            fileName += Path.HasExtension(url) ? Path.GetExtension(url.RemoveQueryParams()) : "";
            return fileName;
        }
        private static string GetFilePath(string fileName)
        {
            return $"{AppCacheDirectory}\\{fileName}";
        }
        private static async Task<MemoryStream> GetFileFromDisk(string filename)
        {
            var memoryStream = new MemoryStream();
            await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        private static async Task<MemoryStream> DownloadFile(string url, string fileName)
        {
            try
            {
                var localFile = GetFilePath(fileName);
                localFile = localFile + DownloadingFileExtension;
                var memoryStream = new MemoryStream();
                var request = WebRequest.Create(new Uri(url));
                request.Timeout = TimeoutMillisecond;
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return null;

                await using (responseStream)
                {
                    var fileStream = new FileStream(localFile, FileMode.Create, FileAccess.Write, FileShare.Delete | FileShare.Read);
                    var byteBuffer = new byte[100];
                    int bytesRead;
                    do
                    {
                        bytesRead = await responseStream.ReadAsync(byteBuffer, 0, 100);
                        await fileStream.WriteAsync(byteBuffer, 0, bytesRead);
                        await memoryStream.WriteAsync(byteBuffer, 0, bytesRead);
                    } while (bytesRead > 0);

                    await fileStream.FlushAsync();
                    await fileStream.DisposeAsync();
                    File.Move(localFile, localFile.Replace(DownloadingFileExtension, ""));
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch
            {
                // ignore exception
                return null;
            }
        }
    }
}
