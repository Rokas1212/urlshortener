using System;
using System.Drawing;
using System.Drawing.Imaging;
using Net.Codecrete.QrCodeGenerator;

namespace UrlShortener.Services
{
    public class QrCodeService
    {
        public string GenerateQrCode(string url)
        {
            var qr = QrCode.EncodeText(url, QrCode.Ecc.Medium);

            const int scale = 10;
            var size = qr.Size * scale;
            var padding = 4 * scale;

            using (var bitmap = new Bitmap(size + 2 * padding, size + 2 * padding))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        for (int y = 0; y < qr.Size; y++)
                        {
                            for (int x = 0; x < qr.Size; x++)
                            {
                                if (qr.GetModule(x, y))
                                {
                                    graphics.FillRectangle(brush, padding + x * scale, padding + y * scale, scale, scale);
                                }
                            }
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    var base64 = Convert.ToBase64String(stream.ToArray());
                    return base64;
                }
            }
        }
    }
}
