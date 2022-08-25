namespace MathGen.Float
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
	}
}
