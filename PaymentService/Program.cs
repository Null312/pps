using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Serivce.Kafka;
using PPS.Common;

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
        rider.AddConsumer<PaymentCreatedConsumer>();
        rider.AddProducer<Payment>("payment.processed");

        rider.UsingKafka((context, k) =>
        {
            k.Host(kafkaConnect);

            k.TopicEndpoint<Payment>(
                "payment.created",
                "payment-service-group",
                e =>
                {
                    e.CreateIfMissing(t =>
                    {
                        t.NumPartitions = 1;
                        t.ReplicationFactor = 1;
                    });
                    e.ConfigureConsumer<PaymentCreatedConsumer>(context);
                });
        });
    });
});

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
});

var connect = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connect)
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
