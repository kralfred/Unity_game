using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;



public class Ticking : MonoBehaviour
{
    [SerializeField]
    Transform hoursPivot, minutesPivot, secondsPivot;

    void Awake() { 
        Debug.Log(DateTime.Now);
        hoursPivot.localRotation = Quaternion.Euler(0, 0, -30);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async Task Update()
    {
        //await 
    }
}
