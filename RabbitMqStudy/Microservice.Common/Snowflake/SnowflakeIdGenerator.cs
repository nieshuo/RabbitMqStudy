namespace Microservice.Common.Snowflake
{
    public class SnowflakeIdGenerator
    {
        private static readonly long twepoch = 1577836800000L; // 起始时间
        private static long sequence = 0L; // 初始序列号
        private static readonly long sequenceMask = -1L ^ (1L << 12); // 最大机器ID
        private static long lastTimestamp = -1L;

        private static readonly object syncRoot = new object();
        private static long workerId; // 机器ID
        private static long datacenterId; // 数据中心ID
        private static long workerIdBits = 5L;
        private static long datacenterIdBits = 5L;
        private static long maxWorkerId = -1L ^ (-1L << (int)workerIdBits);// 最大机器ID
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);// 最大数据中心ID
        private static long workerIdShift = sequenceBits + datacenterIdBits; //机器码数据左移位数，就是数据中心和计数器总字节数
        private static long datacenterIdShift = sequenceBits; //数据中心数据左移位数，就是后面计数器占用的位数
        private static long timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits; //时间戳左移动位数就是机器码丶数据中心和计数器总字节数
        private static long sequenceBits = 12L; //计数器字节数，12个字节用来保存计数码

        public SnowflakeIdGenerator(long workerId, long datacenterId)
        {
            if (workerId > maxWorkerId || workerId < 0)
                throw new ArgumentException("workerId can't be greater than " + maxWorkerId + " or less than 0");
            if (datacenterId > maxDatacenterId || datacenterId < 0)
                throw new ArgumentException("datacenterId can't be greater than " + maxDatacenterId + " or less than 0");

            SnowflakeIdGenerator.workerId = workerId;
            SnowflakeIdGenerator.datacenterId = datacenterId;
        }

        public long NextId()
        {
            lock (syncRoot)
            {
                long timestamp = TimeGen();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception($"Clock moved backwards, refusing to generate id for {lastTimestamp - timestamp} milliseconds");
                }

                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0;
                }

                lastTimestamp = timestamp;

                return ((timestamp - twepoch) << (int)timestampLeftShift) |
                       (datacenterId << (int)datacenterIdShift) |
                       (workerId << (int)workerIdShift) | sequence;
            }
        }

        protected virtual long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }
    }
}
