using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory
{
    HostName = "localhost", //主机地址
    VirtualHost = "admin_vhost",    //虚拟机名,默认"/"
    Port = 5672,    //端口号,默认5672
    UserName = "guest", //用户名
    Password = "guest"  //密码
};
using (var connection = await factory.CreateConnectionAsync())
{
    using (var channel = await connection.CreateChannelAsync())
    {
        //string exchangeName = "direct_logs";
        string exchangeName = "header.fanout";
        //声明交换机
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,//交换机名称
            type: ExchangeType.Headers,//交换机类型
            durable: true,
            autoDelete: true,
            arguments: null
            );

        // 声明一个队列
        // QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        // string queueName = queueDeclareResult.QueueName;//队列名称

        // 声明一个信道
        var queue = await channel.QueueDeclareAsync(
            queue: "emilt_log", //信道
            durable: false,
            exclusive: false,
            autoDelete: true,
            arguments: null
         );
        string queueName = queue.QueueName;//信道名称

        // 需要传参数，如：dotnet run info warning error

        if (args.Length < 1)
        {
            Console.Error.WriteLine(
                "Usage: {} [info] [warning] [error]",
                Environment.GetCommandLineArgs()[0]);
            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
            Environment.ExitCode = 1;
            return;
        }
        // 声明Headers,并把路由设空
        IDictionary<string, dynamic?> headers = new Dictionary<string, dynamic?>();
        headers.Add("key1", "value1");
        headers.Add("key2", 111);

        foreach (var severity in args)
        {
            // 绑定队列
            //await channel.QueueBindAsync(
            //    queue: queueName,
            //    exchange: exchangeName,
            //    routingKey: severity
            //    );
            // headers类型绑定队列
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: String.Empty,
                arguments: headers
                );
        }
        // 生成消费者接收消息
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");

            channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);//手动处理消息

            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}