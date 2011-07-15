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
            SPI.Configuration spiConfig = new SPI.Configuration(
                Pins.GPIO_PIN_D0,
                false,
                100,
                100,
                false,
                true,
                1000,
                SPI.SPI_module.SPI1
            );
            var spi = new SPI(spiConfig);
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            PWMStop(spi);
            int i = 0;
            for (i = 0; i < 32; i++)
            {
                SetPWM(spi, (byte)i, i, i + 1, 80);
            }
//            SetPWM(spi, 0, 1, 9, 13);
            SetTerminate(spi, 0);
            PWMStart(spi);
            GreenLEDOn(spi);
        }

        public static void WriteReg(SPI spi, byte Addr, int Data)
        {
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            TxBuffer[0] = 0x01;
            TxBuffer[1] = Addr;
            TxBuffer[2] = (byte)(Data >> 24 & 0xFF);
            TxBuffer[3] = (byte)(Data >> 16 & 0xFF);
            TxBuffer[4] = (byte)(Data >> 8 & 0xFF);
            TxBuffer[5] = (byte)(Data & 0xFF);
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static int ReadReg(SPI spi, byte Addr)
        {
            int RetVal;
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            TxBuffer[0] = 0x00;
            TxBuffer[1] = Addr;
            TxBuffer[2] = 0x00;
            TxBuffer[3] = 0x00;
            TxBuffer[4] = 0x00;
            TxBuffer[5] = 0x00;
            spi.WriteRead(TxBuffer, RxBuffer);
            RetVal = (int)((RxBuffer[2] << 24) + (RxBuffer[3] << 16) + (RxBuffer[4] << 8) + (RxBuffer[5]));
            return RetVal;
        }

        public static void SetPWM(SPI spi, byte channel, int risetime, int falltime, int period)
        {
            WriteReg(spi, (byte)(channel<<2), period);
            WriteReg(spi, (byte)((channel<<2)+1), risetime);
            WriteReg(spi, (byte)((channel<<2)+2), falltime);
        }

        public static void SetTerminate(SPI spi, int Data)
        {
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            TxBuffer[0] = 0x05;
            TxBuffer[1] = 0x00;
            TxBuffer[2] = (byte)(Data >> 24 & 0xFF);
            TxBuffer[3] = (byte)(Data >> 16 & 0xFF);
            TxBuffer[4] = (byte)(Data >> 8 & 0xFF);
            TxBuffer[5] = (byte)(Data & 0xFF);
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void PWMStop(SPI spi)
        {
            byte[] TxBuffer = { 0x02, 0x00 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void PWMStart(SPI spi)
        {
            byte[] TxBuffer = { 0x02, 0x01 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void GreenLEDOff(SPI spi)
        {
            byte[] TxBuffer = { 0x03, 0x00 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void GreenLEDOn(SPI spi)
        {
            byte[] TxBuffer = { 0x03, 0x01 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void RedLEDOff(SPI spi)
        {
            byte[] TxBuffer = { 0x04, 0x00 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }

        public static void RedLEDOn(SPI spi)
        {
            byte[] TxBuffer = { 0x04, 0x01 };
            byte[] RxBuffer = new byte[2];
            spi.WriteRead(TxBuffer, RxBuffer);
        }
    }
}