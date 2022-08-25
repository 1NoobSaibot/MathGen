namespace MathGen.Float
{
	internal abstract class BinaryOperator : IOperator
	{
		public IFunctionNode A;
		public IFunctionNode B;


		public BinaryOperator(IFunctionNode a, IFunctionNode b)
		{
			A = a;
			B = b;
		}

		public abstract OperationPriority GetPriority();

		public abstract double GetValue(double[] functionArgs);
	}
}
