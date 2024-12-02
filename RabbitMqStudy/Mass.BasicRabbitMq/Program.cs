using Mass.BasicRabbitMq;
using MassTransit;

try
{
    var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
    {
        // 消费
        sbc.Host("localhost",5672, "admin_vhost",h=>
        {
            h.Username("guest");
            h.Password("guest");
        });

        sbc.ReceiveEndpoint("test_queue", ep =>
        {
            ep.Handler<Message>(context =>
            {
                Console.Out.WriteLine($"Received: {context.Message.Text}");
                return Task.CompletedTask;
            });
        });
    });
    await bus.StartAsync(); // This is important!

    await bus.Publish(new Message { Text = "Hi" });


    Console.WriteLine("Please input your message with enter:");
    string? message = Console.ReadLine();

    while (message != "EXIT")
    {
        await bus.Publish(new Message { Text = message });
        message = Console.ReadLine();
    }

    await bus.StopAsync();

    Console.WriteLine("Hello World!");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}