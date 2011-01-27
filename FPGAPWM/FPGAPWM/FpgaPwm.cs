/*
 * VersaShield Servo driver
 *	Coded by Chris Seto 2011
 *	
 * This source code is released under the Apache 2.0 license, copyright Chris Seto
 * */

using Microsoft.SPOT.Hardware;

namespace VersaShield.Servo
{
	class VersaShieldServoDriver
	{
		/// <summary>
		/// FPGA I2C target
		/// </summary>
		private I2CDevice fpga;

		/// <summary>
		/// Available Ports
		/// </summary>
		public enum FpgaPwmPorts
		{
			Port0,
			Port1,
			Port2,
			Port3,
			Port4,
			Port5,
			Port6,
			Port7,
		};

		/// <summary>
		/// Construct, create the Device config
		/// </summary>
		public VersaShieldServoDriver(byte address)
		{
			fpga = new I2CDevice(new I2CDevice.Configuration(address, 100));
		}

		/// <summary>
		/// Set a target servo to a degree
		/// </summary>
		/// <param name="port"></param>
		/// <param name="degrees"></param>
		public void SetServoDegree(FpgaPwmPorts port, double degrees)
		{
			// Calculate how much time to hold the pin high
			int time = (int)(degrees * 1000 / 180 + 1000);

			// Generate MSB/LSB
			byte[] bufferMsb = new byte[2] { (byte)port, (byte)(time & 0xff) };
			byte[] bufferLsb = new byte[2] { (byte)(port + 1), (byte)((time >> 8) & 0xff) };

			// Create the transaction
			I2CDevice.I2CTransaction[] transaction = new I2CDevice.I2CTransaction[]
			{
				I2CDevice.CreateWriteTransaction(bufferLsb),
				I2CDevice.CreateWriteTransaction(bufferMsb)
			};

			// Execute!
			fpga.Execute(transaction, 1000);
		}
	}
}
