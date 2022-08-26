namespace MathGen.Double.Operators
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


		public override int GetAmountOfNodes()
		{
			return 1 + A.GetAmountOfNodes() + B.GetAmountOfNodes();
		}
	}
}
