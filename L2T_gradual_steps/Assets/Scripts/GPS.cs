using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }

    // TODO  improve: i could just pass Input.location.lastData instead of all the other stuff
    //the required information from my GPS
    public float latitude;
    public float longitude;
    public float altitude;
    public float horizontal_accuracy; //in Meters
    public float vertical_accuracy; //in Meters

    //util variables
    float gpsUpdateRate; // update GPS data on the change of <gpsUpdateRate>
    int gpsDesiredAccuracy; // GPS will try to rech this accuracy in meters

    //debug 
    enum GpsDebug {None, All};  //declare new type
    GpsDebug debugMode;  // declare a var from enum GpsDebug type

    // Start is called before the first frame update
    private void Start()
    {
        //debug
        debugMode = GpsDebug.None;

        //initialization
        gpsUpdateRate = 0.1f;
        gpsDesiredAccuracy = 5;

        latitude = 0;
        longitude = 0;
        altitude = 0;
        horizontal_accuracy = 0;
        vertical_accuracy = 0;

        if (debugMode != GpsDebug.None)
        {
            Debug.Log("Start: Lat= " + latitude.ToString());
            Debug.Log("Start: Longi= " + longitude.ToString());
            Debug.Log("Start: Alte= " + altitude.ToString());
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);  //let gps instance live through-out the program
        StartCoroutine(StartLocationService()); //when using delay commands you have to use a CoRoutine
    }

    private IEnumerator StartLocationService()
    {
        Debug.Log("StartLocationService");
        //yield return new WaitForSeconds(3);
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Input.location.isEnabledByUser= " + Input.location.isEnabledByUser.ToString());
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        Input.location.Start(gpsDesiredAccuracy, gpsUpdateRate);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }


        // get the GPS data
        latitude = Input.location.lastData.latitude;
        Debug.Log("StartLocationService: lat = " + latitude.ToString());

        longitude = Input.location.lastData.longitude;
        Debug.Log("StartLocationService: Longi = " + longitude.ToString());

        altitude = Input.location.lastData.altitude;
        Debug.Log("StartLocationService: Alt = " + altitude.ToString());

        horizontal_accuracy = Input.location.lastData.horizontalAccuracy;
        vertical_accuracy = Input.location.lastData.verticalAccuracy;

        yield break;
    }

    private void Update()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitude = Input.location.lastData.altitude;
        horizontal_accuracy = Input.location.lastData.horizontalAccuracy;
        vertical_accuracy = Input.location.lastData.verticalAccuracy;


        if (debugMode != GpsDebug.None)
        {
            Debug.Log("Update: latitude= " + latitude.ToString());
            Debug.Log("Update: longitude= " + longitude.ToString());
            Debug.Log("Update: Alt = " + altitude.ToString());
        }
    }
}
