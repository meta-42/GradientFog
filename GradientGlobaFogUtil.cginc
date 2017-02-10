#ifndef GRADIENT_FOG_INCLUDED
#define GRADIENT_FOG_INCLUDED 

//place   #pragma multi_compile _ GRADIENT_GLOBAL_FOG  to shader

#if defined(GRADIENT_GLOBAL_FOG)
	#define GRADIENT_GLOBAL_FOG_COORDS(idx)			float2 fogCoord : TEXCOORD##idx;
	#define GRADIENT_GLOBAL_FOG_TRANSFER(o, posOS)	o.fogCoord.xy = CalculateGlobalFogCoords(mul(unity_ObjectToWorld, posOS.xyzw).xyz);
	#define GRADIENT_GLOBAL_FOG_APPLY(inFogCoord, c)		c.rgb = ApplyGlobalFog(c.rgb, inFogCoord.xy);
#else
	#define GRADIENT_GLOBAL_FOG_COORDS(idx)
	#define GRADIENT_GLOBAL_FOG_TRANSFER(o, posOS)
	#define GRADIENT_GLOBAL_FOG_APPLY(inFogCoord, c)
#endif


uniform half depthFogDensity;
uniform half depthFogGradientDelta;
uniform half depthFogGradientStartAdd;
uniform sampler2D depthFogGradientTexture;

uniform half verticalFogDensity;
uniform half verticalFogBaseHight;
uniform sampler2D verticalFogGradientTexture;

half2 CalculateGlobalFogCoords(float3 posWS)
{
	half2 fogCoord = 0.0;
	half scale = distance(posWS, _WorldSpaceCameraPos);
	fogCoord.x = (depthFogGradientDelta * scale  + depthFogGradientStartAdd) * depthFogDensity;
	fogCoord.y = exp(-posWS.y - verticalFogBaseHight) * verticalFogDensity;
	return saturate(fogCoord.xy);
}

half3 ApplyGlobalFog(half3 c, half2 fogCoord)
{
	half4 depthFogColor = tex2D(depthFogGradientTexture, half2( fogCoord.x, 0.0f)).rgba;
	c.rgb = lerp(c.rgb, depthFogColor.rgb, depthFogColor.a);

	half4 verticalFogColor = tex2D(verticalFogGradientTexture, half2( fogCoord.y, 0.0f)).rgba;
	c.rgb = lerp(c.rgb, verticalFogColor.rgb,  fogCoord.y);
	return c.rgb;
}

#endif