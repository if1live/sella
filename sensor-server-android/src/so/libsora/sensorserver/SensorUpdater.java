package so.libsora.sensorserver;

import android.os.Handler;
import android.os.Looper;

public class SensorUpdater {
    private OrientationSeneor sensor;
    private int mInterval = 100;
    private Handler mHandler = new Handler(Looper.getMainLooper());
    private SensorUpdaterPolicy policy;
    private boolean running = false;
    
    public SensorUpdater(OrientationSeneor sensor, SensorUpdaterPolicy policy) {
        this.sensor = sensor;
        this.policy = policy;
    }
    
    void stop() {
        if(running) {
            mHandler.removeCallbacks(runnable);
            running = !running;
        }
    }
    void start() {
        if(running == false) {
            runnable.run();
            running = !running;
        }
    }
    
    Runnable runnable = new Runnable() {    
        @Override
        public void run() {
            //float[] data = sensor.filteredOrientation;
            float[] data = sensor.rawOrientation;
            float yaw = data[OrientationSeneor.YAW];
            float pitch = data[OrientationSeneor.PITCH];
            float roll = data[OrientationSeneor.ROLL];
            policy.run(yaw, pitch, roll);
            mHandler.postDelayed(runnable, mInterval);
        }
    };

}

