using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.IO;

// State object for reading client data asynchronously
public class StateObject {
	public StateObject(UdpClient client) {
		this.c = client;
		IPAddress ipAddress = IPAddress.Parse(ServerConfig.SERVER_IP);
        this.e = new IPEndPoint(ipAddress, ServerConfig.PORT);
	}
	public UdpClient c;
	public IPEndPoint e;
}

public class SensorUDPClient : MonoBehaviour {
	
	const int PROTOCOL_REQUEST = 0x01;
	const int PROTOCOL_SENSOR = 0x02;
	
	byte[] CreateRequestPacket() {
		MemoryStream stream = new MemoryStream();
		using(BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write(PROTOCOL_REQUEST);
		}
		byte[] bytes = stream.ToArray();
		return bytes;
	}
	
	SensorValue HandleSensorPacket(byte[] bytes) {
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
	
	public void Start() {
		UdpClient client = new UdpClient(ServerConfig.SERVER_IP, ServerConfig.PORT);
		
		try {
			StateObject state = new StateObject(client);
			byte[] bytes = CreateRequestPacket();
			client.BeginSend(bytes, bytes.Length, new AsyncCallback(SendCallback), state);
		} catch(Exception e) {
			Debug.Log(e.ToString());
		}		
	}
	
	private void SendCallback(IAsyncResult ar) {
		StateObject state = (StateObject) ar.AsyncState;
		UdpClient client = state.c;
		
		int writeLength = client.EndSend(ar);
		//Debug.Log ("Send Bytes : " + writeLength);
		client.BeginReceive(new AsyncCallback(RecvCallback), state);
	}
	
	private void RecvCallback(IAsyncResult ar) {
		StateObject state = (StateObject) ar.AsyncState;
		byte[] bytes = state.c.EndReceive(ar, ref state.e);
		
		Debug.Log ("Recv Bytes : " + bytes.Length);
		
		SensorValue sensorVal = HandleSensorPacket(bytes);
		Debug.Log(sensorVal.ToString());
		
		state.c.Close();
	}
	
	public void Update() {
	}
}