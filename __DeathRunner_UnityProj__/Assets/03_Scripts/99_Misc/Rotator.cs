using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Rotator : MonoBehaviour
{
    //Rotational Speed
    public float speed = 0f;
   
    //Forward Direction

    public bool ForwardY = false;

   
    //Reverse Direction
    public bool ReverseY = false;

   
    void Update ()
    {
        //Forward Direction

        if(ForwardY == true)
        {
            transform.Rotate(0, Time.deltaTime * speed,  0, Space.Self);
        }

        //Reverse Direction
        if(ReverseY == true)
        {
            transform.Rotate(0, -Time.deltaTime * speed,  0, Space.Self);
        }
    }
}