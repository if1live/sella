package so.libsora.sensorserver;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

public class OrientationSeneor implements SensorEventListener {
    public static final int YAW = 0;
    public static final int AZIMUTH = YAW;
    public static final int PITCH = 1;
    public static final int ROLL = 2;
    
	public float orientation[] = new float[3];
	public float filteredOrientation[] = new float[3];
	public float rawOrientation[] = new float[3];
	
	private final float INVALID_BASE_AZIMUTH = 10000000f;
	private float baseAzimuth = INVALID_BASE_AZIMUTH;
	
	private Sensor sensor;
	private SensorManager sensorManager;
	
	public OrientationSeneor(SensorManager sensorManager) {
		this.sensorManager = sensorManager;
		sensor = sensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
	}
	
	public void registerListener() {
		sensorManager.registerListener(this, sensor, SensorManager.SENSOR_DELAY_GAME);
		reset();
	}
	public void unregisterListener() {
		sensorManager.unregisterListener(this, sensor);
	}
	
	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) {
	}
	@Override
	public void onSensorChanged(SensorEvent event) {
		for(int i = 0 ; i < 3 ; ++i) {
			rawOrientation[i] = event.values[i];
		}
		if(baseAzimuth == INVALID_BASE_AZIMUTH) {
		    baseAzimuth = event.values[0];
		}
		for(int i = 0 ; i < 3 ; ++i) {
			orientation[i] = rawOrientation[i];
		}
		orientation[0] -= baseAzimuth;
		
		for(int i = 0 ; i < 3 ; ++i) {
			filteredOrientation[i] = calculateFilteredAngle(orientation[i], filteredOrientation[i]);
		}
	}
	public void reset() {
		baseAzimuth = INVALID_BASE_AZIMUTH;
		for(int i = 0 ; i < 3 ; ++i) {
		    orientation[i] = 0;
		    filteredOrientation[i] = 0;
		}
	}
	
	//http://stackoverflow.com/questions/10192057/android-getorientation-method-returns-bad-results
	//x is a raw angle value from getOrientation(...)
	//y is the current filtered angle value
	private float calculateFilteredAngle(float x, float y){ 
		final float alpha = 0.3f;
		float diff = x-y;

		//here, we ensure that abs(diff)<=180
		diff = restrictAngle(diff);

		y += alpha*diff;
		//ensure that y stays within [-180, 180[ bounds
		y = restrictAngle(y);

		return y;
	}
	
	private float restrictAngle(float tmpAngle){
        while(tmpAngle>=180) tmpAngle-=360;
        while(tmpAngle<-180) tmpAngle+=360;
        return tmpAngle;
    }
}
