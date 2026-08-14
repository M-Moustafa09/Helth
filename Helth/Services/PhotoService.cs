namespace Helth.Services;

public class PhotoService : IPhotoService
{
    private const string UploadsFolder = "uploads/photos";
    private readonly IWebHostEnvironment _env;

    public PhotoService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SavePhotoAsync(IFormFile file)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, UploadsFolder);
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/{UploadsFolder}/{fileName}";
    }

    public void DeletePhoto(string? photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
        {
            return;
        }

        var fullPath = Path.Combine(_env.WebRootPath, photoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
