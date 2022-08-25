namespace MathGen.Double.Operators
{
	internal class Mul : BinaryOperator
	{
		public Mul(IFunctionNode a, IFunctionNode b):base(a, b)
		{ }

		public override OperationPriority GetPriority() => OperationPriority.Multiplication;


		public override double GetValue(double[] functionArgs)
		{
			return A.GetValue(functionArgs) * B.GetValue(functionArgs);
		}


		public override bool IsZero()
		{
			return A.IsZero() || B.IsZero();
		}


		public override bool Equals(object obj)
		{
			if (obj is Mul mul)
			{
				return (mul.A.Equals(A) && mul.B.Equals(B))
						|| (mul.A.Equals(B) && mul.B.Equals(A));
			}
			return false;
		}
	}
}
