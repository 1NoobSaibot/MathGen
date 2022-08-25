namespace MathGen.Double
{
	internal interface IOperator : IFunctionNode
	{
		OperationPriority GetPriority();
	}


	internal enum OperationPriority
	{
		Plus = 1,
		Minus = 1,
		Multiplication = 2,
		Division = 2
	}
}
