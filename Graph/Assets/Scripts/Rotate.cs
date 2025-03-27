using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;

public class Rotate : MonoBehaviour
{

    [SerializeField]
    private AnimationToggle toggle;

    [SerializeField, Range(1, 100)]
    int rotationSpeed = 20;

   

    void Start()
    {
        
    }

    public void SetToggle(AnimationToggle newToggle)
    {
        toggle = newToggle;
    }

    void Update()
    {
        if (toggle.GetValue())
        {
           
            transform.rotation *= toggle.RotateFunc(Time.deltaTime);
        }
        else {

            transform.Rotate(0, 0, 0);
        }
        
    }


}
