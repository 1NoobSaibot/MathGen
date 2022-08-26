namespace MathGen.Double.Operators
{
	internal class Sub : BinaryOperator
	{
		public Sub(IFunctionNode a, IFunctionNode b):base(a, b)
		{ }

		public override OperationPriority GetPriority() => OperationPriority.Minus;


		public override double GetValue(double[] functionArgs)
		{
			return A.GetValue(functionArgs) - B.GetValue(functionArgs);
		}


		public override bool IsZero()
		{
			return A.Equals(B) || (A.IsZero() && B.IsZero());
		}


		public override bool Equals(object obj)
		{
			if (obj is Sub sub)
			{
				return A.Equals(sub.A) && B.Equals(sub.B);
			}
			return false;
		}


		public override string ToString()
		{
			string strA = A.ToString();
			string strB = BToString();
			return strA + " - " + strB;
		}


		public override IFunctionNode Clone()
		{
			return new Sub(A.Clone(), B.Clone());
		}


		private string BToString()
		{
			if (B is IOperator op && op.GetPriority() <= GetPriority())
			{
				return "( " + B.ToString() + " )";
			}
			return B.ToString();
		}
	}
}
