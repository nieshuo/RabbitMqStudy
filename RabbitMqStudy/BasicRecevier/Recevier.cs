using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory
{
    HostName = "localhost", //主机地址
    VirtualHost = "admin_vhost",    //虚拟机名
    Port = 5672,    //端口号
    UserName = "guest", //用户名
    Password = "guest"  //密码
};

using (var connection = await factory.CreateConnectionAsync())
{
    using (var channel = await connection.CreateChannelAsync())
    {
        await channel.QueueDeclareAsync(
            queue: "hello", //信道
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");

            Thread.Sleep(2000);

            channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);//手动处理消息

            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync("hello", autoAck: false, consumer: consumer);
        // await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);//自动处理消息

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}