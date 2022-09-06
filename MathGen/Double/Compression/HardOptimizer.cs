using MathGen.Double.Operators;


namespace MathGen.Double.Compression
{
	public class HardOptimizer : Optimizer
	{
		private readonly static Rule[] HardRules = new Rule[]
		{
			new Rule()
				.Where(op => op is BinaryOperator)
				.Replace(op =>
				{
					IFunctionNode newRoot = MultiplicationOptimizator.OptimizeTree(op);
					if (op.GetAmountOfNodes() < newRoot.GetAmountOfNodes())
					{
						return op;
					}
					return newRoot;
				})
		};


		public HardOptimizer() : base(HardRules) { }
	}
}
