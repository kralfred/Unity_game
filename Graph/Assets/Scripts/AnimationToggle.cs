using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AnimationToggle : MonoBehaviour
{
    public bool isInMotion = true;

    public bool xRot = false;
    public bool yRot = false;
    public bool zRot = false;

    public bool GetValue()
    {
        return isInMotion;
    }
    public Quaternion RotateFunc(float degree) {
        Quaternion rotation = Quaternion.AngleAxis(degree,Vector3.zero);
        if (xRot) {
            rotation *= Quaternion.AngleAxis(degree, Vector3.up);
        }
        else if (yRot) {
            rotation *= Quaternion.AngleAxis(degree, Vector3.right);
        }
        else if (zRot)
        {
            rotation *= Quaternion.AngleAxis(degree, Vector3.forward);
        }
        return rotation;

    }
}
