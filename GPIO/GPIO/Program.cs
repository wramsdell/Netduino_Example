using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoApplication1
{
    public class Program
    {
        public static void Main()
        {
            int I2CClockRateKhz = 100;
            bool LEDState = false;
            ushort I2CAddress = 0x3c;
            I2CDevice FPGA = new I2CDevice(new I2CDevice.Configuration(I2CAddress, I2CClockRateKhz));
            for (byte i = 0; i <= 7; i++)
            {
                SetOE(FPGA, i, 0xFF);
            }
            SetData(FPGA, 1, 0);
            byte data = 1;
            byte port = 0;
            while (true)
            {
                LEDState = !LEDState;
                SetRedLED(FPGA, LEDState);
                Thread.Sleep(200);
                if (data == 0)
                {
                    SetData(FPGA, port, data);
                    SetData(FPGA, (byte)(port + 2), data);
                    SetData(FPGA, (byte)(port + 4), data);
                    SetData(FPGA, (byte)(port + 6), data);
                    if (port == 0)
                    {
                        port = 1;
                        data = 128;
                    }
                    else
                    {
                        port = 0;
                        data = 1;
                    }
                }
                SetData(FPGA, port, data);
                SetData(FPGA, (byte)(port + 2), data);
                SetData(FPGA, (byte)(port + 4), data);
                SetData(FPGA, (byte)(port + 6), data);
                if (port == 0)
                    data <<= 1;
                else
                    data >>= 1;
            }
        }
        public static void SetOE(I2CDevice FPGA, byte Port, byte OE)
        {
            int I2CTimeout = 1000;
            byte PortAddr = (byte)(Port + (8));
            byte[] buffer = new byte[2];
            buffer[0] = PortAddr;
            buffer[1] = OE;
            var transaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(buffer)
            };
            FPGA.Execute(transaction, I2CTimeout);
        }
        public static void SetData(I2CDevice FPGA, byte Port, byte Data)
        {
            int I2CTimeout = 1000;
            byte PortAddr = Port;
            byte[] buffer = new byte[2];
            buffer[0] = PortAddr;
            buffer[1] = Data;
            var transaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(buffer)
            };
            FPGA.Execute(transaction, I2CTimeout);
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
