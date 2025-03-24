using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;


public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    Vector3 position;
    [SerializeField, Range(1, 100)]
    int resolution = 30;

    [SerializeField] 
    Button spin;


    [SerializeField] private FunctionLibrary.FunctionName _function; // Backing field
    public FunctionLibrary.FunctionName Function
    {
        get => _function;
        set
        {
            if (_function != value)
            {
                _function = value;
                UpdateWaveFunction();
            }
        }
    }

    Transform[] points;

    FunctionLibrary.Function waveFunction;

    void Start()
    {
    }

    void Awake()
    {

        UpdateWaveFunction();

        float step = 2f / resolution;
        var scale = Vector3.one * step;

        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {

            Transform point = points[i] = Instantiate(pointPrefab);

            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }
    private void OnValidate()
    {
        UpdateWaveFunction();
    }

    void UpdateWaveFunction() {
        waveFunction = FunctionLibrary.GetFunction(_function);
    }


    void Update()
    {
        
        float time = Time.time;
        float step = 2f / resolution;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            points[i].localPosition =  waveFunction(u, v, time);
            
        }
    }

    async void InitiateAsync()
    {
        await SwitchWavesAsync();
    }

    async Task SwitchWavesAsync()
    {

        while (true)
        {
            foreach (FunctionName funcName in Enum.GetValues(typeof(FunctionName)))
            {
                await Task.Delay(5000);
                waveFunction = FunctionLibrary.GetFunction(funcName);

            }
        }

    }

}
