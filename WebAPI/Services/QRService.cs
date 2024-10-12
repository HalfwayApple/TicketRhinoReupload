using QRCoder;
using System.Drawing;
using static QRCoder.PayloadGenerator;

namespace WebAPI.Services;

public class QRService
{
	public Guid GenerateGuid()
	{
		return Guid.NewGuid();
	}

	public byte[] GenerateQRCode(string guid)
	{
		// Create payload
		Url url = new Url($"https://www.google.com/search?q={guid}");
		string payload = url.ToString();
		
		// Generate QRcode image data
		QRCodeGenerator qrGenerator = new QRCodeGenerator();
		QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
		BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
		byte[] qrCodeImageData = qrCode.GetGraphic(20);

		return qrCodeImageData;
	}

	public Image RefineQRDataIntoImage(byte[] imageData)
	{
		using (MemoryStream memoryStream = new MemoryStream(imageData))
		{
			Image image = Image.FromStream(memoryStream);
			return image;
		}
	}
}
