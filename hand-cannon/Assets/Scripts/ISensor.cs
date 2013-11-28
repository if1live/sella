using UnityEngine;
using System.Collections;

public interface ISensor
{
    Vector3 eulerAngles { get; set; }

    Quaternion attitude { get; set; }
}

public class RealSensor : ISensor
{
    public RealSensor()
    {
        Input.gyro.enabled = true;
    }
 
    public Vector3 eulerAngles
    {
        get
        {
            Quaternion attitude = Input.gyro.attitude;
            return attitude.eulerAngles;
        }
        set {}
    }
 
    public Quaternion attitude
    { 
        get
        {
            return Input.gyro.attitude;
        }
        set {}
    }
}

public class FakeSensor : ISensor
{
    public Vector3 eulerAngles { get; set; }

    public Quaternion attitude { get; set; }
}

public class SensorFactory
{
    public ISensor Create()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return new RealSensor();
        }
        else
        {
            return new FakeSensor();
        }
    }
}