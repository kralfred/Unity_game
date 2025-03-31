#ifndef TEST_FUNCTION_LIBRARY_HLSL
#define TEST_FUNCTION_LIBRARY_HLSL

#define PI 3.14159265358979323846

float3 Wave(float u, float v, float t)
{
    return float3(u, sin(PI * (u + v + t)), v);
}

float3 MultiWave(float u, float v, float t)
{
    float3 p;
    p.x = u;
    p.y = sin(PI * (u + 0.5 * t));
    p.y += 0.5 * sin(2 * PI * (v + t));
    p.y += sin(PI * (u + v + 0.25 * t));
    p.y *= 1 / 2.5;
    p.z = v;
    return p;
}

float3 Ripple(float u, float v, float t)
{
    float d = sqrt(u * u + v * v);
    float3 p;
    p.x = u;
    p.y = sin(PI * (4 * d - t));
    p.y /= 1 + 10 * d;
    p.z = v;
    return p;
}

float3 Sphere(float u, float v, float t)
{
    float r = 0.9 + 0.1 * sin(PI * (6 * u + 4 * v + t));
    float s = r * cos(0.5 * PI * v);
    return float3(
        s * sin(PI * u),
        r * sin(0.5 * PI * v),
        s * cos(PI * u)
    );
}

float3 Torus(float u, float v, float t)
{
    float r1 = 0.7 + 0.1 * sin(PI * (6 * u + 0.5 * t));
    float r2 = 0.15 + 0.05 * sin(PI * (8 * u + 4 * v + 2 * t));
    float s = r1 + r2 * cos(PI * v);
    return float3(
        s * sin(PI * u),
        r2 * sin(PI * v),
        s * cos(PI * u)
    );
}

#endif