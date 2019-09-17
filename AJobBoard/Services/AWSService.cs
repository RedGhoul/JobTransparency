using AJobBoard.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AJobBoard.Services
{
    public class AWSService
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
                BucketName = bucket ,
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
    }
}
