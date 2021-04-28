using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace IntergasMQTT
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if fdhghfgfhgd
            // Setup MQTT client and protocol reader
            using (var intergasReader = new IntergasReader())
            using (var mqttClient = new DomoticzMqttClient())
            using (var pusher = new IntergasMqttPublisher(mqttClient, intergasReader))
            {
                intergasReader.Start();
                mqttClient.Start();
                pusher.Start();

                // Wait until kill signal
                AutoResetEvent waitEvent = new AutoResetEvent(false);
                Console.CancelKeyPress += delegate { waitEvent.Set(); };
                waitEvent.WaitOne();
#else
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
#endif
        }
    }
}