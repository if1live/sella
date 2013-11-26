using System;
using System.IO;

public class PacketGenerator {
	const int PROTOCOL_REQUEST = 0x01;
	
	public byte[] CreateRequestPacket() {
		MemoryStream stream = new MemoryStream();
		using(BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write(PROTOCOL_REQUEST);
		}
		byte[] bytes = stream.ToArray();
		return bytes;
	}
}
