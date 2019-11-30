using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compassRotation : MonoBehaviour
{

    private GameObject compass; // will be used to store a graphical arrow
    private float bearing; // directio using only 2 GPS coordinates, regardless of phones heading
    Quaternion attitude; //will store the attitude from our gyroscope (related to phones heading)



    // Start is called before the first frame update
    void Start()
    {
        compass = GameObject.Find("navArrow"); //store game object        
    }

    // Update is called once per frame
    void Update()
    {
        bearing = angleFromCoordinate(32.772293f, 35.044495f, 32.35996f, 35.96337f);
        //Debug.Log("bearing = " + bearing.ToString());
        attitude = gyro.attitude;
        attitude[0] = 0;
        attitude[1] = 0;
        attitude[3] *= -1;  // tutorials just multiply like this

        compass.transform.rotation = attitude;
        compass.transform.rotation *= Quaternion.Slerp(compass.transform.rotation, Quaternion.Euler(0, 0, Input.compass.trueHeading + bearing), 1f);
        Debug.Log("rot = " + compass.transform.rotation.ToString());

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
