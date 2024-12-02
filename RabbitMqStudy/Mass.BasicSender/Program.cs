using Mass.BasicInMemory;
using MassTransit;

var bus = Bus.Factory.CreateUsingInMemory(sbc =>
{
    // 消费
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