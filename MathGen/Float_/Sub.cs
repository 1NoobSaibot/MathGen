namespace MathGen.Float
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
	}
}
