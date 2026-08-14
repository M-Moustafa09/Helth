namespace Helth.Services;

public interface IQrCodeService
{
    byte[] GeneratePngBytes(string content);
}
