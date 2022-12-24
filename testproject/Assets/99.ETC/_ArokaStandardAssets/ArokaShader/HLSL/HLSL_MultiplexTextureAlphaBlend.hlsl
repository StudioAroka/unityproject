
void MulT_float(float4 BaseColor, 
	float4 v0, 
	float4 v1, 
	float4 v2, 
	float4 v3, 
	float4 v4, 
	float4 v5, 
	float4 v6,
	float4 v7,
	float4 v8,
	float4 v9,
	float4 v10,
	float4 v11,
	float4 v12,
	out float3 Out)
{
	Out =
		BaseColor.rgb *
		(1 - v0.a) *
		(1 - v1.a) *
		(1 - v2.a) *
		(1 - v3.a) *
		(1 - v4.a) *
		(1 - v5.a) *
		(1 - v6.a) *
		(1 - v7.a) *
		(1 - v8.a) *
		(1 - v9.a) *
		(1 - v10.a) *
		(1 - v11.a) *
		(1 - v12.a)

		+ v0.rgb * (1 - v1.a) * (1 - v2.a) * (1 - v3.a) * (1 - v4.a) * (1 - v5.a) * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v1.rgb * (1 - v2.a) * (1 - v3.a) * (1 - v4.a) * (1 - v5.a) * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v2.rgb * (1 - v3.a) * (1 - v4.a) * (1 - v5.a) * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v3.rgb * (1 - v4.a) * (1 - v5.a) * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v4.rgb * (1 - v5.a) * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v5.rgb * (1 - v6.a) * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v6.rgb * (1 - v7.a) * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v7.rgb * (1 - v8.a) * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v8.rgb * (1 - v9.a) * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v9.rgb * (1 - v10.a) * (1 - v11.a) * (1 - v12.a)
		+ v10.rgb * (1 - v11.a) * (1 - v12.a)
		+ v11.rgb * (1 - v12.a)
		+ v12.rgb;
}