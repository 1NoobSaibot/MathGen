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


		public override string ToString()
		{
			string strA = AToString();
			string strB = BToString();
			return strA + " * " + strB;
		}


		public override IFunctionNode Clone()
		{
			return new Mul(A.Clone(), B.Clone());
		}


		private string AToString()
		{
			if (A is IOperator op && op.GetPriority() < GetPriority())
			{
				return "( " + A.ToString() + " )";
			}
			return A.ToString();
		}


		private string BToString()
		{
			if (B is IOperator op)
			{
				if ((op.GetPriority() < GetPriority()) || (op.GetPriority() == GetPriority() && !(op is Mul)))
				{
					return "( " + B.ToString() + " )";
				}
			}
			return B.ToString();
		}


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
