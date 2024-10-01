using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public class UploadBlob
{
    private readonly ILogger _logger;

    public UploadBlob(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UploadBlob>();
    }

    [Function("UploadBlob")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string containerName = req.Query["containerName"];
        string blobName = req.Query["blobName"];

        if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Container name and blob name must be provided.");
            return badRequestResponse;
        }

        try
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = req.Body;
            await blobClient.UploadAsync(stream, overwrite: true);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Blob uploaded successfully. URL: {blobClient.Uri}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blob");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("An error occurred while uploading the blob.");
            return errorResponse;
        }
    }
}