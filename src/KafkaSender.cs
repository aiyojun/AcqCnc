using System;
using Newtonsoft.Json;
using Confluent.Kafka;
using YamlDotNet;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Jqs
{
    public class KafkaSender : BasicLogger
    {
        private IProducer<string, string> producer;

        public KafkaSender(string url)
        {
            logger.Info("Create KafkaSender - " + url);
            ProducerConfig config = new ProducerConfig();
            config.BootstrapServers = url;
            var builder = new ProducerBuilder<string, string>(config);
            builder.SetKeySerializer(Serializers.Utf8);
            producer = builder.Build();
        }

        public void Send(string topic, string message)
        {
            producer.Produce(topic, new Message<string, string>() { Key = "", Value = message }); ;
        }

        //public static void Main(string[] args)
        //{
        //    logger.Info("Kafka sender loading ...");
        //    var yaml = GlobalContext.GetInstance().g_ctx;
        //    logger.Info("Yaml:\n" + yaml);
        //    KafkaSender sender = new KafkaSender(yaml["kafka"]["ip"] + ":" + yaml["kafka"]["port"]);
        //}
    }
}
