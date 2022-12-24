


void GetAdvancedMainLight_half(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{

#ifdef SHADERGRAPH_PREVIEW

	Direction = float3(0.7, 0.5, 0);

	Color = 1;

	DistanceAtten = 1;

	ShadowAtten = 1;


#else

#if SHADOWS_SCREEN
	float4 clipPos = TransformWorldToHClip(WorldPos);
	float4 shadowCoord = ComputeScreenPos(clipPos);
#else
	float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif

	Light mainLight = GetMainLight(shadowCoord);

	Direction = mainLight.direction;

	Color = mainLight.color;

	DistanceAtten = mainLight.distanceAttenuation;
	//DistanceAtten = unity_LightData.z;

	ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();


	float4 shadowParams = GetMainLightShadowParams();

	float shadowAttenSoft = SampleShadowmapFiltered(TEXTURE2D_SHADOW_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData);

	float shadowAttenHard = SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, false);

	//ShadowAtten = shadowAttenSoft;
	ShadowAtten = shadowAttenHard;

#endif



}
