using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // for  gps_dummy
using Photon.Realtime;
using Photon.Pun;
using System.Globalization;
using Vuforia;
using System;

public class Find_script : Photon.Pun.MonoBehaviourPunCallbacks, Photon.Pun.IPunObservable
{
    private int my_count;
    private int remote_count;

    //gps data to pass through the Network 
    public static float remote_lat;  //used by rotation script (compassRotation)
    public static float remote_longi;  //used by rotation script (compassRotation)
    private float remote_alt;
    private float remote_horizontal_acc;
    private float remote_vertical_acc;

    private int maxValidARDist = 65;

    [Space(5)]  //space between vars in the editor of Unity
    public Text my_count_text;
    [Space(5)]
    public Text remote_count_text;
    [Space(5)]
    public GameObject buttonAddToMyCount;
    public GameObject buttonLoadAR;
    public GameObject buttonLoad2D;
    public GameObject buttonLoadDebug;
    public GameObject background;
    //added by maya
    public GameObject targetsScreen;
    public static GameObject compass1;

    public Text closeToTheTarget;
    public Text howToHoldYourPhone;

    //gps data text
    [Space(15)]
    public Text remote_lat_text;
    public Text remote_longi_text;
    public Text remote_alt_text;
    public Text remote_horizontal_acc_text;
    public Text remote_vertical_acc_text;
    //gps calculations text
    public Text distance_to_target_text;

    //debug 
    enum FindDebug { None, All, GPS, Photon, Pit };  //declare new type
    FindDebug FindDebugMode;  // declare a var from enum GpsDebug type


    //lev get access to functions from other script 
    public AugmentedScript ar_script;


    //maya get access to GPS data from other script 
    public Update_GPS_text gpsData;

    // Start is called before the first frame update
    void Start()
    {
        //lev initializations
        //variables
        FindDebugMode = FindDebug.Pit; // FindDebug.GPS;
        my_count = 0;
        remote_count = 0;

        remote_lat = 0;
        remote_longi = 0;
        remote_alt = 0;
        remote_horizontal_acc = 0;
        remote_vertical_acc = 0;

        //my_count_text = GetComponent<Text>(); //this command didnt work so i comment for documentation
        SetMyCountText();
        SetRemoteCountText();

        //text object handling
        SetRemoteLatText();
        SetRemoteLongiText();
        SetRemoteAltText();
        SetRemoteHorizonAccText();
        SetRemoteVerticalAccText();

        if (!PhotonNetwork.IsConnected) // 1
        {
            SceneManager.LoadScene("Initial_menu_scene");
            // TODO improve: maybe you can improve performance by shutting down here the GPS  
            return;
        }

        compass1 = GameObject.Find("navArrow");
        
        //added by maya
        //if you are the target
        if (PhotonNetwork.IsMasterClient == true)
        {
            buttonLoadAR.SetActive(false);
            buttonLoadDebug.SetActive(false);
            buttonLoad2D.SetActive(false);
            targetsScreen.SetActive(true);
            background.SetActive(false);
            howToHoldYourPhone.gameObject.SetActive(false);

            remote_lat_text.color = Color.clear;
            remote_longi_text.color = Color.clear;
            remote_alt_text.color = Color.clear;
            remote_horizontal_acc_text.color = Color.clear;
            remote_vertical_acc_text.color = Color.clear;

            gpsData.latitude_text.gameObject.SetActive(false);
            gpsData.alt_text.gameObject.SetActive(false);
            gpsData.longitude_text.gameObject.SetActive(false);
            gpsData.horizontal_accuracy_text.gameObject.SetActive(false);
            gpsData.vertical_accuracy_text.gameObject.SetActive(false);

            compass1.SetActive(false);

            closeToTheTarget.text = "\nthe LASER is\n looking for you\n\n wait...\n it won't take long";
            closeToTheTarget.color = Color.magenta;
            distance_to_target_text.gameObject.SetActive(false);
        }

        else
        {
            buttonLoadAR.SetActive(true);
            buttonLoadDebug.SetActive(true);
            buttonLoad2D.SetActive(false);
            background.SetActive(true);
            howToHoldYourPhone.gameObject.SetActive(true);
            howToHoldYourPhone.text = "Make sure your phone is HORIZONTAL";
            howToHoldYourPhone.color = new Color32(245, 109, 14, 255);

            //added by maya
            remote_lat_text.color = Color.clear;
            remote_longi_text.color = Color.clear;
            remote_alt_text.color = Color.clear;
            remote_horizontal_acc_text.color = Color.clear;
            remote_vertical_acc_text.color = Color.clear;

            gpsData.latitude_text.gameObject.SetActive(false);
            gpsData.alt_text.gameObject.SetActive(false);
            gpsData.longitude_text.gameObject.SetActive(false);
            gpsData.horizontal_accuracy_text.gameObject.SetActive(false);
            gpsData.vertical_accuracy_text.gameObject.SetActive(false);

            targetsScreen.SetActive(false);
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            SetMyCountText();
            SetRemoteCountText();

            SetRemoteLatText();
            SetRemoteLongiText();
            SetRemoteAltText();

            SetRemoteHorizonAccText();
            SetRemoteVerticalAccText();

            setDistanceText();
            setErrorRadius();
        }

        

        //changeTargetScreen();
    }

    public void OnAddToMyCount()
    {
        my_count = my_count + 1;
        Debug.Log("OnAddToMyCount: my_count= " + my_count.ToString());
    }

    void SetMyCountText()
    {
        my_count_text.text = "my_count: " + my_count.ToString();
        if (FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetMyCountText: my_count_text= " + my_count_text.text);
        }
    }

    void SetRemoteCountText()
    {
        remote_count_text.text = "remotecount: " + remote_count.ToString();
        if (FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteCountText: remote_count_text= " + remote_count_text.text);
        }
    }

    void SetRemoteLatText()
    {
        remote_lat_text.text = "remote Lat: " + remote_lat.ToString();

        if (FindDebugMode == FindDebug.GPS || FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteLatText: remote_lat_text= " + remote_lat_text.text);
        }
    }

    void SetRemoteLongiText()
    {
        remote_longi_text.text = "remote longi: " + remote_longi.ToString();

        if (FindDebugMode == FindDebug.GPS || FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteLongiText: remote_longi_text= " + remote_longi_text.text);
        }    
    }

    void SetRemoteAltText()
    {
        remote_alt_text.text = "remote alt: " + remote_alt.ToString();

        if (FindDebugMode == FindDebug.GPS || FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteAltText: remote_alt_text= " + remote_alt_text.text);
        }
    }

    void SetRemoteHorizonAccText()
    {
        remote_horizontal_acc_text.text = "remote Horizontal acc: " + remote_horizontal_acc.ToString();

        if (FindDebugMode == FindDebug.GPS || FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteHorizonAccText: remote_horizontal_acc_text= " + remote_horizontal_acc_text.text);
        }
    }

    void SetRemoteVerticalAccText()
    {
        remote_vertical_acc_text.text = "remote Vertical acc: " + remote_vertical_acc.ToString();

        if (FindDebugMode == FindDebug.GPS || FindDebugMode == FindDebug.All)
        {
            Debug.Log("SetRemoteVerticalAccText: remote_vertical_acc_text= " + remote_vertical_acc_text.text);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //i'm the data owner
            stream.SendNext(my_count);
            stream.SendNext(GPS.Instance.latitude.ToString());
            stream.SendNext(GPS.Instance.longitude.ToString());
            stream.SendNext(GPS.Instance.altitude.ToString());
            stream.SendNext(GPS.Instance.horizontal_accuracy.ToString());
            stream.SendNext(GPS.Instance.vertical_accuracy.ToString());


            if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon)
            {
                Debug.Log("Sending: my_count= " + my_count.ToString());
                Debug.Log("Sending: latitude= " + GPS.Instance.latitude.ToString());
                Debug.Log("Sending: longitude= " + GPS.Instance.longitude.ToString());
                Debug.Log("Sending: altitude= " + GPS.Instance.altitude.ToString());
                Debug.Log("Sending: horizontal_accuracy= " + GPS.Instance.horizontal_accuracy.ToString());
                Debug.Log("Sending: vertical_accuracy= " + GPS.Instance.vertical_accuracy.ToString());
            }

        }
        else if(stream.IsReading)
        {
            if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon)
            {
                Debug.Log("stream.IsReading is excecuting");
            }
            //receive others data

            SetRemoteCount((int)stream.ReceiveNext()); // should be count

            SetRemoteLat((string)stream.ReceiveNext()); // should be lat
            SetRemoteLongi((string)stream.ReceiveNext()); // should be longi
            SetRemoteAlt((string)stream.ReceiveNext()); // should be alt
            SetRemoteHorizontalAcc((string)stream.ReceiveNext()); // should be horizontal accuracy
            SetRemoteVerticalAcc((string)stream.ReceiveNext()); // should be vertical accuracy
        }
    }

    private void SetRemoteCount(int remoteCount)
    {
        if(remote_count == remoteCount)
        {
            return;
        }

        remote_count = remoteCount;
        //Debug.Log("SetRemoteCount: remote_count= " + remote_count.ToString());
    }

    private void SetRemoteLat(string remoteLat)
    {       
        if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon || FindDebugMode == FindDebug.GPS)
        {
            Debug.Log("SetRemoteLat: remoteLat= " + remoteLat.ToString());  //already a string
        }

        if (remote_lat == float.Parse(remoteLat, CultureInfo.InvariantCulture.NumberFormat))
        {
            return;
        }
        remote_lat = float.Parse(remoteLat, CultureInfo.InvariantCulture.NumberFormat);
    }

    private void SetRemoteLongi(string remoteLongi)
    {
        if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon || FindDebugMode == FindDebug.GPS)
        {
            Debug.Log("SetRemoteLongi: remoteLongi= " + remoteLongi.ToString());  //already a string
        }

        if (remote_longi == float.Parse(remoteLongi, CultureInfo.InvariantCulture.NumberFormat))
        {
            return;
        }
        remote_longi = float.Parse(remoteLongi, CultureInfo.InvariantCulture.NumberFormat);

    }

    private void SetRemoteAlt(string remoteAlt)
    {
        if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon || FindDebugMode == FindDebug.GPS)
        {
            Debug.Log("SetRemoteAlt: remoteAlt= " + remoteAlt.ToString());  //already a string
        }

        if (remote_alt == float.Parse(remoteAlt, CultureInfo.InvariantCulture.NumberFormat))
        {
            return;
        }
        remote_alt = float.Parse(remoteAlt, CultureInfo.InvariantCulture.NumberFormat);

    }

    private void SetRemoteHorizontalAcc(string HoriAccuracy)
    {
        if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon || FindDebugMode == FindDebug.GPS)
        {
            Debug.Log("SetRemoteHorizontalAcc: HoriAccuracy= " + HoriAccuracy.ToString());  //already a string
        }

        if (remote_horizontal_acc == float.Parse(HoriAccuracy, CultureInfo.InvariantCulture.NumberFormat))
        {
            return;
        }
        remote_horizontal_acc = float.Parse(HoriAccuracy, CultureInfo.InvariantCulture.NumberFormat);
    }



    private void SetRemoteVerticalAcc(string VertAccuracy)
    {
        if (FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.Photon || FindDebugMode == FindDebug.GPS)
        {
            Debug.Log("SetRemoteHorizontalAcc: HoriAccuracy= " + VertAccuracy.ToString());  //already a string
        }

        if (remote_vertical_acc == float.Parse(VertAccuracy, CultureInfo.InvariantCulture.NumberFormat))
        {
            return;
        }
        remote_vertical_acc = float.Parse(VertAccuracy, CultureInfo.InvariantCulture.NumberFormat);
    }


    //find distance

    float DegToRad(float deg)
    {
        float temp;
        temp = (deg * Mathf.PI) / 180.0f;
        temp = Mathf.Tan(temp);
        return temp;
    }

    float Distance_x(float lon_a, float lon_b, float lat_a, float lat_b)
    {
        float temp;
        float c;
        temp = (lat_b - lat_a);
        c = Mathf.Abs(temp * Mathf.Cos((lat_a + lat_b)) / 2);
        return c;
    }

    private float Distance_y(float lat_a, float lat_b)
    {
        float c;
        c = (lat_b - lat_a);
        return c;
    }

    float Final_distance(float x, float y)
    {
        float c;
        c = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f))) * 6371;
        return c;
    }

    //*******************************
    //This is the function to call to calculate the distance between two points in meters

    private float Calculate_Distance_Meters(float long_a, float lat_a, float long_b, float lat_b)
    {
        if(FindDebugMode == FindDebug.All || FindDebugMode == FindDebug.GPS)
        {
            //todo - the target sees strange values because it doesnt receive any data so it shouldnt display any distance
            Debug.Log("Calculate_Distance_Meters: long_a= " + long_a.ToString());
            Debug.Log("Calculate_Distance_Meters: lat_a= " + lat_a.ToString());
            Debug.Log("Calculate_Distance_Meters: long_b= " + long_b.ToString());
            Debug.Log("Calculate_Distance_Meters: lat_b= " + lat_b.ToString());
        }

        float a_long_r, a_lat_r, p_long_r, p_lat_r, dist_x, dist_y, total_dist;
        a_long_r = DegToRad(long_a);
        a_lat_r = DegToRad(lat_a);
        p_long_r = DegToRad(long_b);
        p_lat_r = DegToRad(lat_b);
        dist_x = Distance_x(a_long_r, p_long_r, a_lat_r, p_lat_r);
        dist_y = Distance_y(a_lat_r, p_lat_r);
        total_dist = Final_distance(dist_x, dist_y) * 1000; //divide by 1000 to get meters

        return total_dist;

    }

    private void setDistanceText()
    {
        float distance_to_target = Calculate_Distance_Meters(GPS.Instance.longitude, GPS.Instance.latitude, remote_longi, remote_lat);
        // float distance_to_target = Calculate_Distance_Meters(35.044251f, 32.772542f, 35.044542f, 32.772231f);  //worked! it is 44 meters
        
        //checking pitagoras
        float pitagorasDist = CalcPitagorasDist_Meters(GPS.Instance.longitude, GPS.Instance.latitude, remote_longi, remote_lat);
        //checking pitagoras
        if (FindDebugMode == FindDebug.Pit)
        {
            Debug.Log("pitagorasDist= " + pitagorasDist.ToString() + "def= " + distance_to_target.ToString() + "Delta= " + (pitagorasDist - distance_to_target).ToString());
        }

        if (ar_script.ar_mode == true)
        {
            //ar_script.updatedPosition.z = distance_to_target;  //update Z value for the AR script
            //ar_script.updatedPosition.x = 0f;
            //ar_script.updatedPosition.y = -5f;
            float y_plane_val = Y_plane_correction(distance_to_target);
            ar_script.updatedPosition = new Vector3(0, y_plane_val, distance_to_target);


        }

        if (PhotonNetwork.IsMasterClient == false)
        {
            distance_to_target_text.text = "Distance to Target:\n" + distance_to_target.ToString();
            distance_to_target_text.color = new Color32(255, 227, 197, 255);
        }
        
        //else
        //{
            //distance_to_target_text.text = "Distance from Laser:\n" + distance_to_target.ToString();
           // distance_to_target_text.color = new Color32(255, 227, 197, 255);
        //}

        // We want it only on one phone. posible?????
        //if you are the laser, not the target
        if (PhotonNetwork.IsMasterClient == false)
        {
            if (distance_to_target < maxValidARDist)
            {
                closeToTheTarget.text = "You are very close to your TARGET\n switch to AR mode";
                closeToTheTarget.color = Color.magenta;
                //howToHoldYourPhone.gameObject.SetActive(false);


            } else
            {
                closeToTheTarget.text = "";
                //howToHoldYourPhone.gameObject.SetActive(true);
            }
        } else
        { // you are target
            if (distance_to_target < maxValidARDist)
            {
                closeToTheTarget.text = "\n \nthe Laser is very close to you \n \nyou can wave to him!";
                closeToTheTarget.color = Color.magenta;
            }
            else
            {
                closeToTheTarget.text = "\nthe LASER is\n looking for you\n\n wait...\n it won't take long";
                closeToTheTarget.color = Color.magenta;
            }
        }
            
    }

    private void sleepfor()
    {
        throw new NotImplementedException();
    }

    public void LoadAR()
    {
        buttonLoadAR.SetActive(false);
        buttonLoadDebug.SetActive(true);
        buttonLoad2D.SetActive(true);
        background.SetActive(false);
        compass1.SetActive(false);
        howToHoldYourPhone.gameObject.SetActive(true);
        howToHoldYourPhone.text = "Make sure your phone is VERTICAL";
        howToHoldYourPhone.color = new Color32(245,109, 14, 255);
        //VuforiaRuntime.Instance.InitVuforia(); // This is how you turn vuforia on!
        //closeToTheTarget.text = "";
        closeToTheTarget.gameObject.SetActive(false);

        ar_script.enableAR(); //lev enabling ar mode

        //added by maya
        remote_lat_text.color = Color.clear;
        remote_longi_text.color = Color.clear;
        remote_alt_text.color = Color.clear;
        remote_horizontal_acc_text.color = Color.clear;
        remote_vertical_acc_text.color = Color.clear;
        gpsData.latitude_text.gameObject.SetActive(false);
        gpsData.alt_text.gameObject.SetActive(false);
        gpsData.longitude_text.gameObject.SetActive(false);
        gpsData.horizontal_accuracy_text.gameObject.SetActive(false);
        gpsData.vertical_accuracy_text.gameObject.SetActive(false);


    }

    public void Load2D()
    {
        buttonLoadAR.SetActive(true);
        buttonLoadDebug.SetActive(true);
        buttonLoad2D.SetActive(false);
        //compass1 = GameObject.Find("navArrow");
        compass1.SetActive(true);
        background.SetActive(true);
        howToHoldYourPhone.gameObject.SetActive(true);
        howToHoldYourPhone.text = "Make sure your phone is HORIZONTAL";
        howToHoldYourPhone.color = new Color32(245, 109, 14, 255);

        ar_script.disableAR();  //lev disabling ar mode

        //added by maya
        remote_lat_text.color = Color.clear;
        remote_longi_text.color = Color.clear;
        remote_alt_text.color = Color.clear;
        remote_horizontal_acc_text.color = Color.clear;
        remote_vertical_acc_text.color = Color.clear;
        gpsData.latitude_text.gameObject.SetActive(false);
        gpsData.alt_text.gameObject.SetActive(false);
        gpsData.longitude_text.gameObject.SetActive(false);
        gpsData.horizontal_accuracy_text.gameObject.SetActive(false);
        gpsData.vertical_accuracy_text.gameObject.SetActive(false);

        closeToTheTarget.gameObject.SetActive(true);
    }

    public void LoadDebug()
    {
        buttonLoadAR.SetActive(false);
        buttonLoadDebug.SetActive(false);
        buttonLoad2D.SetActive(true);
        //compass1 = GameObject.Find("navArrow");
        compass1.SetActive(false);
        background.SetActive(true);
        howToHoldYourPhone.gameObject.SetActive(true);
        howToHoldYourPhone.text = "Debug Mode";
        howToHoldYourPhone.color = new Color32(245, 109, 14, 255);
        closeToTheTarget.gameObject.SetActive(false);

        ar_script.disableAR();  //lev disabling ar mode

        //added by maya
        remote_lat_text.color = Color.white;
        remote_longi_text.color = Color.white;
        remote_alt_text.color = Color.white;
        remote_horizontal_acc_text.color = Color.white;
        remote_vertical_acc_text.color = Color.white;
        gpsData.latitude_text.gameObject.SetActive(true);
        gpsData.alt_text.gameObject.SetActive(true);
        gpsData.longitude_text.gameObject.SetActive(true);
        gpsData.horizontal_accuracy_text.gameObject.SetActive(true);
        gpsData.vertical_accuracy_text.gameObject.SetActive(true);
    }

    void setErrorRadius()
    {
        float total_err_walking_plane = GPS.Instance.horizontal_accuracy + remote_horizontal_acc; //assumption: humans dont fly up
        float max_limit_gps_err = 10; //lev var to limit capsule size
        if (total_err_walking_plane > 0)
        {
            if(total_err_walking_plane > max_limit_gps_err)
            {
                ar_script.err_radius_vec = new Vector3(2f * max_limit_gps_err, 1f, 1.2f); // take max error for horizontal plane
            }            
            else
            {
                ar_script.err_radius_vec = new Vector3(2f * total_err_walking_plane, 1f, 1.2f); // take max error for horizontal plane
            }
        }        
    }

    float Y_plane_correction(float distance_to_target)
    {
        const float REQ_RATIO = 13;
        float y_value= (distance_to_target / REQ_RATIO) * (-1); //-1 for correction downwards
        return y_value;
    }

    float CalcPitagorasDist_Meters(float long_a, float lat_a, float long_b, float lat_b)
    {
        float DeltaLong = Mathf.Abs(long_a - long_b);
        float DeltaLat = Mathf.Abs(lat_a - lat_b);
        float dist = Mathf.Sqrt( Mathf.Pow(DeltaLong, 2) + Mathf.Pow(DeltaLat, 2));

        return dist * 100000; //for meters
    }
}
