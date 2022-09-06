using MathGen.Double.Operators;


namespace MathGen.Double.Compression
{
	public class Optimizer
	{
		protected Rule[] _rules { get; private set; }


		public Optimizer(Rule[] rules)
		{
			_rules = rules;
		}


		public virtual Function Optimize(Function f)
		{
			IFunctionNode newRoot = _OptimizeTree(f.Root);
			return new Function(f.RndContext, newRoot);
		}


		private IFunctionNode _OptimizeTree(IFunctionNode root) {
			if (root is BinaryOperator b)
			{
				b.A = _OptimizeTree(b.A);
				b.B = _OptimizeTree(b.B);
			}

			for (int i = 0; i < _rules.Length; i++)
			{
				root = _rules[i].Optimize(root);
			}

			return root;
		}
	}
}
