using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntergasMQTT {
    public partial class MainForm : Form {
        private readonly IntergasReader _intergasReader = new IntergasReader();
        private readonly DomoticzMqttClient _mqttClient = new DomoticzMqttClient();
        private IntergasMqttPublisher _pusher;

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            _intergasReader.Start();
            _mqttClient.Start();

            _intergasReader.FrameReceived += OnFrameReceived;

            _pusher = new IntergasMqttPublisher(_mqttClient, _intergasReader);
            _pusher.Start();
        }

        private void OnFrameReceived(object sender, IntergasResponseFrameEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action) delegate { OnFrameReceived(sender, e); });
                return;
            }

            tbT1.Text = $"{e.Frame.T1:F2} °C";
            tbT2.Text = $"{e.Frame.T2:F2} °C";
            tbT3.Text = $"{e.Frame.T3:F2} °C";
            tbT4.Text = $"{e.Frame.T4:F2} °C";
            tbT5.Text = $"{e.Frame.T5:F2} °C";
            tbT6.Text = $"{e.Frame.T6:F2} °C";

            tbFanSetpoint.Text = $"{e.Frame.FanspeedSetpoint:F0} rpm";
            tbFanSpeed.Text = $"{e.Frame.Fanspeed:F0} rpm";
            tbFanPWM.Text = $"{e.Frame.FanPWM:F0}%";
            tbWaterPressure.Text = $"{e.Frame.WaterPressure:F2} bar";

            ckbGpSwitch.Checked = e.Frame.GpSwitch;
            ckbTapSwitch.Checked = e.Frame.TapSwitch;
            ckbRoomThermostat.Checked = e.Frame.RoomThermostat;
            ckbPumpRunning.Checked = e.Frame.PumpRunning;
            ckbThreeWayValve.Checked = e.Frame.ThreeWayValve;
            ckbAlarmStatus.Checked = e.Frame.AlarmStatus;
            ckbCascadeRelay.Checked = e.Frame.CascadeRelay;
            ckbOpenThermUsed.Checked = e.Frame.OpenThermUsed;

            ckbGasValve.Checked = e.Frame.GasValve;
            ckbSpark.Checked = e.Frame.Spark;
            ckbIonisation.Checked = e.Frame.IonisationSignal;
            ckbOpenThermDisabled.Checked = e.Frame.OpenThermDisabled;
            ckbLowWaterPressure.Checked = e.Frame.LowWaterPressure;
            ckbWaterPressureSensor.Checked = e.Frame.WaterPressureSensorPresent;
            ckbBurnerBlocked.Checked = e.Frame.BurnerBlock;
            ckbGradientFlag.Checked = e.Frame.GradientFlag;

            lblDisplayCode.Text = e.Frame.Display.ToString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _intergasReader.Dispose();
            _mqttClient.Dispose();
            _pusher.Dispose();
        }
    }

}
