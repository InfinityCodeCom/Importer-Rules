/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

using System.IO;

namespace InfinityCode.ImporterRules
{
    public static class ImporterRuleImageUtils
    {
        private static bool GetBMPSize(FileStream fs, ref int width, ref int height)
        {
            byte[] bytes = new byte[8];
            fs.Seek(18, SeekOrigin.Begin);
            fs.Read(bytes, 0, 8);

            for (int i = 3; i >= 0; i--)
            {
                width = bytes[i] | width << 8;
                height = bytes[i + 4] | height << 8;
            }

            return true;
        }

        private static bool GetGIFSize(FileStream fs, ref int width, ref int height)
        {
            byte[] bytes = new byte[4];
            fs.Seek(6, SeekOrigin.Begin);
            fs.Read(bytes, 0, 4);
            width = bytes[0] | bytes[1] << 8;
            height = bytes[2] | bytes[3] << 8;
            return true;
        }

        public static bool GetImageSize(string filename, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (!File.Exists(filename)) return false;

            FileInfo info = new FileInfo(filename);
            string ext = info.Extension.ToLower().Substring(1);
            bool status = false;
            FileStream fs = File.OpenRead(filename);

            if (ext == "png") status = GetPNGSize(fs, ref width, ref height);
            else if (ext == "jpg" || ext == "jpeg") status = GetJPEGSize(fs, ref width, ref height);
            else if (ext == "gif") status = GetGIFSize(fs, ref width, ref height);
            else if (ext == "bmp") status = GetBMPSize(fs, ref width, ref height);
            else if (ext == "tga") status = GetTGASize(fs, ref width, ref height);
            else if (ext == "psd") status = GetPSDSize(fs, ref width, ref height);

            fs.Close();

            return status;
        }

        private static bool GetJPEGSize(FileStream fs, ref int width, ref int height)
        {
            byte[] buf = new byte[4];
            fs.Read(buf, 0, 4);
            long blockStart = fs.Position;
            fs.Read(buf, 0, 2);
            long blockLength = ((buf[0] << 8) + buf[1]);
            fs.Read(buf, 0, 4);
            if (System.Text.Encoding.ASCII.GetString(buf, 0, 4) == "JFIF"
                && fs.ReadByte() == 0)
            {
                blockStart += blockLength;
                while (blockStart < fs.Length)
                {
                    fs.Position = blockStart;
                    fs.Read(buf, 0, 4);
                    blockLength = ((buf[2] << 8) + buf[3]);
                    if (blockLength >= 7 && buf[0] == 0xff && buf[1] == 0xc0)
                    {
                        fs.Position += 1;
                        fs.Read(buf, 0, 4);
                        height = (buf[0] << 8) + buf[1];
                        width = (buf[2] << 8) + buf[3];
                        return true;
                    }
                    blockStart += blockLength + 2;
                }
            }
            return false;
        }

        private static bool GetPNGSize(FileStream fs, ref int width, ref int height)
        {
            byte[] bytes = new byte[8];
            fs.Seek(16, SeekOrigin.Begin);
            fs.Read(bytes, 0, 8);

            for (int i = 0; i <= 3; i++)
            {
                width = bytes[i] | width << 8;
                height = bytes[i + 4] | height << 8;
            }

            return true;
        }

        private static bool GetPSDSize(FileStream fs, ref int width, ref int height)
        {
            byte[] bytes = new byte[8];
            fs.Seek(14, SeekOrigin.Begin);
            fs.Read(bytes, 0, 8);
            for (int i = 0; i <= 3; i++)
            {
                width = bytes[i + 4] | width << 8;
                height = bytes[i] | height << 8;
            }
            return true;
        }

        private static bool GetTGASize(FileStream fs, ref int width, ref int height)
        {
            byte[] bytes = new byte[4];
            fs.Seek(12, SeekOrigin.Begin);
            fs.Read(bytes, 0, 4);
            width = bytes[0] | bytes[1] << 8;
            height = bytes[2] | bytes[3] << 8;
            return true;
        }
    }
}