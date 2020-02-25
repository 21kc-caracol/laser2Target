using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compassRotation : MonoBehaviour
{

    private GameObject compass; // will be used to store a graphical arrow.
    private float bearing; // directio using only 2 GPS coordinates, regardless of phones heading //not using this var
    public Text bearingText; //debug
    Quaternion attitude; //will store the attitude from our gyroscope (related to phones heading)

    //debug
    private float bearing_z; //rotate counter-clock. 0 degrees is TrueNorth
    public float azimuth; //== bearing_Z. this is for the other scripts. i don't want to expose bearing z.


    //debug 
    enum CompassDebug { None, All, Find_x_rotation,AZIMUTH };  //declare new type
    CompassDebug CompassDebugMode;  // declare a var from enum GpsDebug type

    // Start is called before the first frame update
    void Start()
    {
        CompassDebugMode = CompassDebug.AZIMUTH;
        azimuth = 0;

        compass = GameObject.Find("navArrow"); //store game object        

        //enable inner compass for heading accuracies sampling
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //debugg
        //take only z axis bearing
        //bearing_z = angleFromCoordinate(32.7721325f, 35.0441824f, 32.7725536f, 35.043838f); // faith garden parallel road

        bearing_z = angleFromCoordinate(GPS.Instance.latitude, GPS.Instance.longitude, Find_script.remote_lat, Find_script.remote_longi); 
        azimuth = bearing_z;

        if (CompassDebugMode == CompassDebug.All)
        {
            //take only z rotation
            Debug.Log("Gyro_rot (x= " + gyro.attitude.eulerAngles.x.ToString() + ",y= " + gyro.attitude.eulerAngles.y.ToString() + ",z= " + gyro.attitude.eulerAngles.z.ToString());
            Debug.Log("True north= " + Input.compass.trueHeading.ToString());
            Debug.Log("comp rot Z = " + compass.transform.rotation.eulerAngles.z.ToString());
            Debug.Log("bearing_z = " + bearing_z.ToString());
            Debug.Log("Heading acc= " + Input.compass.headingAccuracy.ToString());
        }

        if (CompassDebugMode == CompassDebug.Find_x_rotation)
        {
            Debug.Log("Gyro_rot (x= " + gyro.attitude.eulerAngles.x.ToString() + ",y= " + gyro.attitude.eulerAngles.y.ToString() + ",z= " + gyro.attitude.eulerAngles.z.ToString());
        }
        //{
        //   Debug.Log("comp rot Y = " + compass.transform.rotation.eulerAngles.y.ToString());
        //    Debug.Log("comp rot Z = " + compass.transform.rotation.eulerAngles.z.ToString());
        //}
        if (CompassDebugMode == CompassDebug.AZIMUTH)
        {
            Debug.Log("azimuth= " + azimuth.ToString());
        }


        compass.transform.rotation = Quaternion.Slerp(compass.transform.rotation, Quaternion.Euler(0f,0f, bearing_z+ Input.compass.trueHeading), 1f);

    }

    //calculate bearing angle- north is 0 degrees
    //contra to actual bearing, the function returns a counter-clock-wise angle
    // (actual bearing is clock-wise)
    private float angleFromCoordinate(float myLat, float myLong, float TargetLat, float TargetLong)
    {
        myLat *= Mathf.Deg2Rad;
        TargetLat *= Mathf.Deg2Rad;
        myLong *= Mathf.Deg2Rad;
        TargetLong *= Mathf.Deg2Rad;

        float dLon = (TargetLong - myLong);
        float y = Mathf.Sin(dLon) * Mathf.Cos(TargetLat);
        float x = (Mathf.Cos(myLat) * Mathf.Sin(TargetLat)) - (Mathf.Sin(myLat) * Mathf.Cos(TargetLat) * Mathf.Cos(dLon));
        float brng = Mathf.Atan2(y, x);
        brng = Mathf.Rad2Deg * brng;
        brng = (brng + 360) % 360;
        brng = 360 - brng; //this makes it from actual bearing which is calculated clockwise to counter-clockwise
        return brng;
    }



}
