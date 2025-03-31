using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static FunctionLibrary;

public class GPUGraph : MonoBehaviour
{
    [SerializeField, Range(10, 200)]
    int resolution = 10;


    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField, Min(0f)]
    ComputeBuffer timeBuff;

    [SerializeField, Range(0, 10)]  // Now size is clamped between 0 and 10
    int size = 1;

    private enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

    [SerializeField]
    private FunctionName currentFunction;
    private FunctionName previousFunction;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    private int kernelHandle;
    private int morphKernel;
    private float transitionProgress;

    static readonly int
       positionsId = Shader.PropertyToID("_Positions"),
       resolutionId = Shader.PropertyToID("_Resolution"),
       stepId = Shader.PropertyToID("_Step"),
       timeId = Shader.PropertyToID("_Time"),
       functionId = Shader.PropertyToID("_functionIndex"),
       functionFrom = Shader.PropertyToID("_FunctionFrom"),
       transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private ComputeBuffer positionsBuffer;
    public ComputeBuffer timeBuffer;
    void UpdateFunctionOnGPU()
    {


        float step = (1f + size) / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(kernelHandle, positionsId, positionsBuffer);
        computeShader.SetInt(functionId, (int)currentFunction);



        computeShader.SetBuffer(kernelHandle, "_TimeBuffer", timeBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelHandle, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));

        Graphics.DrawMeshInstancedProcedural(
            mesh, 0, material, bounds, positionsBuffer.count
        );
    }


    void Awake()
    {

        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
        timeBuffer = new ComputeBuffer(1, sizeof(float)); 
        computeShader.SetInt(functionId, (int)currentFunction);
        kernelHandle = computeShader.FindKernel("FunctionKernel");
        morphKernel = computeShader.FindKernel("MorphKernel");
        UpdateFunctionOnGPU();
    }



    private void StartTransition(FunctionName from, FunctionName to)
    {


        transitionProgress = 0f;
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {



        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {

            transitionProgress = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            computeShader.SetBuffer(morphKernel, "_TimeBuffer", timeBuffer);
            computeShader.SetBuffer(morphKernel, positionsId, positionsBuffer);
            computeShader.SetFloat(transitionProgressId, transitionProgress);

            kernelHandle = computeShader.FindKernel("MorphKernel");
            elapsed += Time.deltaTime;
            UpdateFunctionOnGPU();
            yield return null;

        }
        kernelHandle = computeShader.FindKernel("FunctionKernel");
        transitionProgress = 1f;
    }

    private void OnValidate()
    {


        if (Application.isPlaying)
        {
            size = Mathf.Clamp(size, 0, 10);
            if (currentFunction != previousFunction)
            {


                computeShader.SetInt(functionFrom, (int)previousFunction);
                Debug.Log($"Function changing from {previousFunction} to {currentFunction}");


                FunctionName oldFunction = previousFunction;
                previousFunction = currentFunction;


                StartTransition(oldFunction, currentFunction);
                Debug.Log($"Function changing from {oldFunction}");
            }
        }
    }

    void OnEnable()
    {
        timeBuffer = new ComputeBuffer(1, sizeof(float));
        float time = Time.time;
        timeBuffer.SetData(new[] { time });

        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("Compute shaders not supported!");
            enabled = false;
            return;
        }

        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);

        if (computeShader != null && !computeShader.HasKernel("FunctionKernel"))
        {
            Debug.LogError("Kernel not found in compute shader!");
            enabled = false;
        }
        if (computeShader != null && !computeShader.HasKernel("MorphKernel"))
        {
            Debug.LogError("Kernel not found in compute shader!");
            enabled = false;
        }
    }
    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
        timeBuffer?.Release();
    }
    void UpdateTime()
    {
        float time = Time.time;
        timeBuffer.SetData(new[] { time });
    }

    void Update()
    {
        UpdateFunctionOnGPU();
        UpdateTime();
    }




}
