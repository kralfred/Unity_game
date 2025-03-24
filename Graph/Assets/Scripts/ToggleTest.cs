using UnityEngine;
using UnityEngine.UI;

public class ToggleTest : MonoBehaviour
{
    [SerializeField] private Toggle toggle; // Assign the Toggle here

    void Start()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(value => Debug.Log("Toggle is: " + (value ? "On" : "Off")));
            Debug.Log("Toggle found and set up.");
        }
        else
            Debug.LogError("Toggle not assigned!");
    }
}
