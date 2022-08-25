namespace MathGen.Double
{
	public interface IFunctionNode
	{
		double GetValue(double[] functionArgs);
		bool IsZero();
	}
}
