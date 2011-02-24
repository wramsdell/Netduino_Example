using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace ChrisSeto.Fpga
{
	class FpgaPpmReaderDriver
	{
		/// <summary>
		/// FPGA i2c
		/// </summary>
		private I2CDevice fpga;

		/// <summary>
		/// Available channels to read
		/// </summary>
		public enum Channels
		{
			Channel0,
			Channel1,
			Channel2,
			Channel3,
			Channel4,
			Channel5,
		};

		/// <summary>
		/// Construct, create the 
		/// </summary>
		/// <param name="address"></param>
		public FpgaPpmReaderDriver(byte address)
		{
			I2CDevice.Configuration config = new I2CDevice.Configuration(address, 100);

			fpga = new I2CDevice(config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		public int GetChannel(Channels channel)
		{
			//Odd high
			//Even low

			byte[] msb = new byte[] 
			{
				(byte)((int)channel << 1) 
			};

			byte[] lsb = new byte[]
			{
				(byte)(((int)channel << 1) + 1)
			};

			byte[] msbOut = new byte[1];
			byte[] lsbOut = new byte[1];

			I2CDevice.I2CTransaction[] transaction = new I2CDevice.I2CTransaction[]
			{
				I2CDevice.CreateWriteTransaction(msb),
				I2CDevice.CreateReadTransaction(msbOut),
				I2CDevice.CreateWriteTransaction(lsb),
				I2CDevice.CreateReadTransaction(lsbOut),
			};

			int output = ((((int) msbOut[0]) << 8) | (lsbOut[0]));

			return output;
		}
	}
}
