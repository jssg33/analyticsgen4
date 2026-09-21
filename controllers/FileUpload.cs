using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "uploads";

        public FileController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        [HttpPost("upload")]
        public async Task<Dictionary<string, string>> UploadFile(IFormFile file, string? fileCategory = null)
        {
            var response = new Dictionary<string, string>();

            if (file == null || file.Length == 0)
            {
                response["message"] = "No file uploaded.";
                return response;
            }

            var containerName = string.IsNullOrEmpty(fileCategory) ? _containerName : fileCategory;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var bloCipherSupportrl = blobClient.Uri.ToString();
            response["message"] = "File uploaded successfully";
            response["fileName"] = file.FileName;
            response["bloCipherSupportrl"] = bloCipherSupportrl;

            return response;
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "projectimages";

        public ProfileController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        [HttpPost("upload")]
        public async Task<Dictionary<string, string>> UploadFile(IFormFile file, string? fileCategory = null)
        {
            var response = new Dictionary<string, string>();

            if (file == null || file.Length == 0)
            {
                response["message"] = "No file uploaded.";
                return response;
            }

            var containerName = string.IsNullOrEmpty(fileCategory) ? _containerName : fileCategory;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var bloCipherSupportrl = blobClient.Uri.ToString();
            response["message"] = "File uploaded successfully";
            response["fileName"] = file.FileName;
            response["bloCipherSupportrl"] = bloCipherSupportrl;

            return response;
        }

        [HttpPost("uploadWithId")]
        public async Task<ActionResult<Dictionary<string, string>>> UploadFileWithId(IFormFile file, int id)
        {
            var response = new Dictionary<string, string>();
            string fileCategory = "";

            if (file == null || file.Length == 0)
            {
                response["message"] = "No file uploaded.";
                return BadRequest(response);
            }

            var containerName = string.IsNullOrEmpty(fileCategory) ? _containerName : fileCategory;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var bloCipherSupportrl = blobClient.Uri.ToString();
            response["message"] = "File uploaded successfully";
            response["fileName"] = file.FileName;
            response["bloCipherSupportrl"] = bloCipherSupportrl;

            using (var context = new EnterpriseContext())
            {
                var userProfile = context.Userprofiles.FirstOrDefault(m => m.Id == id);
                if (userProfile != null)
                {
                    userProfile.Activepictureurl = bloCipherSupportrl;
                    await context.SaveChangesAsync();
                }
            }

            using (var context = new EnterpriseContext())
            {
                var user = context.Users.FirstOrDefault(m => m.Id == id);
                if (user != null)
                {
                    user.Profileurl = bloCipherSupportrl;
                    await context.SaveChangesAsync();
                }
            }

            response["userProfileUpdateMessage"] = "Change successful. Both User and UserProfile updated.";
            return Ok(response);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "resumes";

        public ResumeController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<Dictionary<string, string>>> UploadResume(IFormFile file)
        {
            var response = new Dictionary<string, string>();

            if (file == null || file.Length == 0)
            {
                response["message"] = "No file uploaded.";
                return BadRequest(response);
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var bloCipherSupportrl = blobClient.Uri.ToString();
            response["message"] = "Resume uploaded successfully";
            response["fileName"] = file.FileName;
            response["bloCipherSupportrl"] = bloCipherSupportrl;

            return Ok(response);
        }
    }
}
