using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    Vector3 position;
    [SerializeField, Range(1, 100)]
    int resolution = 30;

    Transform[] points;

    void Start()
    {
    }

    private void Awake()
    {

        points = new Transform[resolution];

        float step = 2f / resolution;

        var position = Vector3.zero;
        var scale = Vector3.one * step;


        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            position.x = (i + 0.5f) * step - 1f;
            point.localPosition = position;
            point.localScale = scale;
        }
    }
    void Update()
    {

        float time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + time));
            point.localPosition = position;
        }
    }

    async Task Move()
    {
        float step = 2f / resolution;

        var position = Vector3.zero;
        var scale = Vector3.one * step;
        Transform point = Instantiate(pointPrefab, transform);

        for (int i = 0; i < resolution; i++)
        {
            position.x = (i + 0.5f) * step - 1f;
            position.y = Mathf.Sin(Mathf.PI * position.x);
            point.localPosition = position;
            point.localScale = scale;
            await Task.Delay(100);
        }
        Destroy(point.gameObject);
    }



}
