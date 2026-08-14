namespace Helth.Services;

public interface IPhotoService
{
    Task<string> SavePhotoAsync(IFormFile file);
    void DeletePhoto(string? photoPath);
}
