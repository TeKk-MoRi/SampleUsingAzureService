using Azure.Messaging.ServiceBus;

namespace Payment.Worker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionString = _configuration["ServiceBus:ConnectionString"];
        var queueName = _configuration["ServiceBus:QueueName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName))
        {
            _logger.LogWarning("Service Bus is not configured. Worker is running but not consuming messages.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            return;
        }

        _client = new ServiceBusClient(connectionString);

        _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        _logger.LogInformation("Payment.Worker started listening to queue {QueueName}", queueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        _logger.LogInformation("Received OrderCreated message: {MessageBody}", body);

        // Simulate payment processing
        await Task.Delay(TimeSpan.FromSeconds(1));

        _logger.LogInformation(
            "Payment processed successfully. MessageId: {MessageId}",
            args.Message.MessageId);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(
            args.Exception,
            "Service Bus error. Entity: {EntityPath}, ErrorSource: {ErrorSource}",
            args.EntityPath,
            args.ErrorSource);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        if (_client is not null)
        {
            await _client.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}