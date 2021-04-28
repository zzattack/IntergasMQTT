using System;
using System.Threading;

namespace IntergasMQTT
{
    public class IntergasMqttPublisher : IDisposable
    {
        private readonly DomoticzMqttClient _mqttClient;
        private readonly IntergasReader _intergasReader;

        public IntergasMqttPublisher(DomoticzMqttClient mqttClient, IntergasReader intergasReader)
        {
            _mqttClient = mqttClient;
            _intergasReader = intergasReader;
            intergasReader.FrameReceived += OnFrameReceived;
        }

        private void OnFrameReceived(object sender, IntergasResponseFrameEventArgs e)
        {
            // transform frame into domiticz-understood JSON
            int x = 4;
            // _mqttClient.PostFrame(data);
        }

        public void Start()
        {
        }

        public void Dispose()
        {
        }
    }
}