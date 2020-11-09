using System.ServiceProcess;

namespace Jqs
{
    public class CncAcquisitionService : ServiceBase
    {
        public CncAcquisitionService()
        {
            InitializeComponent();
        }

        AcqCnc acq;

        protected override void OnStart(string[] args)
        {
            acq = new AcqCnc();
            acq.Prepare();
            acq.Start();
        }

        protected override void OnStop()
        {
            acq.Close();
        }

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "CNC Acquisition Service";
        }
    }
}