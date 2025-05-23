using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AnimationToggle : MonoBehaviour
{
    public bool isInMotion = true;

    [SerializeField, Range(1, 100)]
    int rotationSpeed = 20;

    public bool xRot = false;
    public bool yRot = false;
    public bool zRot = false;

    public bool GetValue()
    {
        return isInMotion;
    }
    public Quaternion RotateFunc(float time) {
        Quaternion rotation = Quaternion.identity;
        if (xRot) {
            rotation *= Quaternion.AngleAxis(rotationSpeed * time, Vector3.up);
        }
        if (yRot) {
            rotation *= Quaternion.AngleAxis(rotationSpeed * time, Vector3.right);
        }
        if (zRot)
        {
            rotation *= Quaternion.AngleAxis(rotationSpeed * time, Vector3.forward);
        }
        return rotation;
    }
}
