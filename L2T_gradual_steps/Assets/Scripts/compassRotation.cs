using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compassRotation : MonoBehaviour
{

    private GameObject compass; // will be used to store a graphical arrow
    private float bearing; // directio using only 2 GPS coordinates, regardless of phones heading
    public Text bearingText; //debug
    Quaternion attitude; //will store the attitude from our gyroscope (related to phones heading)

    //debug
    private float bearing_z;

    //Debug arrow- understand if to have magnetic or true heading
    private GameObject compass_magnetic;

    public Text headingAcc_text;

    // Start is called before the first frame update
    void Start()
    {
        compass = GameObject.Find("navArrow"); //store game object        
        compass_magnetic = GameObject.Find("navArrow_magnetic");

        //enable inner compass for heading accuracies sampling
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //bearing = angleFromCoordinate(32.7721325f, 35.0441824f, 32.7725536f, 35.043838f); // faith garden parallel road
        //Debug.Log("bearing = " + bearing.ToString());
        //bearing = angleFromCoordinate(GPS.Instance.latitude, GPS.Instance.longitude, Find_script.remote_lat, Find_script.remote_longi);
        //bearingText.text = "GPS bearing= " + bearing.ToString(); //debug
        //attitude = gyro.attitude;
        //attitude[0] = 0;
        //attitude[1] = 0;
        //attitude[3] *= -1;  // tutorials just multiply like this

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //debugg
        //take only z axis bearing
        //bearing_z = angleFromCoordinate(32.7721325f, 35.0441824f, 32.7725536f, 35.043838f); // faith garden parallel road
        bearing_z = angleFromCoordinate(GPS.Instance.latitude, GPS.Instance.longitude, Find_script.remote_lat, Find_script.remote_longi); // faith garden parallel road
        Debug.Log("bearing_z = " + bearing_z.ToString());


        //take only z rotation
        Debug.Log("Gyro_rot (x= " + gyro.attitude.eulerAngles.x.ToString()+",y= "+ gyro.attitude.eulerAngles.y.ToString()+",z= "+ gyro.attitude.eulerAngles.z.ToString());
        Debug.Log("True north= " + Input.compass.trueHeading.ToString());

        Debug.Log("comp rot Z = " + compass.transform.rotation.eulerAngles.z.ToString());

        compass.transform.rotation = Quaternion.Slerp(compass.transform.rotation, Quaternion.Euler(0f,0f, bearing_z+ Input.compass.trueHeading), 1f);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        compass_magnetic.transform.rotation = attitude;
        //version with magnetic heading
        compass_magnetic.transform.rotation *= Quaternion.Slerp(compass_magnetic.transform.rotation, Quaternion.Euler(0, 0, Input.compass.magneticHeading + bearing), 1f);
        //Debug.Log("rot magnet Head = " + compass_magnetic.transform.rotation.ToString());

        headingAcc_text.text = "Heading acc= " + Input.compass.headingAccuracy.ToString();
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
