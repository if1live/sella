using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum PacketType {
	NULL = 0x00,
	REQUEST = 0x01,
	SENSOR = 0x02,
}

public interface IPacket {
	PacketType Type();
}

public struct RequestPacket : IPacket {
	public PacketType Type() {
		return PacketType.REQUEST;
	}
}

public struct SensorPacket : IPacket {
	public PacketType Type() {
		return PacketType.SENSOR;
	}
	
	public int sequence;
	public float yaw;
	public float pitch;
	public float roll;
}

public interface IPacketConverter {
	PacketType Type();
	byte[] ToByte(IPacket packet);
	IPacket ToPacket(byte[] bytes);
}

public class RequestPacketConverter : IPacketConverter {
	public PacketType Type() {
		return PacketType.REQUEST;
	}
	public byte[] ToByte(IPacket packet) {
		MemoryStream stream = new MemoryStream();
		using(BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write((int)Type());
		}
		byte[] data = stream.ToArray();
		return data;
		
	}
	public IPacket ToPacket(byte[] bytes) {
		RequestPacket packet = new RequestPacket();
		MemoryStream stream = new MemoryStream(bytes);
		using(BinaryReader reader = new BinaryReader(stream)) {
			reader.ReadInt32();
		}
		return packet;
	}
}

public class SensorPacketConverter : IPacketConverter {
	public PacketType Type() {
		return PacketType.SENSOR;
	}
	public byte[] ToByte(IPacket packet) {
		SensorPacket sensorPacket = (SensorPacket)packet;
		MemoryStream stream = new MemoryStream();
		using(BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write((int)Type());
			writer.Write(sensorPacket.sequence);
			writer.Write(sensorPacket.yaw);
			writer.Write(sensorPacket.pitch);
			writer.Write(sensorPacket.roll);
		}
		byte[] data = stream.ToArray();
		return data;
	}
	public IPacket ToPacket(byte[] bytes) {
		SensorPacket packet = new SensorPacket();
		MemoryStream stream = new MemoryStream(bytes);
		using(BinaryReader reader = new BinaryReader(stream)) {
			reader.ReadInt32();
			packet.sequence = reader.ReadInt32();
			packet.yaw = reader.ReadSingle();
			packet.pitch = reader.ReadSingle();
			packet.roll = reader.ReadSingle();
		}

		return packet;
	}
}

public class PacketConverter : IPacketConverter {
	public PacketType Type() {
		return PacketType.NULL;
	}
	
	private Dictionary<PacketType, IPacketConverter> converterDict = new Dictionary<PacketType, IPacketConverter>();
	
	public PacketConverter() {
		List<IPacketConverter> converterList = new List<IPacketConverter>();
		converterList.Add(new RequestPacketConverter());
		converterList.Add(new SensorPacketConverter());
		
		foreach(IPacketConverter converter in converterList) {
			PacketType type = converter.Type();
			converterDict[type] = converter;
		}
	}
	
	public byte[] ToByte(IPacket packet) {
		IPacketConverter converter = converterDict[packet.Type()];
		return converter.ToByte(packet);
	}
	
	public IPacket ToPacket(byte[] bytes) {
		MemoryStream stream = new MemoryStream(bytes);
		using(BinaryReader reader = new BinaryReader(stream)) {
			PacketType type = (PacketType)reader.ReadInt32();
			IPacketConverter converter = converterDict[type];
			return converter.ToPacket(bytes);
		}
	}
}