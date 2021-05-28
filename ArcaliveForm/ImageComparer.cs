using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace ArcaliveForm
{
    public static class ImageComparer
    {
        [Obsolete("Use 'DownloadBytesFromUrl' instead")]
        public static Bitmap DownloadImageFromUrl(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Arcalive.ArcaliveCrawler.ArcaliveUserAgent);
                using (Stream stream = client.OpenRead(url))
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[16 * 2048];
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                            Bitmap bitmap = (Bitmap)tc.ConvertFrom(ms.ToArray());
                            return bitmap;
                        }
                    }
                    catch (Exception e)
                    {
                        string filename = url.Split('/').Last();
                        client.DownloadFile(url, filename);
                        Bitmap bitmap = new Bitmap(filename);
                        return bitmap;
                        throw;
                    }
                }
            }
        }

        public static byte[] DownloadBytesFromUrl(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Arcalive.ArcaliveCrawler.ArcaliveUserAgent);
                using (Stream stream = client.OpenRead(url))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[16 * 2048];
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }

                        return ms.ToArray();
                    }
                }
            }
        }

        public static List<bool> GetImageHash(Bitmap src)
        {
            List<bool> result = new List<bool>();
            Bitmap bmpMin = new Bitmap(src, new Size(16, 16));

            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    result.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }

            return result;
        }

        public static bool Compare(Bitmap src1, Bitmap src2)
        {
            List<bool> hash1 = GetImageHash(src1);
            List<bool> hash2 = GetImageHash(src2);

            return hash1.Zip(hash2, (i, j) => i != j).Count(eq => eq) == 0;
        }

        public static bool Compare(List<bool> hash1, List<bool> hash2)
        {
            return hash1.Zip(hash2, (i, j) => i != j).Count(eq => eq) == 0;
        }

        public static unsafe bool Compare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            var len = a.Length;
            fixed (byte* ap = a, bp = b)
            {
                long* alp = (long*)ap, blp = (long*)bp;
                for (; len >= 8; len -= 8)
                {
                    if (*alp != *blp) return false;
                    alp++;
                    blp++;
                }
                byte* ap2 = (byte*)alp, bp2 = (byte*)blp;
                for (; len > 0; len--)
                {
                    if (*ap2 != *bp2) return false;
                    ap2++;
                    bp2++;
                }
            }
            return true;
        }
    }
}