namespace MathGen.Float
{
	internal class Constant : IFunctionNode
	{
		public double Value;


		public Constant(double value)
		{
			Value = value;
		}


		public double GetValue(double[] functionArgs)
		{
			return this.Value;
		}


		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
