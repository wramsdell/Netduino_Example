using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using VersaShield.Servo;

namespace FPGAPWM
{
	public class Program
	{
		private static VersaShieldServoDriver fpgaDriver;

		public static void Main()
		{
			fpgaDriver = new VersaShieldServoDriver(0x3c);

			for (int i = 0; i < 180; i++)
			{
				fpgaDriver.SetServoDegree(VersaShieldServoDriver.FpgaPwmPorts.Port0, i);
				Thread.Sleep(10);
			}

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
