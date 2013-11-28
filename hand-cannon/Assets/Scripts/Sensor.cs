using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour
{
    public GUIText text;
    public Transform cubeTransform;
    ISensor sensor;

    // Use this for initialization
    void Start()
    {
        SensorFactory factory = new SensorFactory();
        sensor = factory.Create();
    }
 
    // Update is called once per frame
    void Update()
    {
        cubeTransform.rotation = sensor.attitude;

        Vector3 angles = sensor.eulerAngles;
        float yaw = angles.x;
        float pitch = angles.y;
        float roll = angles.z;
     
        string str = string.Format("{0} {1} {2}", yaw, pitch, roll);
        text.text = str;
        //attitude.
     
    }
}
