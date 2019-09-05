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

namespace AJobBoard.Services
{
    public static class S3Uploader
    {
        static long MAX_FILE_SIZE = 4048576;

        public static async Task<string> UploadFromRegister(IFormFile formFile,
            ModelStateDictionary modelState)
        {
            var fieldDisplayName = string.Empty;

            MemberInfo property =
                typeof(RegisterJobSeekerModel.InputModel).GetProperty(
                    formFile.Name.Substring(formFile.Name.IndexOf(".") + 1));

            if (property != null)
            {
                DisplayAttribute displayAttribute =
                    property.GetCustomAttribute(typeof(DisplayAttribute))
                        as DisplayAttribute;

                if (displayAttribute != null)
                {
                    fieldDisplayName = $"{displayAttribute.Name} ";
                }
            }

            var fileName = WebUtility.HtmlEncode(
                Path.GetFileName(formFile.FileName));

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
                    $"The {fieldDisplayName}file ({fileName}) exceeds 1 MB.");
            }
            else
            {
                try
                {
                    string fileContents;

                    using (
                        var reader =
                            new StreamReader(
                                formFile.OpenReadStream(),
                                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                                    throwOnInvalidBytes: true),
                                detectEncodingFromByteOrderMarks: true))
                    {
                        fileContents = await reader.ReadToEndAsync();

                        // do some S3 Upload here
                        if (fileContents.Length > 0)
                        {
                            return fileContents;
                        }
                        else
                        {
                            modelState.AddModelError(formFile.Name,
                                $"The {fieldDisplayName}file ({fileName}) is empty.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    modelState.AddModelError(formFile.Name,
                        $"The {fieldDisplayName}file ({fileName}) upload failed. " +
                        $"Please contact the Help Desk for support. Error: {ex.Message}");

                }
            }

            return string.Empty;
        }
    }
}
