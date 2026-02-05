using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Serivce.Kafka;

var builder = Host.CreateApplicationBuilder(args);

// Add services to the container.
var kafkaConnect = builder.Configuration.GetConnectionString("kafkaConnect");


builder.Services.AddMassTransit(x =>
{
    // Регистрируем consumer
    x.AddConsumer<PaymentCreatedConsumer>();

    x.UsingInMemory((context, cfg) =>
    {
        // Можно оставить пустым или убрать, если не используете InMemory
    });

    x.AddRider(rider =>
    {
        // Добавляем consumer для Kafka
        rider.AddConsumer<PaymentCreatedConsumer>();

        // Если вам нужно также отправлять сообщения
        rider.AddProducer<CreatePaymentRequest>("payment.created");

        rider.UsingKafka((context, k) =>
        {
            k.Host(kafkaConnect);

            // Настраиваем топик для чтения
            k.TopicEndpoint<CreatePaymentRequest>(
                "payment.created",      // имя топика
                "pps-consumer-group",   // consumer group id (уникальный для вашего приложения)
                e =>
                {
                    // Настройка retry политики (опционально)
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    // Подключаем consumer
                    e.ConfigureConsumer<PaymentCreatedConsumer>(context);
                });
        });
    });
});


var connect = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connect)
);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


app.Run();
