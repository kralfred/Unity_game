using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    ComputeBuffer timeBuff;

    [SerializeField]
    int size;

    private enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

    [SerializeField]
    private FunctionName currentFunction = FunctionName.Wave;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        functionId = Shader.PropertyToID("_functionIndex"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private ComputeBuffer positionsBuffer;
    public ComputeBuffer timeBuffer;
    void UpdateFunctionOnGPU()
    {

       
        float step = (1f + size) / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        computeShader.SetInt(functionId, (int)currentFunction);
        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        int kernelHandle = computeShader.FindKernel("FunctionKernel");
        computeShader.SetBuffer(kernelHandle, "_TimeBuffer", timeBuffer);
        if (kernelHandle < 0)
        {
            Debug.LogError("Failed to find kernel");
            return;
        }
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
        // Initialize buffers in Awake
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
        timeBuffer = new ComputeBuffer(1, sizeof(float));  // Single float buffer

        UpdateFunctionOnGPU();
    }

    float duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    void OnEnable()
    {
        timeBuffer = new ComputeBuffer(1, sizeof(float));
        float time = Time.time;
        timeBuffer.SetData(new[] { time });



        // Check compute shader support
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("Compute shaders not supported!");
            enabled = false;
            return;
        }

        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);

        // Verify kernel exists
        if (computeShader != null && !computeShader.HasKernel("FunctionKernel"))
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
    void UpdateTime() { 
            float time = Time.time;
        timeBuffer.SetData(new[] { time });
    }

    void Update() {
        UpdateFunctionOnGPU();
        UpdateTime();
    }




}
