using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SPAA05.Shared.DataSources;
using SPAA05.Shared.Protocol;

namespace IntergasMQTT {
    public class IntergasReader : IDisposable {
        private Timer _timer;
        private DataSource _dataSource;
        private readonly IntergasProtocol _proto = new IntergasProtocol();

        public event EventHandler<IntergasResponseFrameEventArgs> FrameReceived;

        public IntergasReader()
        {
            _dataSource = new TcpClientDataSource("10.31.45.88", 20108);
            _dataSource.ReconnectBehavior = ReconnectBehavior.ReconnectAlways;
            _proto.Attach(_dataSource);

            _proto.LineReceived += ProtoOnLineReceived;
        }

        private void ProtoOnLineReceived(object sender, LineTransmitEventArgs<IntergasFrame> e)
        {
            if (e.Line is IntergasResponseFrame frame)
            {
                FrameReceived?.Invoke(this, new IntergasResponseFrameEventArgs(frame));
            }
        }

        private void TimerLapsed(object state)
        {
            // query for new data
            _proto.Write(new IntergasQueryFrame());
        }

        public void Dispose()
        {
            _timer.Dispose();
            _dataSource?.Dispose();
            _proto?.Dispose();
        }

        public void Start()
        {
            _dataSource.Start();
            _timer = new Timer(TimerLapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
    }

    public class IntergasResponseFrameEventArgs : EventArgs
    {
        public IntergasResponseFrame Frame { get; private set; }

        public IntergasResponseFrameEventArgs(IntergasResponseFrame frame)
        {
            Frame = frame;
        }
    }

}
