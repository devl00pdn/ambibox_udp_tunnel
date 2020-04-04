using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ambilbox_tunel
{
    public partial class Adalight_tunel_service : ServiceBase
    {
        private ambibox_tunel_dev.Adalight_tunel adatunel = new ambibox_tunel_dev.Adalight_tunel();
        public Adalight_tunel_service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            adatunel.start();
        }

        protected override void OnStop()
        {
            adatunel.stop();
        }
    }
}
