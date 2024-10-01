using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public static class ProcessQueueMessage
{
    [Function("ProcessQueueMessage")]
    public static async Task<IActionResult> Run(
     [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
     ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string queueName = req.Query["queueName"];
        string message = req.Query["message"];

        if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
        {
            return new BadRequestObjectResult("Please pass a queueName and message on the query string");
        }

        try
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);

            return new OkObjectResult($"Message '{message}' sent to queue '{queueName}'");
        }
        catch (Exception ex)
        {
            log.LogError($"Error: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}