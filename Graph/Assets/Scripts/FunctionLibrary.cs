using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{

    public delegate Vector3 Function(float u, float v, float t);

    public delegate Vector3 SpinFunction(Vector3 vector, float speed);

    public enum FunctionName { Wave, MultiWave, Ripple, Sphere }
    static Function[] functions = { Wave, MultiWave, Ripple , Sphere };

    public enum SpinFunctionName { SpinObject }
    static SpinFunction[] spinFunctions = { SpinObject };

    public static Function GetFunction(FunctionName functionName) {

        return functions[(int)functionName];
    }
    public static SpinFunction GetSpinFunction(SpinFunctionName functionName)
    {

        return spinFunctions[(int)functionName];
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return SpinObject(p, t);
    }
    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return SpinObject(p, t);
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return SpinObject(p, t);
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        
        float r = Cos(0.5f * PI * v);
        Vector3 p;


        p.x = r * Sin(PI * u);
        p.y = Sin(PI * 0.5f * v);
        p.z = r * Cos(PI * u);

        return SpinObject(p, t);
    }
    public static Vector3 SpinObject(Vector3 vector, float speed) {

        Vector3 returnValue;
        var cosT = Cos(speed);
        var sinT = Sin(speed);
        returnValue.x = cosT * vector.x + sinT * vector.z;
        returnValue.y = vector.y;
        returnValue.z = -sinT * vector.x + cosT * vector.z;
        return returnValue;
    }

}
