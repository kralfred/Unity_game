using UnityEngine;
using UnityEngine.UI;

public class AnimationToggle : MonoBehaviour
{
    public bool isInMotion = true;

    void Start()
    {
       
    }

    public bool GetValue() {

        return isInMotion;
    }

}
