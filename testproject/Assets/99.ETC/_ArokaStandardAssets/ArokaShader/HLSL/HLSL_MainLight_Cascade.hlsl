
void GetMainLightCascade_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0.5);
    Color = float3(1, 1, 1);
    ShadowAtten = 1.0f;
    DistanceAtten = 1;
#else

    //shadow Coord 만들기 
#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half cascadeIndex = ComputeCascadeIndex(WorldPos);
    half4 shadowTypeCascade = mul(_MainLightWorldToShadow[cascadeIndex], float4(WorldPos, 1.0));
    //half4 shadowTypeNoCascade = TransformWorldToShadowCoord(WorldPos);

    //half4 shadowCoord = shadowTypeCascade ;
    
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light light = GetMainLight();
    Direction = light.direction;
    Color = light.color;
    DistanceAtten = light.distanceAttenuation;

    //메인라이트가 없거나 리시브 셰도우 오프가 되어 있을때 
#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
    ShadowAtten = 1.0f;
#endif

    //ShadowAtten 받아와서 만들기 
#if SHADOWS_SCREEN
    ShadowAtten = SampleScreenSpaceShadowmap(shadowCoord);

#else
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half shadowStrength = GetMainLightShadowStrength();
    ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
        sampler_MainLightShadowmapTexture),
        shadowSamplingData, shadowStrength, false);

#endif

#endif

}
