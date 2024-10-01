using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.Storage.Blobs;
using Microsoft.Azure.WebJobs.Extensions.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .ConfigureWebJobs(b =>
    {
        // Register specific storage bindings
        b.AddHttp();
        b.AddAzureStorageBlobs(); // For Blob Storage functions
        b.AddAzureStorageQueues(); // For Queue Storage functions
        // b.AddAzureStorageQueuesScaleForTrigger(); // Add this if scaling is needed for Queue Triggers
    })
    .Build();

host.Run();
