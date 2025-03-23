using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

    [SerializeField, Range(0, 2)]
    int function;

    Transform[] points;
    List<Func<float,float, float>>  waveFunctions = new List<Func<float, float, float>>();
    Func<float, float, float> waveFunc;

    void Start()
    {
    }

    private void Awake()
    {
        waveFunctions.Add(FunctionLibrary.Wave);
        waveFunctions.Add(FunctionLibrary.MultiWave);
        waveFunctions.Add(FunctionLibrary.Ripple);

        waveFunc = waveFunctions.ElementAt(function);

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
        InitiateAsync();
    }
    async void InitiateAsync()
    {
       await SwitchWavesAsync();
    }

        void Update()
    {

        float time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
           
                position.y = waveFunc(position.x,time);
                
                point.localPosition = position;
        }
    }

    async Task SwitchWavesAsync()
    {
        int i = 0;
        while (true) {
            await Task.Delay(5000);
            waveFunc = waveFunctions.ElementAt(i % waveFunctions.Count);
            i++;
        }
        
    }



}
