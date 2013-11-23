using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System;

//http://drago7.tistory.com/43
public class SensorTCPClient : MonoBehaviour {
	
	private Socket sock = null;
	string serverIp = "192.168.0.9";
	int port = 23223;
	
	private byte[] recvBuffer = new byte[1024];
	private byte[] sendBuffer = new byte[4];

	// Use this for initialization
	void Start () {
		TcpClient client = new TcpClient();
		client.Connect(serverIp, port);
		StreamWriter sw = new StreamWriter(client.GetStream());
		StreamReader sr = new StreamReader(client.GetStream());
		
		sw.WriteLine("req");
		sw.Flush();
		
		string line1 = sr.ReadLine();
		Debug.Log (line1);
		
		sw.WriteLine("req");
		sw.Flush();
		
		string line2 = sr.ReadLine();
		Debug.Log (line2);
		
		sw.WriteLine("quit");
		sw.Flush();
		
		string line3 = sr.ReadLine();
		Debug.Log (line3);
		
		sr.Close();
		sw.Close();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void Shutdown() {
		if( sock != null ) {
			sock.Shutdown( SocketShutdown.Both );
			sock = null;
		}
	}
	
	private void ReceiveComplete( IAsyncResult ar ) {
		try {
			if( null == sock ) {
				return;
			}
			int len = sock.EndReceive( ar );
			if( len == 0 ) {
				Shutdown();
			} else {
				Debug.Log(String.Format("{0} received", recvBuffer[0] ) );
				sock.BeginReceive(
					recvBuffer,
					0,
					recvBuffer.Length,
					SocketFlags.None,
					new AsyncCallback( ReceiveComplete ),
					null );
			}                     
		} catch( Exception e ) {
			Debug.Log( "Exception: " + e.Message );
			Shutdown();
		}
	}
	
	private void SendComplete( IAsyncResult ar ) {
		try {
			if( null == sock ) {
				return;
			}
			int len = sock.EndSend( ar );
			if( len == 1 ) {
				Debug.Log( "Send success" );
			}
		} catch( Exception e ) {
			Debug.Log( "Exception: " + e.Message );
			Shutdown();
		}
	}
}
