using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;

public class SensorUDPServer : MonoBehaviour
{
    IPacketConverter packetConverter = new PacketConverter();
    Socket serverSocket = null;
 
    // Data stream
    private byte[] recvBuffer = new byte[1024];
 
    public void OnDestroy()
    {
        StopServer();
    }
 
    public void StopServer()
    {
        if (serverSocket == null)
        {
            return;
        }
     
        serverSocket.Close(1);
        serverSocket = null;
    }
 
    public void RunServer()
    {
        if (serverSocket != null)
        {
            return;
        }
     
        // Initialise the socket
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
     
        // Initialise the IPEndPoint for the server and listen on port 
        IPEndPoint server = new IPEndPoint(IPAddress.Any, ServerConfig.PORT);
     
        // Associate the socket with this IP address and port
        serverSocket.Bind(server);
     
        // Initialise the IPEndPoint for the clients
        IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
        EndPoint epSender = (EndPoint)clients;
     
        // Start listening for incoming data
        // 프로토콜 타입으로 패킷 분기하니까 clear안해도 별 문제는 없겠지만
        // 혹시나 디버깅 하는 사태를 대비해서 clear를 넣어줬다
        Array.Clear(recvBuffer, 0, recvBuffer.Length);
        serverSocket.BeginReceiveFrom(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(RecvCallback), epSender);
    }
 
    // Use this for initialization
    void Start()
    {
        RunServer();
    }
 
    // Update is called once per frame
    void Update()
    {
 
    }
 
    private void SendCallback(IAsyncResult ar)
    {
        int writeLength = serverSocket.EndSendTo(ar);
        //Debug.Log ("Send Bytes : " + writeLength);
     
        IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
        EndPoint epSender = (EndPoint)clients;
        Array.Clear(recvBuffer, 0, recvBuffer.Length);
        serverSocket.BeginReceiveFrom(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(RecvCallback), epSender);
    }
 
    private void RecvCallback(IAsyncResult ar)
    {
        // Initialise the IPEndPoint for the clients
        IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
     
        // Initialise the EndPoint for the clients
        EndPoint epSender = (EndPoint)clients;
     
        // Receive all data
        int recvLength = serverSocket.EndReceiveFrom(ar, ref epSender);
     
        //Debug.Log ("Recv Bytes : " + recvLength);
        RequestPacket packet = (RequestPacket)packetConverter.ToPacket(recvBuffer);
     
        SensorPacket sensorPacket = new SensorPacket();
        sensorPacket.sequence = 1;
        sensorPacket.yaw = 0.1f;
        sensorPacket.pitch = 0.2f;
        sensorPacket.roll = 0.3f;
        byte[] data = packetConverter.ToByte(sensorPacket);
        serverSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, epSender, new AsyncCallback(SendCallback), null);
    }
}
