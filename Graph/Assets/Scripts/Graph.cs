using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    Vector3 position;
    [SerializeField, Range(1, 100)]
    int resolution = 30;


    void Start()
    {
    }

    private void Awake()
    {

        float step = 2f / resolution;

        var position = Vector3.zero;
        var scale = Vector3.one * step;

        for (int i = 0; i < resolution; i++)
        {

            position.x = (i + 0.5f) * step - 1f;
            Transform point = Instantiate(pointPrefab);
            position.y = position.x * position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;

        }
    }


    void Update()
    {

    }
}
