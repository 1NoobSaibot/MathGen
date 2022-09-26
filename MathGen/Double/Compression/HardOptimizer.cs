using MathGen.Double.Operators;
using System;

namespace MathGen.Double.Compression
{
	public class HardOptimizer : Optimizer
	{
		private DateTime _StartMoment;
		private TimeSpan _Limit;

		private readonly static Rule[] HardRules = new Rule[]
		{
			new Rule()
				.Where(op => op is BinaryOperator)
				.Replace(op =>
				{
					IFunctionNode newRoot = MultiplicationOptimizator.OptimizeTree(op);
					if (op.GetAmountOfNodes() <= newRoot.GetAmountOfNodes())
					{
						return op;
					}
					return newRoot;
				})
		};


		public HardOptimizer(TimeSpan timeLimit) : base(HardRules) {
			_Limit = timeLimit;
		}


		public override Function Optimize(Function f)
		{
			_StartMoment = DateTime.Now;
			IFunctionNode newRoot;

			try
			{
				newRoot = _OptimizeTree(f.Root, new Random());
			}
			catch (TimeoutException)
			{
				return f;
			}

			return new Function(f.RndContext, newRoot);
		}


		private IFunctionNode _OptimizeTree(IFunctionNode root, Random rnd)
		{
			if (DateTime.Now - _StartMoment > _Limit)
			{
				throw new TimeoutException();
			}

			if (root is BinaryOperator b)
			{
				if (rnd.NextDouble() < 0.5)
				{
					b.A = _OptimizeTree(b.A, rnd);
					b.B = _OptimizeTree(b.B, rnd);
				}
				else
				{
					b.B = _OptimizeTree(b.B, rnd);
					b.A = _OptimizeTree(b.A, rnd);
				}
			}

			for (int i = 0; i < _rules.Length; i++)
			{
				root = _rules[i].Optimize(root);
			}

			return root;
		}
	}
}
