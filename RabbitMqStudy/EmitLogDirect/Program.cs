using RabbitMQ.Client;
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
        // string exchangeName = "direct_logs";
        string exchangeName = "header.fanout";
        //声明交换机
        /* Direct：直连，需要绑定路由
         * Fanout：发布订阅广播，不需要绑定路由
         * Topic：发布订阅-路由，可以根据*，#模糊匹配路由
         * Headers：允许匹配消息的头部（headers）而不是路由键
        */
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,//交换机名称
            type: ExchangeType.Headers,//交换机类型
            durable: true,
            autoDelete: true,
            arguments: null
            );


        // 需要传参数，如：dotnet run info hello
        var severity = (args.Length > 0) ? args[0] : "info";

        var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";

        var body = Encoding.UTF8.GetBytes(message);

        // 声明Headers,并把路由设空
        IDictionary<string,dynamic?> headers = new Dictionary<string, dynamic?>();
        headers.Add("key1", "value1");
        headers.Add("key2", 111);

        var properties = new BasicProperties
        {
            Persistent = true,//持久化的
            Headers = headers
        };

        // 声明一个信道，在发送端不创建信道，会导致如果先开启发送端，这时发送消息，再开启消费端，消费端获取不到消息
        var queue = await channel.QueueDeclareAsync(
            queue: "emilt_log", //信道
            durable: false,
            exclusive: false,
            autoDelete: true,
            arguments: null
         );
        string queueName = queue.QueueName;//信道名称
        // 绑定交换机到信道
        await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: String.Empty,
                arguments: headers
                );

        // 发送消息
        await channel.BasicPublishAsync(
            exchange: exchangeName, //交换机
            // routingKey: severity, //路由
            routingKey:String.Empty,
            mandatory: true, //强制性的
            basicProperties: properties,
            body: body
        );

        Console.WriteLine($" [x] Sent {severity}:{message}");
    }
}