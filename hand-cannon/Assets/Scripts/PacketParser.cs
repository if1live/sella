using System;
using System.IO;

public class PacketParser {
	const int PROTOCOL_SENSOR = 0x02;
	
	public SensorValue HandleSensorPacket(byte[] bytes) {
		SensorValue sensorVal = new SensorValue();
		MemoryStream stream = new MemoryStream(bytes);
		using(BinaryReader reader = new BinaryReader(stream)) {
			int protocolId = reader.ReadInt32();
			int sequence = reader.ReadInt32();
			float yaw = reader.ReadSingle();
			float pitch = reader.ReadSingle();
			float roll = reader.ReadSingle();
			
			sensorVal.sequence = sequence;
			sensorVal.yaw = yaw;
			sensorVal.pitch = pitch;
			sensorVal.roll = roll;
		}
		return sensorVal;
	}
}