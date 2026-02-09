//using FraudDetectionService;

//var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

//var host = builder.Build();
//host.Run();



using FraudDetectionService.Service;
using MassTransit;

using PPS.Common.KafkaDto;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

var kafkaConnect = builder.Configuration.GetConnectionString("KafkaConnect");

Console.WriteLine($"=== Kafka Bootstrap Server: {kafkaConnect} ===");

if (string.IsNullOrEmpty(kafkaConnect))
{
    throw new InvalidOperationException("KafkaConnect connection string is not configured!");
}

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRider(rider =>
    {
        rider.AddConsumer<FraudConsumer>();
        rider.AddProducer<BaseMessageKafka<FraudPaymentDto>>("payment.fraud-checked");

        //rider.AddProducer<Payment>("payment.completed");

        rider.UsingKafka((context, k) =>
        {
            k.Host(kafkaConnect);

            k.TopicEndpoint<BaseMessageKafka<ValidatedPaymentDto>>(
                "payment.validated",
                "Fraud-service-group",
                e =>
                {
                    e.CreateIfMissing(t =>
                    {
                        t.NumPartitions = 1;
                        t.ReplicationFactor = 1;
                    });
                    e.ConfigureConsumer<FraudConsumer>(context);
                });
        });
    });
});


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration["Redis:ConnectionString"]!);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<IRedisService, RedisService>();

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
});



var app = builder.Build();


app.Run();
