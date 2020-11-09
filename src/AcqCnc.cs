using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Jqs
{
    class SelfParam
    {
        public AcqCnc Self { get; set; }
        public string Id { get; set; }
    }

    public class AcqCnc : BasicLogger
    {

        private JObject yml = GlobalContext.GetInstance().g_ctx;

        private Dictionary<string, Tuple<CncReader, KafkaSender>> targetHandles = new Dictionary<string, Tuple<CncReader, KafkaSender>>();

        private Dictionary<string, Thread> targetWorkers = new Dictionary<string, Thread>();

        private static bool isWorking = false;

        public void Prepare()
        {
            //logger.Info("type of interval:" + yml["acq"]["interval"].GetType());
            logger.Info("Yaml:\n" + yml);
            //Dictionary<string, Tuple<string, int>> targets = new Dictionary<string, Tuple<string, int>>();
            foreach (var ele in yml["cnc"])
            {
                logger.Info(ele);
                string ip = ele["ip"].ToString();
                int port = (int) ele["port"];
                string url = ip + ":" + port;
                //targets[url] = new Tuple<string, int>(ip, port);
                if (targetHandles.ContainsKey(url))
                {
                    continue;
                }
                try
                {
                    var cr = new CncReader(ip, port);
                    var ks = new KafkaSender(yml["kafka"]["ip"].ToString() + ":" + yml["kafka"]["port"].ToString());
                    targetHandles[url] = new Tuple<CncReader, KafkaSender>(cr, ks);
                    targetWorkers[url] = new Thread(new ParameterizedThreadStart(Worker));
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
            logger.Info("Prepare over!");
        }

        private static void Worker(Object obj)
        {
            SelfParam sp = (SelfParam) obj;
            var self = sp.Self;
            var id = sp.Id;
            CncReader reader = self.targetHandles[id].Item1;
            KafkaSender sender = self.targetHandles[id].Item2;
            while (isWorking)
            {
                //logger.Info("Task#" + id + " started!");
                string resp = reader.read();
                sender.Send(self.yml["kafka"]["topic"].ToString(), resp);
                //logger.Info("Gen msg - " + resp);
                Thread.Sleep((int) self.yml["acq"]["interval"]);
            }
        }

        public void Close()
        {
            isWorking = false;
        }

        public void Start()
        {
            logger.Info("AcqCnc started!");
            isWorking = true;
            foreach (var id in targetHandles.Keys)
            {
                var param = new SelfParam
                {
                    Self = this,
                    Id = id
                };
                targetWorkers[id].Start(param);
            }
            //Thread worker = new Thread(new ParameterizedThreadStart(Worker));
            //var param = new SelfParam
            //{
            //    Self = this,
            //    Id = "10.8.8.231:8193"
            //};
            //worker.Start(param);
            //Worker(this, "10.8.8.231:8193");
        }

        //public static void Main(string[] args)
        //{
        //    logger.Info("AcqCnc starting ...");
        //    AcqCnc acq = new AcqCnc();
        //    acq.Prepare();
        //    acq.Start();
        //}
    }
}