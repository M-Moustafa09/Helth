using Helth.Models;

namespace Helth.Services;

public interface IPdfService
{
    byte[] GenerateEmployeeCertificate(Employee employee, string publicUrl, string webRootPath);
}
