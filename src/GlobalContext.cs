#define debug

using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Jqs
{
    public class GlobalContext : BasicLogger
    {
        public static GlobalContext self;

        public JObject g_ctx;

        private GlobalContext()
        {
            string path =
#if debug
				"C:/Users/aiyo/Desktop/AcqCnc/config/acq.yml";
#else
                "config/acq.yml";
#endif
            StreamReader sr = File.OpenText(path);
            logger.Info("Load yaml from: " + path);
            var ds = new DeserializerBuilder().Build();
            var sb = new SerializerBuilder().JsonCompatible().Build();
            string js = "";
            try
            {
                js = sb.Serialize(ds.Deserialize(sr));
            }
            catch (SemanticErrorException see)
            {
                logger.Error("Yaml syntax error! " + see.Message);
                Environment.Exit(1);
            }
            //logger.Info(js);
            g_ctx = JObject.Parse(js);
        }

        public static GlobalContext GetInstance()
        {
            if (self == null)
                self = new GlobalContext();
            return self;
        }

    }
}