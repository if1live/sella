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
	
	PacketGenerator packetGenerator = new PacketGenerator();
	PacketParser packetParser = new PacketParser();
	UdpClient client;
	
	SensorValue? lastSensorValue = null;
	bool running = true;
	
	public SensorUDPClient() {
		client = new UdpClient(ServerConfig.SERVER_IP, ServerConfig.PORT);
	}
	
	
	public void Start() {
	}
	
	public void Pause() {
		running = false;
	}
	public void Resume() {
		running = true;
	}
	
	public void Update() {
		if(lastSensorValue == null) {
			if(running) {
				RequestSensorValue();
			}
		} else {
			Debug.Log(lastSensorValue.ToString());
			lastSensorValue = null;
		}
	}
	
	private void SendCallback(IAsyncResult ar) {
		StateObject state = (StateObject) ar.AsyncState;
		UdpClient client = state.c;
		
		int writeLength = client.EndSend(ar);
		Debug.Log ("Send Bytes : " + writeLength);
		client.BeginReceive(new AsyncCallback(RecvCallback), state);
	}
	
	private void RecvCallback(IAsyncResult ar) {
		StateObject state = (StateObject) ar.AsyncState;
		byte[] bytes = state.c.EndReceive(ar, ref state.e);
		Debug.Log ("Recv Bytes : " + bytes.Length);
		
		SensorValue sensorVal = packetParser.HandleSensorPacket(bytes);
		lastSensorValue = sensorVal;
		//Debug.Log(sensorVal.ToString());
	}
	
	private void RequestSensorValue() {
		StateObject state = new StateObject(client);
		byte[] bytes = packetGenerator.CreateRequestPacket();
		client.BeginSend(bytes, bytes.Length, new AsyncCallback(SendCallback), state);
	}
	
}