using MathGen.Double.Operators;
using MathGen.Double.Text;
using System;
using System.Collections.Generic;

namespace MathGen.Double
{
	public class Function
	{
		internal IFunctionNode Root { get; private set; }
		internal readonly FunctionRandomContext RndContext;
		public readonly int AmountOfNodes;


		public Function(FunctionRandomContext context, string expression)
		{
			RndContext = context;
			Root = Parser.Parse(context.Args, expression);
			AmountOfNodes = Root.GetAmountOfNodes();
		}


		internal Function(FunctionRandomContext context, IFunctionNode root)
		{
			RndContext = context;
			Root = root;
			AmountOfNodes = Root.GetAmountOfNodes();
		}


		public double Calculate(params double[] argValues)
		{
			if (RndContext.Args.Count != argValues.Length)
			{
				throw new Exception("Amount of arguments doesn't equal to count of its names");
			}

			return Root.GetValue(argValues);
		}


		public override string ToString()
		{
			return Root.ToString();
		}


		public Function Clone()
		{
			return new Function(RndContext, Root.Clone());
		}


		public double this[params double[] args] => Calculate(args);


		public int GetAmountOfNodes()
		{
			return AmountOfNodes;
		}


		public Function GetMutatedClone()
		{
			Function clone = Clone();
			clone.Mutate();
			return clone;
		}


		private void Mutate()
		{
			List<IFunctionNode> operators = new List<IFunctionNode>();
			Root.AddOperatorsToList(operators);

			int randomOpIndex = RndContext.rnd.Next(operators.Count);
			IFunctionNode choosenNode = operators[randomOpIndex];
			IFunctionNode mutatedNode = choosenNode.GetMutatedClone(RndContext);

			if (choosenNode == Root)
			{
				this.Root = mutatedNode;
				return;
			}

			BinaryOperator parent = FindParent(operators, choosenNode) as BinaryOperator;
			if (parent.A == choosenNode)
			{
				parent.A = mutatedNode;
			}
			else
			{
				parent.B = mutatedNode;
			}
		}


		private BinaryOperator FindParent(List<IFunctionNode> list, IFunctionNode children)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is BinaryOperator bin && (bin.A == children || bin.B == children))
				{
					return bin;
				}
			}

			return null;
		}
	}
}
