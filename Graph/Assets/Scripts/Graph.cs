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
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;
using UnityEngine.Profiling;
using System.Data;


public class Graph : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    Transform pointPrefab;

    [Header("Settings")]
    [SerializeField, Range(1, 100)]
    private int resolution = 30;
    [SerializeField]
    AnimationToggle toggle;


    [SerializeField] private FunctionLibrary.FunctionName _function;
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

    private float step;

    Transform[] points;


    FunctionLibrary.Function waveFunction;


    void Awake()
    {

        UpdateWaveFunction();

        step = 2f / resolution;
        var scale = Vector3.one * step;

        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);

            Rotate rotateScript = point.gameObject.AddComponent<Rotate>();
            rotateScript.SetToggle(toggle);

        }


    }
    private void OnValidate()
    {
        UpdateWaveFunction();
    }

    void UpdateWaveFunction()
    {
        waveFunction = FunctionLibrary.GetFunction(_function);

    }

    void Update()
    {
        Profiler.BeginSample("Update Sample");
        float time = Time.time;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;

            points[i].localPosition = waveFunction(u, v, time);


        }
        Profiler.EndSample();
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
