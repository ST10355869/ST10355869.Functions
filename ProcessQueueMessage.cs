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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
        string queueName = req.Query["queueName"];
        string message = req.Query["message"];

        var queueServiceClient = new QueueServiceClient(connectionString);
        var queueClient = queueServiceClient.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync();
        await queueClient.SendMessageAsync(message);

        return new OkObjectResult("Message added to queue");
    }
}
