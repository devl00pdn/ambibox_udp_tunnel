using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ambilbox_tunnel
{
    public partial class Adalight_tunnel_service : ServiceBase
    {
        private ambibox_tunnel_dev.Adalight_tunnel adatunel = new ambibox_tunnel_dev.Adalight_tunnel();
        public Adalight_tunnel_service()
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
