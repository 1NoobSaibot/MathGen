namespace MathGen.Double.Operators
{
	public abstract class IFunctionNode
	{
		public abstract IFunctionNode Clone();
		public abstract double GetValue(double[] functionArgs);
		public abstract bool IsZero();


		public static IFunctionNode operator +(IFunctionNode a, IFunctionNode b)
		{
			if (a is Constant ca && b is Constant cb)
			{
				return new Constant(ca.Value + cb.Value);
			}
			return new Sum(a, b);
		}


		public static IFunctionNode operator -(IFunctionNode a, IFunctionNode b)
		{
			if (a is Constant ca && b is Constant cb)
			{
				return new Constant(ca.Value - cb.Value);
			}
			return new Sub(a, b);
		}


		public static IFunctionNode operator *(IFunctionNode a, IFunctionNode b)
		{
			if (a is Constant ca && b is Constant cb)
			{
				return new Constant(ca.Value * cb.Value);
			}
			return new Mul(a, b);
		}


		public abstract int GetAmountOfNodes();
	}
}
