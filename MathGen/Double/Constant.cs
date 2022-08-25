using System;

namespace MathGen.Double
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


		public bool IsZero()
		{
			return Math.Abs(Value) <= 1E-15;
		}


		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
