
void FloatSUM_float(
	float1 f0, 
	float1 f1, 
	float1 f2, 
	float1 f3, 
	float1 f4, 
	float1 f5,
	float1 f6,
	float1 f7,
	float1 f8,
	float1 f9,
	float1 f10,
	float1 f11,
	float1 f12,
	out float1 Out)
{
	float1 tmp =
		f0 +
		f1 +
		f2 +
		f3 +
		f4 +
		f5 +
		f6 +
		f7 +
		f8 +
		f9 +
		f10 +
		f11 +
		f12;
	Out = tmp > 1 ? 1 : tmp;
}