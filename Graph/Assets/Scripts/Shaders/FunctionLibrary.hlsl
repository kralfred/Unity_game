// Assets/Shaders/FunctionLibrary.cginc
#ifndef FUNCTION_LIBRARY_CGINC
#define FUNCTION_LIBRARY_CGINC

#define PI 3.14159265358979323846

StructuredBuffer<float> _TimeBuffer;


float3 Wave (float u, float v) {
	float t = _TimeBuffer[0]; 
	float3 p;
	p.x = u;
	p.y = sin(PI * (u + v + t));
	p.z = v;
	return p;
}


float3 MultiWave(float u, float v) {
	float t = _TimeBuffer[0]; 
    float3 p;
    p.x = u;
    p.y = sin(PI * (u + 0.5f * t));  
    float test = 2.0f * PI * (v + t);  
    p.y += 0.5f * sin(test);
    p.y += sin(PI * (u + v + 0.25f * t));
    p.y *= 1.0f / 2.5f; 
    p.z = v;
    return p;
}
	float3 Ripple(float u, float v)
    {
		float t = _TimeBuffer[0];
        float d = sqrt(u * u + v * v);
        float3 p;
        p.x = u;
		float test = PI * (4 * d - t);
        p.y = sin(test);
        p.y /= 1.0f + 10 * d;
        p.z = v;
        return p;
    }
float3 Sphere (float u, float v) {
	float t = _TimeBuffer[0];
	float r = cos(0.5f * PI * v);
	float3 p;
	p.x = r * sin(PI * u);
	p.y = sin(PI * 0.5f * v);
	p.z = r * cos(PI * u);
	return p;
}
float3 Torus (float u, float v) {
	float t = _TimeBuffer[0];
	float r1 = 0.7 + 0.1 * sin(PI * (6.0 * u + 0.5 * t));
	float r2 = 0.15 + 0.05 * sin(PI * (8.0 * u + 4.0 * v + 2.0 * t));
	float s = r2 * cos(PI * v) + r1;
	float3 p;
	p.x = s * sin(PI * u);
	p.y = r2 * sin(PI * v);
	p.z = s * cos(PI * u);
	return p;
}


#endif