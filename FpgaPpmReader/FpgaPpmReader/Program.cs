using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using ChrisSeto.Fpga;

namespace FpgaPpmReader
{
	public class Program
	{
		private static FpgaPpmReaderDriver fpga;

		public static void Main()
		{
			fpga = new FpgaPpmReaderDriver(0x3c);

			while (true)
			{
				Debug.Print(fpga.GetChannel(FpgaPpmReaderDriver.Channels.Channel0).ToString());

				Thread.Sleep(100);
			}
		}

	}
}
