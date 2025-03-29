using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FunctionLibrary;

public class GPUGraph : MonoBehaviour
{
    [SerializeField, Range(10, 200)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    public enum FunctionName { Wave, MultiWave, Ripple, Sphere }

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    private int functionId = 2;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");
        functionId = Shader.PropertyToID("_functionIndex");

    private ComputeBuffer positionsBuffer;

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetInt(functionID, 2);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int kernelHandle = computeShader.FindKernel("FunctionKernel");
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



    float duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    void OnEnable()
    {
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
    }

    void Update() {
        UpdateFunctionOnGPU();

    }

    void PickNextFunction() {
    }

    void Start()
    {
        
    }


}
