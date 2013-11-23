package so.libsora.sensorserver;

public interface SensorUpdaterPolicy {
    void run(float yaw, float pitch, float roll);
}
