using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR_text_script : MonoBehaviour
{

    //get access to transform from other script 
    public AugmentedScript ar_script;
    private Vector3 textPosition;
    private float yAxisFix= -15;
    private float zAxisFix = 300;
    private float xAxisFix = -30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        textPosition = new Vector3(ar_script.transform.localPosition.x+ xAxisFix, ar_script.transform.localPosition.y + yAxisFix, zAxisFix);
        Debug.Log("ar_script.transform.localPosition.y= " + ar_script.transform.localPosition.y.ToString()); //  -5//
        transform.localPosition = textPosition;

    }
}
