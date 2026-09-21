using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class FileUserDirectoryController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "uploads";

    public FileUserDirectoryController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // -------------------------------
    // Existing Upload Endpoint
    // -------------------------------
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

    // -------------------------------
    // NEW: Create User Directory
    // Route: /api/File/UserDirectory
    // -------------------------------
    [HttpPost("UserDirectory")]
    public async Task<Dictionary<string, string>> CreateUserDirectory(string username)
    {
        var response = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            response["message"] = "Username is required.";
            return response;
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Virtual directory path
        string directoryPath = $"{username}/";
        string placeholderBlobName = $"{directoryPath}placeholder.txt";

        var blobClient = containerClient.GetBlobClient(placeholderBlobName);

        using var stream = new MemoryStream(new byte[0]);
        await blobClient.UploadAsync(stream, overwrite: true);

        response["message"] = "User directory created successfully.";
        response["directory"] = directoryPath;
        response["placeholderBlob"] = blobClient.Uri.ToString();

        return response;
    }
}
