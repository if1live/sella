package so.libsora.sensorserver;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.text.SimpleDateFormat;
import java.util.Date;

import android.util.Log;

public class SensorTCPServer implements Runnable {
    public static final String TAG = "server";
    public static final int port = 23223;
    
    public static final String CMD_REQUEST = "req";
    public static final String CMD_QUIT = "quit";
    
    ServerSocket serverSocket;
    // 어차피 서버는 1-1만 버티면 된다.
    Thread thread;
    OrientationSeneor sensor;
    
    public SensorTCPServer(OrientationSeneor sensor) {
        this.sensor = sensor;
        
        try {
            serverSocket = new ServerSocket(port);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    
    public void start() {
        thread = new Thread(this);
        thread.start();
    }
    
    @Override
    public void run() {
        while (true) {
            try {
                Log.v(TAG, getTime() + " Waiting connection...");
                
                Socket socket = serverSocket.accept();
                Log.v(TAG, getTime() + " " + socket.getInetAddress() + "로부터 연결요청이 들어왔습니다.");
 
                OutputStream out = socket.getOutputStream();
                InputStream in = socket.getInputStream();
                DataOutputStream dos = new DataOutputStream(out);
                DataInputStream dis = new DataInputStream(in);
                
                boolean running = true;
                while(running) {
                    String cmd = dis.readLine();
                    running = handleCommand(cmd, dos);
                }
                Log.v(TAG, getTime() + "Connection Quit");
                dos.close();
                socket.close();
            } catch (IOException e) {
                Log.v(TAG, "socket fail");
                e.printStackTrace();
            }
        }
    }
    
    public boolean handleCommand(String cmd, DataOutputStream dos) throws IOException {
        Log.v(TAG, cmd);
        if(cmd.equals(CMD_QUIT)) {
            return onQuit(dos);
        } else if(cmd.equals(CMD_REQUEST)) {
            return onRequest(dos);
        } else {
            return onUnknownCmd(dos);
        }
    }
    
    public boolean onQuit(DataOutputStream dos) throws IOException {
        String msg = "quit";
        dos.writeBytes(msg);
        dos.writeByte('\n');
        dos.flush();        
        return false;
    }
    public boolean onRequest(DataOutputStream dos) throws IOException {
        float yaw = sensor.orientation[OrientationSeneor.YAW];
        float pitch = sensor.orientation[OrientationSeneor.PITCH];
        float roll = sensor.orientation[OrientationSeneor.ROLL];
        String msg = String.format("%.3f/%.3f/%.3f", yaw, pitch, roll);
        dos.writeBytes(msg);
        dos.writeByte('\n');
        dos.flush();
        return true;
    }    
    public boolean onUnknownCmd(DataOutputStream dos) throws IOException {
        dos.writeBytes("unknown");
        dos.writeByte('\n');
        dos.flush();
        return true;
    }
    
    static String getTime() {
        String name = Thread.currentThread().getName();
        SimpleDateFormat f = new SimpleDateFormat("[hh:mm:ss]");
        return f.format(new Date()) + name;
    }
}
