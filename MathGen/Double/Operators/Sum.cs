﻿namespace MathGen.Double.Operators
{
	internal class Sum : BinaryOperator
	{
		public Sum(IFunctionNode a, IFunctionNode b):base(a, b)
		{ }


		public override OperationPriority GetPriority() => OperationPriority.Plus;


		public override double GetValue(double[] functionArgs)
		{
			return A.GetValue(functionArgs) + B.GetValue(functionArgs);
		}


		public override bool IsZero()
		{
			return A.IsZero() && B.IsZero();
		}


		public override bool Equals(object obj)
		{
			if (obj is Sum sum)
			{
				return (A.Equals(sum.A) && B.Equals(sum.B))
						|| (A.Equals(sum.B) && B.Equals(sum.A));
			}
			return false;
		}


		public override string ToString()
		{
			string strA = A.ToString();
			string strB = B.ToString();
			return strA + " + " + strB;
		}


		public override IFunctionNode Clone()
		{
			return new Sum(A.Clone(), B.Clone());
		}


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
