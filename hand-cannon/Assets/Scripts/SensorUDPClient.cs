using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.IO;

/*
public class StateObject {
	public StateObject(UdpClient client) {
		this.c = client;
		IPAddress ipAddress = IPAddress.Parse(ServerConfig.SERVER_IP);
        this.e = new IPEndPoint(ipAddress, ServerConfig.PORT);
	}
	public UdpClient c;
	public IPEndPoint e;
}
*/

public class SensorUDPClient : MonoBehaviour {
	UdpClient client = null;
	bool running = false;
	
	IPacketConverter packetConverter = new PacketConverter();
	SensorValue? lastSensorValue = null;
	
	public void Start() {
		RunClient();
	}
	
	public void StopClient() {
		if(client == null) {
			return;
		}
		client.Close();
		client = null;
		running = false;
		Debug.Log("close client");
	}
	public void RunClient() {
		if(client != null) {
			return;
		}
		Debug.Log("run client");
		client = new UdpClient(ServerConfig.SERVER_IP, ServerConfig.PORT);
		running = true;
		RequestSensorValue();
	}
	
	public void Resume() {
		running = true;
	}
	public void Stop() {
		running = false;
	}
	
	public void Update() {
	}
	
	public void FixedUpdate() {
		if(running == false) {
			return;
		}
		
		if(client == null) {
			//재접속 시도
			client = new UdpClient(ServerConfig.SERVER_IP, ServerConfig.PORT);
		}
		
		if(lastSensorValue == null) {
			RequestSensorValue();
		} else {
			Debug.Log(lastSensorValue.ToString());
			lastSensorValue = null;
		}
	}
	
	private void SendCallback(IAsyncResult ar) {		
		int writeLength = client.EndSend(ar);
		//Debug.Log ("Send Bytes : " + writeLength);
		client.BeginReceive(new AsyncCallback(RecvCallback), null);
	}
	
	private void RecvCallback(IAsyncResult ar) {
		IPAddress ipAddress = IPAddress.Parse(ServerConfig.SERVER_IP);
        IPEndPoint endPoint = new IPEndPoint(ipAddress, ServerConfig.PORT);
		byte[] bytes = client.EndReceive(ar, ref endPoint);
		//Debug.Log ("Recv Bytes : " + bytes.Length);
		
		SensorPacket packet = (SensorPacket)packetConverter.ToPacket(bytes);
		SensorValue sensorVal = new SensorValue();
		sensorVal.sequence = packet.sequence;
		sensorVal.yaw = packet.yaw;
		sensorVal.pitch = packet.pitch;
		sensorVal.roll = packet.roll;
		
		lastSensorValue = sensorVal;
	}
	
	
	private void RequestSensorValue() {
		byte[] bytes = packetConverter.ToByte(new RequestPacket());
		try {
			client.BeginSend(bytes, bytes.Length, new AsyncCallback(SendCallback), null);
		} catch(SocketException e) {
			//Debug.Log(e.ToString());
			client.Close();
			client = null;
		}
	}
	
}