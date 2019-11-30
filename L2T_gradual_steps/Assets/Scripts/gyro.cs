using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyro : MonoBehaviour
{


    private Gyroscope gyr;
    private bool gyroEnabled;
    public static Quaternion attitude; // should be approached from other script 


    // Start is called before the first frame update
    void Start()
    {
        gyroEnabled = EnableGyro();
    }

    // Update is called once per frame
    void Update()
    {
        attitude = gyr.attitude;
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyr = Input.gyro;
            gyr.enabled = true;
            return true;
        }

        //todo- handle exceptions better- exit or print to screen..
        Debug.Log("Gyroscope could not be ebabled, check phone compatibility, then examine code");
        return false;

    }
}
