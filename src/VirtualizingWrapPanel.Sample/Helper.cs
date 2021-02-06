using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace VirtualizingWrapPanel.Sample
{
    public static class Helper
    {
        public static string Hash(this string text)
        {
            if (text == null)
                return null;

            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(text.ToIsoBytes());
            return hashBytes.ToHex();
        }

        public static byte[] ToIsoBytes(this string text)
        {
            var iso = Encoding.GetEncoding("ISO-8859-1");
            var utfBytes = Encoding.UTF8.GetBytes(text);
            var isoBytes = Encoding.Convert(Encoding.UTF8, iso, utfBytes);
            return isoBytes;
        }

        public static string ToHex(this byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
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
