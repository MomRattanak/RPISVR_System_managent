using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using System.IO;

namespace RPISVR_Managements.ViewModel
{
    public static class QRCodeService
    {
        public static byte[] GenerateQRCode(string studentInfoUrl)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(studentInfoUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                // Increase the size to enhance readability
                using (var qrCodeImage = qrCode.GetGraphic(10)) // Increase multiplier
                using (var memoryStream = new MemoryStream())
                {
                    qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    return memoryStream.ToArray();
                }
            }
        }

    }
}
