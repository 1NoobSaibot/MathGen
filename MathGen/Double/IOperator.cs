namespace MathGen.Double
{
	internal abstract class IOperator : IFunctionNode
	{
		public abstract OperationPriority GetPriority();
	}


	internal enum OperationPriority
	{
		Plus = 1,
		Minus = 1,
		Multiplication = 2,
		Division = 2
	}
}
