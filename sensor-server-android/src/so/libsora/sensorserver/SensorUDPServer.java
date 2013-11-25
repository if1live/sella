package so.libsora.sensorserver;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import android.util.Log;

public class SensorUDPServer implements Runnable {
    public static final String TAG = "server";
    public static final int port = 15225;
    
    public static final int PROTOCOL_REQUEST = 0x01;
    public static final int PROTOCOL_SENSOR = 0x02;
    
    OrientationSeneor sensor;
    // 어차피 서버는 1-1만 버티면 된다.
    Thread thread;
    int sequence;
    
    DatagramSocket socket;
    
    @Override
    public void run() {
        try {
            InetAddress serverAddr = InetAddress.getByName("0.0.0.0");
            socket = new DatagramSocket(port, serverAddr);

            while(true) {
                byte[] recvData = new byte[4];
                DatagramPacket recvPacket = new DatagramPacket(recvData, recvData.length);
                socket.receive(recvPacket);
                // 첫번째 구현에서는 클라의 요청이 1개뿐이니까 자세히 해석할 필요 없다      
                    
                InetAddress clientAddr = recvPacket.getAddress();
                int clientPort = recvPacket.getPort();
                DatagramPacket sendPacket = createSensorPacket(clientAddr, clientPort);
                socket.send(sendPacket);
            }
        } catch (Exception e) {
            Log.v(TAG, e.getClass().getName() + ':' + e.getMessage());
        } finally {
            if(socket != null && socket.isConnected()) {
                socket.close();
            }
            socket = null;
        }
    }
    
    public DatagramPacket createSensorPacket(InetAddress addr, int port) {
        ByteBuffer buffer = ByteBuffer.allocate(20);
        buffer.rewind();
        buffer.order(ByteOrder.LITTLE_ENDIAN);
        buffer.putInt(PROTOCOL_SENSOR);
        buffer.putInt(sequence);
        float yaw = sensor.rawOrientation[OrientationSeneor.YAW];
        float pitch = sensor.rawOrientation[OrientationSeneor.PITCH];
        float roll = sensor.rawOrientation[OrientationSeneor.ROLL];
        buffer.putFloat(yaw);
        buffer.putFloat(pitch);
        buffer.putFloat(roll);
        byte[] sendData = buffer.array();
        
        
        DatagramPacket packet = new DatagramPacket(sendData, sendData.length, addr, port);
        sequence++;
        return packet;
    }
    
    public SensorUDPServer(OrientationSeneor sensor) {
        this.sensor = sensor;
        this.sequence = 1;
    }
    
    public void start() {
        if(thread == null) {
            thread = new Thread(this);
            thread.start();
            Log.v(TAG, "Server Start");
        }
    }
    
    public void stop() {
        if(thread != null) {
            thread.interrupt();
            socket.close();
            thread = null;
            socket = null;
            Log.v(TAG, "Server Stop");
        }
    }
    
    static String getTime() {
        String name = Thread.currentThread().getName();
        SimpleDateFormat f = new SimpleDateFormat("[hh:mm:ss]", Locale.KOREA);
        return f.format(new Date()) + name;
    }
}
