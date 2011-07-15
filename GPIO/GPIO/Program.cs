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
        public enum PinType {PinInput=(byte)0,PinOutput=(byte)1,PinPwm=(byte)2,PinInvertedPwm=(byte)3};
        public enum PwmParam {PwmRise=(byte)0,PwmFall=(byte)1,PwmPeriod=(byte)2};
        public static void Main()
        {
            SPI.Configuration spiConfig = new SPI.Configuration(
                Pins.GPIO_PIN_D2,
                false,
                100,
                100,
                false,
                true,
                1000,
                SPI.SPI_module.SPI1
            );
            var spi = new SPI(spiConfig);
            bool LEDState=false;
            byte channel;
//            for (channel = 32; channel < 48; channel++)
//            {
//                Thread.Sleep(100);
//                SetPinType(spi, channel,PinType.PinOutput);
//                SetPinState(spi, channel, true);
//                SetPinState(spi, (byte)(channel - 1), false);
//            }
//            SetPinType(spi, 16, PinType.PinPwm);
            Debug.Print(GetVersion(spi).ToString());
            PwmStop(spi);
            uint i;
            for (i = 0; i < 16; i++)
            {
                SetPinType(spi,(byte)(16 + i), PinType.PinPwm);
                SetPwmParameter(spi, (byte)(16 + i), PwmParam.PwmRise, 100000 + (i * 100000));
                SetPwmParameter(spi, (byte)(16 + i), PwmParam.PwmFall, 1700000);
                SetPwmParameter(spi, (byte)(16 + i), PwmParam.PwmPeriod, 1700000);
            }
            SetTerminate(spi,0);
            PwmGo(spi);
        }

        public static void SetRedLED(SPI spi, bool state)
        {
            byte[] WriteBuffer = new byte[2];
            WriteBuffer[0] = 0x03; //Command
            WriteBuffer[1] = state?(byte)0x01:(byte)0x00; //Operand
            spi.Write(WriteBuffer);
        }

        public static void SetGreenLED(SPI spi, bool state)
        {
            byte[] WriteBuffer = new byte[2];
            WriteBuffer[0] = 0x02; //Command
            WriteBuffer[1] = state?(byte)0x01:(byte)0x00; //Operand
            spi.Write(WriteBuffer);
        }

        public static void SetPinType(SPI spi, byte pin, PinType type)
        {
            byte[] WriteBuffer = new byte[2];
            switch (type)
            {
                case PinType.PinInput:
                    WriteBuffer[0] = 0x08;
                    break;
                case PinType.PinOutput:
                    WriteBuffer[0] = 0x09;
                    break;
                case PinType.PinPwm:
                    WriteBuffer[0] = 0x0A;
                    break;
                case PinType.PinInvertedPwm:
                    WriteBuffer[0] = 0x0B;
                    break;
                default:
                    WriteBuffer[0] = 0x55;
                    break;
            }
            WriteBuffer[1] = pin; //Operand
            spi.Write(WriteBuffer);
        }

        public static void PwmGo(SPI spi)
        {
            byte[] WriteBuffer = new byte[2];
            WriteBuffer[0] = 0x05; //Command
            WriteBuffer[1] = 0x01; //Operand
            spi.Write(WriteBuffer);
        }

        public static void PwmStop(SPI spi)
        {
            byte[] WriteBuffer = new byte[2];
            WriteBuffer[0] = 0x05; //Command
            WriteBuffer[1] = 0x00; //Operand
            spi.Write(WriteBuffer);
        }

        public static void SetPinState(SPI spi, byte pin, bool state)
        {
            byte[] WriteBuffer = new byte[6];
            if (pin < 32)
            {
                if (state)
                {
                    WriteBuffer[0] = 0x0D;
                }
                else
                {
                    WriteBuffer[0] = 0x0F;
                }
            }
            else
            {
                if (state)
                {
                    WriteBuffer[0] = 0x0E;
                }
                else
                {
                    WriteBuffer[0] = 0x10;
                }
            }
            WriteBuffer[1] = 0x00; //Operand
            int Data = 0x01 << pin;
            WriteBuffer[2] = (byte)(Data >> 24 & 0xFF);
            WriteBuffer[3] = (byte)(Data >> 16 & 0xFF);
            WriteBuffer[4] = (byte)(Data >> 8 & 0xFF);
            WriteBuffer[5] = (byte)(Data & 0xFF);
            spi.Write(WriteBuffer);
        }

        public static void SetPwmParameter(SPI spi, byte channel, PwmParam param, uint data)
        {
            byte address = (byte)((channel << 2) + param);
            SetMemory(spi, address, data);
        }

        public static uint GetPwmParameter(SPI spi, byte channel, PwmParam param)
        {
            byte address = (byte)((channel << 2) + param);
            return GetMemory(spi, address);
        }

        public static void SetMemory(SPI spi, byte address, uint Data)
        {
            byte[] WriteBuffer = new byte[6];
            WriteBuffer[0] = 0x01; //Operand
            WriteBuffer[1] = address; //Operand
            WriteBuffer[2] = (byte)(Data >> 24 & 0xFF);
            WriteBuffer[3] = (byte)(Data >> 16 & 0xFF);
            WriteBuffer[4] = (byte)(Data >> 8 & 0xFF);
            WriteBuffer[5] = (byte)(Data & 0xFF);
            spi.Write(WriteBuffer);
        }

        public static void SetTerminate(SPI spi, uint Data)
        {
            byte[] WriteBuffer = new byte[6];
            WriteBuffer[0] = 0x06; //Operand
            WriteBuffer[1] = 0x00; //Operand
            WriteBuffer[2] = (byte)(Data >> 24 & 0xFF);
            WriteBuffer[3] = (byte)(Data >> 16 & 0xFF);
            WriteBuffer[4] = (byte)(Data >> 8 & 0xFF);
            WriteBuffer[5] = (byte)(Data & 0xFF);
            spi.Write(WriteBuffer);
        }

        public static uint GetMemory(SPI spi, byte address)
        {
            uint RetVal;
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            TxBuffer[0] = 0x00;
            TxBuffer[1] = address;
            TxBuffer[2] = 0x00;
            TxBuffer[3] = 0x00;
            TxBuffer[4] = 0x00;
            TxBuffer[5] = 0x00;
            spi.WriteRead(TxBuffer, RxBuffer);
            RetVal = (uint)((RxBuffer[2] << 24) + (RxBuffer[3] << 16) + (RxBuffer[4] << 8) + (RxBuffer[5]));
            return RetVal;
        }

        public static uint GetVersion(SPI spi)
        {
            uint RetVal;
            var TxBuffer = new byte[6];
            var RxBuffer = new byte[6];
            TxBuffer[0] = 0xFF;
            TxBuffer[1] = 0x00;
            TxBuffer[2] = 0x00;
            TxBuffer[3] = 0x00;
            TxBuffer[4] = 0x00;
            TxBuffer[5] = 0x00;
            spi.WriteRead(TxBuffer, RxBuffer);
            RetVal = (uint)((RxBuffer[2] << 24) + (RxBuffer[3] << 16) + (RxBuffer[4] << 8) + (RxBuffer[5]));
            return RetVal;
        }

    }
}
