using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;

public class SensorUDPServer : MonoBehaviour {
	IPacketConverter packetConverter = new PacketConverter();
	Socket socket;
	// Use this for initialization
	void Start () {
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		socket.ReceiveTimeout = 5000;
		IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
		IPEndPoint endPoint = new IPEndPoint(ipAddress, ServerConfig.PORT);
		socket.Bind(endPoint);
	
		byte[] recvData = new byte[1024];
		EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
		int recvByte = socket.ReceiveFrom(recvData, ref sender);
		Debug.Log ("Recv Bytes : " + recvByte);
		
		SensorPacket sensorPacket = new SensorPacket();
		sensorPacket.sequence = 1;
		sensorPacket.yaw = 0.1f;
		sensorPacket.pitch = 0.2f;
		sensorPacket.roll = 0.3f;
		byte[] data = packetConverter.ToByte(sensorPacket);
		socket.SendTo(data, sender);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	private void SendCallback(IAsyncResult ar) {
		int writeLength = socket.EndSend(ar);
		Debug.Log ("Send Bytes : " + writeLength);
		socket.BeginReceive(new AsyncCallback(RecvCallback), null);
	}
	
	private void RecvCallback(IAsyncResult ar) {
		IPAddress ipAddress = IPAddress.Parse(ServerConfig.SERVER_IP);
        IPEndPoint endPoint= new IPEndPoint(ipAddress, ServerConfig.PORT);
		byte[] bytes = socket.EndReceive(
		Debug.Log ("Recv Bytes : " + bytes.Length);
		
		RequestPacket packet = (RequestPacket)packetConverter.ToPacket(bytes);
		
		SensorPacket sensorPacket = new SensorPacket();
		sensorPacket.sequence = 1;
		sensorPacket.yaw = 0.1f;
		sensorPacket.pitch = 0.2f;
		sensorPacket.roll = 0.3f;
		byte[] data = packetConverter.ToByte(sensorPacket);
		socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), null);
	}
	*/	
}
