using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
//using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoApplication1
{
    public class Program
    {
        public static void Main()
        {
            int I2CClockRateKhz = 100;
            ushort I2CAddress = 0x3c;
            I2CDevice FPGA = new I2CDevice(new I2CDevice.Configuration(I2CAddress, I2CClockRateKhz));
            byte RecByte = GetData(FPGA, 0);
            Debug.Print(RecByte.ToString());
            RecByte = GetData(FPGA, 1);
            Debug.Print(RecByte.ToString());
        }

        public static void SetRedLED(I2CDevice FPGA, bool state)
        {
            int I2CTimeout = 1000;
            byte[] buffer = new byte[2];
            buffer[0] = 0x10;
            buffer[1] = state ? (byte)0x01 : (byte)0x00;
            var transaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(buffer)
            };
            FPGA.Execute(transaction, I2CTimeout);
        }

        public static byte GetData(I2CDevice FPGA, byte Port)
        {
            int I2CTimeout = 1000;
            byte[] buffer = new byte[1];
            var transaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(new byte[] {Port}),
                I2CDevice.CreateReadTransaction(buffer)
            };
            FPGA.Execute(transaction, I2CTimeout);
            return buffer[0];
        }

    }
}
