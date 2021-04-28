using System.Linq;
using SPAA05.Shared.DataSources;
using SPAA05.Shared.Protocol;

namespace IntergasMQTT {
    public class IntergasProtocol : DataProtocol<IntergasFrame> {
        protected override void AddToBuffer(DataSource o, DataReceivedEventArgs e) {
            if (e.Data.Length + _buffer.Count >= 32) {
                _buffer.Clear();
            }

            _buffer.AddRange(e.Data);

            if (_buffer.Count == 32) {
                var msg = new IntergasResponseFrame();
                if (msg.Parse(e.Data.ToList()))
                    OnLineReceived(msg);
            }
        }
    }
}