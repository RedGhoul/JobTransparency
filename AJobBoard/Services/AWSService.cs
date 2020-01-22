using AJobBoard.Areas.Identity.Pages.Account;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace AJobBoard.Services
{
    public class AWSService : IAWSService
    {
        private AmazonS3Client S3Client;
        private long MAX_FILE_SIZE = 4048576;

        public AWSService(IConfiguration configuration)
        {
            string AccessKeyId = configuration.GetSection("AppSettings")["AKIDS3"];
            string AwsSecretKey = configuration.GetSection("AppSettings")["SAK"];
            S3Client = new AmazonS3Client(AccessKeyId, AwsSecretKey, Amazon.RegionEndpoint.USEast1);
        }

        public async Task<string> UploadStreamToBucket(string bucket, string key, string contentType, Stream stream)
        {
            Console.WriteLine("Uploading file to AWS: {0}/{1}", bucket, key);
            PutObjectRequest PutRequest = new PutObjectRequest
            {
                InputStream = stream,
                BucketName = bucket,
                Key = key,
                ContentType = contentType,
            };

            await S3Client.PutObjectAsync(PutRequest);

            return key;
        }


        public void validateFile(IFormFile formFile, ModelStateDictionary modelState)
        {
            var fieldDisplayName = string.Empty;
            string fileName = GetFileName(formFile);

            if (formFile.ContentType.ToLower() != "text/plain" &&
                formFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                && formFile.ContentType.ToLower() != "application/pdf")
            {
                modelState.AddModelError(formFile.Name,
                    $"The {fieldDisplayName}file ({fileName}) must be a text file.");
            }

            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.Name,
                    $"The {fieldDisplayName}file ({fileName}) is empty.");
            }
            else if (formFile.Length > MAX_FILE_SIZE)
            {
                modelState.AddModelError(formFile.Name,
                    $"The {fieldDisplayName}file ({fileName}) exceeds 5 MB.");
            }
        }

        public string GetFileName(IFormFile formFile)
        {
            MemberInfo property =
                            typeof(RegisterJobSeekerModel.InputModel).GetProperty(
                                formFile.Name.Substring(formFile.Name.IndexOf(".") + 1));

            if (property != null)
            {
                DisplayAttribute displayAttribute =
                    property.GetCustomAttribute(typeof(DisplayAttribute))
                        as DisplayAttribute;
            }

            var fileName = WebUtility.HtmlEncode(
                Path.GetFileName(formFile.FileName));
            return fileName;
        }

        public async Task<byte[]> GetFileInBytes(string fileName, string bucket)
        {
            Console.WriteLine("Downloading file from AWS: {0}/{1}", bucket, fileName);
            // Create a GetObject request
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = fileName
            };

            GetObjectResponse response = await S3Client.GetObjectAsync(request);
            Stream responseStream = response.ResponseStream;
            byte[] bytes = ReadStreamToBytes(responseStream); // helper method below
            return bytes;
        }

        public static byte[] ReadStreamToBytes(Stream responseStream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task DeleteFile(string fileName, string bucketName)
        {


            try
            {
                DeleteObjectRequest ObjectDeleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                };

                Console.WriteLine("Deleting an object");
                DeleteObjectResponse response = await S3Client.DeleteObjectAsync(ObjectDeleteRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }


        }

    }
}
