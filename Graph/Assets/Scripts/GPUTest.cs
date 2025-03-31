using UnityEngine;
using System.Collections;

public class GPUTest : MonoBehaviour
{
    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }
    public enum TransitionMode { Cycle, Random }

    [Header("Settings")]
    [SerializeField] private int resolution = 10;
    [SerializeField] private float size = 1f;
    [SerializeField] private TransitionMode transitionMode = TransitionMode.Cycle;
    [SerializeField, Min(0f)] private float functionDuration = 1f;
    [SerializeField, Min(0f)] private float transitionDuration = 1f;

    [Header("References")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;

    // Shader property IDs
    private static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        functionId = Shader.PropertyToID("_functionIndex"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    // Buffers
    private ComputeBuffer positionsBuffer;
    private ComputeBuffer timeBuffer;

    // State
    public FunctionName currentFunction;
    private bool transitioning;
    private bool parametersChanged;
    private float transitionTimer;

    void Awake()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
        timeBuffer = new ComputeBuffer(1, sizeof(float));
    }

    void OnEnable()
    {
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("Compute shaders not supported!");
            enabled = false;
            return;
        }

        UpdateAllParameters();
    }

    void Update()
    {
        UpdateTime(); // Runs every frame

        if (parametersChanged || transitioning)
        {
            UpdateFunctionParameters();
            parametersChanged = false;
        }

        DrawInstanced();
    }

    void UpdateTime()
    {
        if (timeBuffer == null) return; // Safety check

        float time = Time.time;
        timeBuffer.SetData(new[] { time });
        computeShader.SetFloat(timeId, time);
        material.SetFloat(timeId, time);
    }

    void UpdateFunctionParameters()
    {
        float step = (1f + size) / resolution;

        // Core parameters
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetInt(functionId, (int)currentFunction);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        // Material updates
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        DispatchComputeShader();
    }

    void UpdateAllParameters()
    {
        UpdateTime();
        UpdateFunctionParameters();
    }

    void DispatchComputeShader()
    {
        int kernel = computeShader.FindKernel("FunctionKernel");
        if (kernel < 0)
        {
            Debug.LogError("Kernel not found");
            return;
        }

        computeShader.SetBuffer(kernel, "_TimeBuffer", timeBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernel, groups, groups, 1);
    }

    void DrawInstanced()
    {
        Graphics.DrawMeshInstancedProcedural(
            mesh, 0, material,
            new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution)),
            resolution * resolution
        );
    }

    public void ChangeFunction(FunctionName newFunction, bool immediate = false)
    {
        if (immediate)
        {
            currentFunction = newFunction;
            parametersChanged = true;
        }
        else
        {
            StartCoroutine(TransitionToFunction(newFunction));
        }
    }

    IEnumerator TransitionToFunction(FunctionName targetFunction)
    {
        transitioning = true;
        transitionTimer = 0f;

        FunctionName startFunction = currentFunction;
        computeShader.SetInt("_FunctionFrom", (int)startFunction);
        computeShader.SetInt("_FunctionTo", (int)targetFunction);

        while (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, transitionTimer / transitionDuration);

            computeShader.SetFloat(transitionProgressId, progress);
            material.SetFloat(transitionProgressId, progress);
            parametersChanged = true;

            yield return null;
        }

        currentFunction = targetFunction;
        transitioning = false;
        parametersChanged = true;
    }

    void OnDisable()
    {
        positionsBuffer?.Release();
        timeBuffer?.Release();
    }

    void OnValidate()
    {
        if (Application.isPlaying && enabled)
        {
            parametersChanged = true;
        }
    }
}
