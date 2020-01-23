using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AJobBoard.Services
{
    public interface IAWSService
    {
        Task DeleteFile(string fileName, string bucketName);
        Task<byte[]> GetFileInBytes(string fileName, string bucket);
        string GetFileName(IFormFile formFile);
        Task<string> UploadStreamToBucket(string bucket, string key, string contentType, Stream stream);
        void validateFile(IFormFile formFile, ModelStateDictionary modelState);
    }
}