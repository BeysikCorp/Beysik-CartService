using SQLite;
using Beysik_CartService.Services;
using Beysik_Common;
using RabbitMQ.Client;

namespace Beysik_CartService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSingleton<CartService>();
            //builder.Services.AddSingleton<RabbitMqHelper>(sp => new RabbitMqHelper(builder.Configuration["RabbitMQConnection"]));
            builder.Services.AddSingleton<RabbitMqHelper>(sp => new RabbitMqHelper(
                builder.Configuration.GetSection("RabbitMQ").GetSection("HostName").Value));
            //builder.Services.AddSingleton<RabbitMqConsumerService>(
            //);

            builder.Services.AddSingleton<RabbitMqEventAggregator>();

            builder.Services.AddHostedService<RabbitMqConsumerService>(sp =>
                new RabbitMqConsumerService(
                sp.GetRequiredService<RabbitMqHelper>(),
                sp.GetRequiredService<RabbitMqEventAggregator>(),
                "order.tocart",
                ExchangeType.Topic,
                "*.frompc"
                )
            );

            builder.Services.AddHostedService<RabbitMqConsumerService>(sp =>
                new RabbitMqConsumerService(
                sp.GetRequiredService<RabbitMqHelper>(),
                sp.GetRequiredService<RabbitMqEventAggregator>(),
                "order.tocart",
                ExchangeType.Topic,
                "*.fromorder"
                )
            );

            builder.Services.AddHostedService<RabbitMqConsumerService>(sp =>
                new RabbitMqConsumerService(
                sp.GetRequiredService<RabbitMqHelper>(),
                sp.GetRequiredService<RabbitMqEventAggregator>(),
                "api",
                ExchangeType.Fanout,
                null
                )
            );


            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton<ISQLiteConnection>(new SQLiteConnection(connectionString));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
