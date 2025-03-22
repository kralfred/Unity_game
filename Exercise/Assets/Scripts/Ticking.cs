using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;



public class Ticking : MonoBehaviour
{
    [SerializeField]
    Transform minutesPivot, secondsPivot, hoursPivot;

    // Awake can stay void since we won't await an infinite task
    void Awake()
    {
        
        SetInMotion();
        
    }

    async void SetInMotion()
    {
        minutesPivot.localRotation = Quaternion.Euler(0, 0, DateTime.Now.Minute * (-6));

        hoursPivot.localRotation = Quaternion.Euler(0, 0, DateTime.Now.Hour * (-30) + (DateTime.Now.Minute * (-1)/4));

        await Move(secondsPivot, 1000, DateTime.Now.Second * (-6));
    }

    async Task Move(Transform myTransform, int delay, int currentPosition) {

        Debug.Log("Current DateTime: " + DateTime.Now);
        while (true) { 
           
           myTransform.localRotation = Quaternion.Euler(0, 0, currentPosition);
            if (currentPosition % 360 == 0) {
                minutesPivot.Rotate(0,0,-6);
                if (currentPosition % 4 == 0) { 
                   hoursPivot.Rotate(0,0,-1);
                }
                
            }
            currentPosition -= 6;
           await Task.Delay(delay); 
        }
    }


    // Update is called once per frame
    void Update()
    {
     
    }
}
