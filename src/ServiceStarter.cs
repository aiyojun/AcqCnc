using System.ServiceProcess;

namespace Jqs
{
    public class ServiceStarter
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CncAcquisitionService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}