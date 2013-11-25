using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System;

public class LegacySensorTCPClicntTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void LegacyConnectTest() {
		TcpClient client = new TcpClient();
		client.Connect(SensorTCPClient.SERVER_IP, SensorTCPClient.PORT);
		StreamWriter sw = new StreamWriter(client.GetStream());
		StreamReader sr = new StreamReader(client.GetStream());
		
		sw.WriteLine(SensorTCPClient.CMD_REQUEST);
		sw.Flush();
		
		string line1 = sr.ReadLine();
		Debug.Log (line1);
		
		sw.WriteLine(SensorTCPClient.CMD_REQUEST);
		sw.Flush();
		
		string line2 = sr.ReadLine();
		Debug.Log (line2);
		
		sw.WriteLine(SensorTCPClient.CMD_REQUEST);
		sw.Flush();
		
		string line3 = sr.ReadLine();
		Debug.Log (line3);
		
		sr.Close();
		sw.Close();
	}
}
