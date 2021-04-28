using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SPAA05.Shared.Protocol;

namespace IntergasMQTT {
    public abstract class IntergasFrame : DataLine {
        public enum FrameType {
            Query,
            Response
        }
    }

    public class IntergasQueryFrame : IntergasFrame {
        public override List<byte> Serialize(bool escape = true) {
            return Encoding.Default.GetBytes("S?\r").ToList();
        }

        public override bool Parse(List<byte> data) {
            return false;
        }
    }

    public class IntergasResponseFrame : IntergasFrame {
        public double T1 { get; set; } // Rookgassensor
        public double T2 { get; set; } // Aanvoersensor S1
        public double T3 { get; set; } // Retoursensor S2
        public double T4 { get; set; } // Warmwatersensor S3
        public double T5 { get; set; } // Boilersensor S4
        public double T6 { get; set; } // Buitenvoeler (?)
        public double WaterPressure { get; set; }
        public double TemperatureSetpoint { get; set; }
        public double FanspeedSetpoint { get; set; }
        public double Fanspeed { get; set; }
        public double FanPWM { get; set; }
        public double IOcurrent { get; set; }
        public bool GpSwitch { get; set; }
        public bool TapSwitch { get; set; }
        public bool RoomThermostat { get; set; }
        public bool PumpRunning { get; set; }
        public bool ThreeWayValve { get; set; }
        public bool AlarmStatus { get; set; }
        public bool CascadeRelay { get; set; }
        public bool OpenThermUsed { get; set; }

        public bool GasValve { get; set; }
        public bool Spark { get; set; }
        public bool IonisationSignal { get; set; }
        public bool OpenThermDisabled { get; set; }
        public bool LowWaterPressure { get; set; }
        public bool WaterPressureSensorPresent { get; set; }
        public bool BurnerBlock { get; set; }
        public bool GradientFlag { get; set; }

        public enum DisplayCode : byte {
            Unknown,
            Code0,
            Code1,
            Code2,
            Code3,
            Code4,
            Code5,
            Code6,
            Code7
        }

        public DisplayCode Display { get; set; }

        public override List<byte> Serialize(bool escape = true) {
            throw new System.NotImplementedException();
        }

        double getFloat(byte msb, byte lsb) {
            if (msb > 127)
                return -(((msb ^ 255) + 1) * 256.0 - lsb);
            else
                return msb * 256.0 + lsb;
        }

        public override bool Parse(List<byte> data)
        {
            if (data.Count != 32)
                return false;


            T1 = getFloat(data[1], data[0]) / 100.0;
            T2 = getFloat(data[3], data[2]) / 100.0;
            T3 = getFloat(data[5], data[4]) / 100.0;
            T4 = getFloat(data[7], data[6]) / 100.0;
            T5 = getFloat(data[9], data[8]) / 100.0;
            T6 = getFloat(data[11], data[10]) / 100.0;
            WaterPressure = getFloat(data[13], data[12]) / 100.0;
            TemperatureSetpoint = getFloat(data[15], data[14]) / 100.0;
            FanspeedSetpoint = getFloat(data[17], data[16]);
            Fanspeed = getFloat(data[19], data[18]);
            FanPWM = getFloat(data[21], data[20]) / 10.0;
            IOcurrent = getFloat(data[23], data[22]);

            switch (data[24]) {
                case 102: Display = DisplayCode.Code1; break;
                case 0: Display = DisplayCode.Code1; break;
                case 126: Display = DisplayCode.Code0; break;
                case 204: Display = DisplayCode.Code0; break;
                case 231: Display = DisplayCode.Code5; break;
                case 240: Display = DisplayCode.Code7; break;
                case 32: Display = DisplayCode.Code7; break; // ?
                default: Debugger.Break(); break;
            }

            // byte 26
            GpSwitch = (data[26] & 0x01) != 0;
            TapSwitch = (data[26] & 0x02) != 0;
            RoomThermostat = (data[26] & 0x04) != 0;
            PumpRunning = (data[26] & 0x08) != 0;
            ThreeWayValve = (data[26] & 0x10) != 0;
            AlarmStatus = (data[26] & 0x20) != 0;
            CascadeRelay = (data[26] & 0x40) != 0;
            OpenThermUsed = (data[26] & 0x80) != 0;

            // byte 28
            GasValve = (data[28] & 0x01) != 0;
            Spark = (data[28] & 0x02) != 0;
            IonisationSignal = (data[28] & 0x04) != 0;
            OpenThermDisabled = (data[28] & 0x08) != 0;
            LowWaterPressure = (data[28] & 0x10) != 0;
            WaterPressureSensorPresent = (data[28] & 0x20) != 0;
            BurnerBlock = (data[28] & 0x40) != 0;
            GradientFlag = (data[28] & 0x80) != 0;

            return true;
        }
    }
}