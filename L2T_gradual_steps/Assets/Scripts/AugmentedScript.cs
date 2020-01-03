using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AugmentedScript : MonoBehaviour
{
    private Vector3 defaultPosition;


    //public- changed from Find script
    public Vector3 updatedPosition; 
    public Vector3 err_radius_vec;

    public bool ar_mode;  //TODO when pressing on AR_BUTTON CHANGE THIS TO TRUE AND WHEN 2D MODE CHANGE TO FALSE.
    //warning: also changed from Find script to control the z update

    private float speed = 1f;  //speed of animation //.1f


    //debug 
    enum ArDebug { None, All };  //declare new type
    ArDebug ArDebugMode;  // declare a var from enum GpsDebug type


    // Start is called before the first frame update
    void Start()
    {
        ArDebugMode = ArDebug.All; // FindDebug.GPS;

        ar_mode = false; //default is without AR

        defaultPosition = transform.localPosition; //lev check bugs. new Vector3(0, -5f, 0); ; //initial capsule position- below the cam, for 65 meters its good enough
        Debug.Log("capsule defaultPosition= " + defaultPosition.ToString());
        err_radius_vec = transform.localScale; //later update error radius
        Debug.Log("capsule err_radius_vec= " + err_radius_vec.ToString());

        updatedPosition = defaultPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ar_mode == true)
        {
            //update position 
            float minUpdateDelta = 2f;
            float zDifference = Mathf.Abs(transform.localPosition.z - updatedPosition.z);
            float eDifference = Mathf.Abs(transform.localScale.x - err_radius_vec.x);

            if (zDifference > minUpdateDelta)
            {
                transform.localPosition = updatedPosition; //Vector3.Lerp(transform.position, updatedPosition, speed); // z is updated from Find script //make less assignments- do it only on delta difference
            }            
            transform.localEulerAngles += new Vector3(0, 1f, 0); // circular movement

            if (eDifference > minUpdateDelta)
            {
                transform.localScale = err_radius_vec; // update error radius
            }

            if (ArDebugMode == ArDebug.All)
            {
                Debug.Log("AR current- trans.pos= " + transform.localPosition.ToString());
                Debug.Log("AR desired- updatedPosition= " + updatedPosition.ToString());
                Debug.Log("AR current- trans.lScale= " + transform.localScale.ToString());
            }

        }
    }


    public void enableAR()
    {
        //updatedPosition.x = 0;  //lev checks bugs - verify correct pos on AR start
        //updatedPosition.y = -5f; //lev checks bugs - verify correct pos on AR start
        updatedPosition = new Vector3(0, -5f, 0);

        if (ArDebugMode == ArDebug.All)
        {
            Debug.Log("enable AR trans.pos= " + transform.localPosition.ToString());
        }
        ar_mode = true;
    }

    public void disableAR()
    {
        ar_mode = false;
        transform.localPosition = defaultPosition; // Vector3.Lerp(transform.position, defaultPosition, speed);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1f, 1f, 1f); 
        if (ArDebugMode == ArDebug.All)
        {
            Debug.Log("disable trans.pos= " + transform.localPosition.ToString());
            Debug.Log("disable trans.lScale= " + transform.localScale.ToString());
        }            
    }
}
