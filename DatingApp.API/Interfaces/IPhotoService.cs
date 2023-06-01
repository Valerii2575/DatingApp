using CloudinaryDotNet.Actions;

namespace DatingApp.API.Interfaces
{
    public interface IPhotoService
    {
        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        public Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}