using System;

public struct SensorValue {
	public int sequence;
	public float yaw;
	public float pitch;
	public float roll;
	
	override public string ToString() {
		return String.Format("{0} : {0}/{1}/{2}", sequence, yaw, pitch, roll);
		
	}
}
