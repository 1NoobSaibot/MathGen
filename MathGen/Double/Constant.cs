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


		public override double GetValue(double[] functionArgs)
		{
			return this.Value;
		}


		public override bool IsZero()
		{
			return Math.Abs(Value) <= 1E-15;
		}


		public override string ToString()
		{
			return Value.ToString();
		}


		public override bool Equals(object obj)
		{
			if (obj is Constant constant)
			{
				return constant.Value == this.Value;
			}
			return false;
		}
	}
}
