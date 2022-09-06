using MathGen.Double.Operators;
using System.Collections.Generic;

namespace MathGen.Double.Compression
{
	internal class ReducedNode
	{
		private Multiplication _common;
		private ReducedNode[] _children;


		public ReducedNode(List<Multiplication> muls)
		{
			if (muls.Count == 1)
			{
				_common = muls[0];
				return;
			}

			_common = GetCommonMultiplier(muls);
			if (_common.IsConstant() == false)
			{
				ReduceSum(muls, _common);
			}

			_children = SplitListIntoNodes(muls);
		}


		private Multiplication GetCommonMultiplier(List<Multiplication> sum)
		{
			Multiplication common = sum[0];
			for (int i = 1; i < sum.Count; i++)
			{
				common = sum[i].GetCommonWith(common);
			}

			return common;
		}


		internal IFunctionNode ToFunctionNode()
		{
			if (_children == null)
			{
				return _common.ToFunctionNode();
			}

			IFunctionNode childA = _children[0].ToFunctionNode();
			IFunctionNode childB = _children[1].ToFunctionNode();
			IFunctionNode sum;
			if (childA is Constant constA && childB is Constant constB)
			{
				sum = new Constant(constA.Value + constB.Value);
			}
			else
			{
				sum = new Sum(childA, childB);
			}
			
			// It means that common part is 1 with no arguments.
			if (_common.IsConstant())
			{
				return sum;
			}

			return new Mul(_common.ToFunctionNode(), sum);
		}


		private void ReduceSum(List<Multiplication> muls, Multiplication common)
		{
			for (int i = 0; i < muls.Count; i++)
			{
				muls[i] = muls[i].DivideBy(common);
			}
		}


		private ReducedNode[] SplitListIntoNodes(List<Multiplication> muls)
		{
			Argument mostCommon = GetMostCommon(muls);
			// Separate All muls which contain the common part.

			List<Multiplication> contains = new List<Multiplication>();
			List<Multiplication> notContains = new List<Multiplication>();

			for (int i = 0; i < muls.Count; i++)
			{
				if (muls[i].Contains(mostCommon))
				{
					contains.Add(muls[i]);
				}
				else
				{
					notContains.Add(muls[i]);
				}
			}

			ReducedNode[] nodes = new ReducedNode[2];
			nodes[0] = new ReducedNode(notContains);
			nodes[1] = new ReducedNode(contains);

			return nodes;
		}


		private Argument GetMostCommon(List<Multiplication> muls)
		{
			List<(Argument arg, int counter)> argCounters = new List<(Argument arg, int counter)>();

			for (int i = 0; i < muls.Count; i++)
			{
				Multiplication mul = muls[i];
				for (int j = 0; j < mul.arguments.Count; j++)
				{
					Argument arg = mul.arguments[j].Arg;
					CountArgument(argCounters, arg);
				}
			}

			int maxCount = argCounters[0].counter;
			Argument mostCommonArgument = argCounters[0].arg;
			for (int i = 1; i < argCounters.Count; i++)
			{
				if (maxCount < argCounters[i].counter)
				{
					maxCount = argCounters[i].counter;
					mostCommonArgument = argCounters[i].arg;
				}
			}

			return mostCommonArgument;
		}


		private void CountArgument(List<(Argument arg, int counter)> argCounters, Argument arg)
		{
			for (int i = 0; i < argCounters.Count; i++)
			{
				if (argCounters[i].arg.Equals(arg))
				{
					argCounters[i] = (arg, argCounters[i].counter + 1);
					return;
				}
			}

			argCounters.Add((arg, 1));
		}
	}
}
