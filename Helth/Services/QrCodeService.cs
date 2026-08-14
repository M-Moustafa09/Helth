using QRCoder;

namespace Helth.Services;

public class QrCodeService : IQrCodeService
{
    public byte[] GeneratePngBytes(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(data);
        return pngQrCode.GetGraphic(20);
    }
}
