namespace MathGen.Float
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
	}
}
