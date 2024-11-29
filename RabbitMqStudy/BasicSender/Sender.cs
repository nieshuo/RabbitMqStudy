using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { 
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
        await channel.QueueDeclareAsync(
            queue: "hello", //信道
            durable: false, //如果true，则此种队列叫持久化队列（Durable queues）.此队列会被存储在磁盘上,当消息代理（broker）重启的时候，它依旧存在。没有被持久化的队列称作暂存队列（Transient queues）
            exclusive: false,   //排他,表示此对应只能被当前创建的连接使用，而且当连接关闭后队列即被删除。此参考优先级高于durable 
            autoDelete: true,  //自动删除,当没有生成者/消费者使用此队列时，此队列会被自动删除
            arguments: null
        );

        Console.WriteLine("Please input your message,with end:");

        string message = Console.ReadLine() ?? "hello";

        while (message != "EXIT")
        {
            var body = Encoding.UTF8.GetBytes(message);
             
            var properties = new BasicProperties
            {
                Persistent = true,//持久化的
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty, //交换机
                routingKey: "hello", //路由
                mandatory: true, //强制性的
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($" [x] Sent {message}");
            Console.WriteLine("Please input your message,with end:");

            message = Console.ReadLine() ?? "hello";
        }
    }
}
    