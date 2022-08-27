using MathGen.Double.Operators;
using MathGen.Double.Text;
using System;
using System.Collections.Generic;

namespace MathGen.Double
{
	public class Function
	{
		private IFunctionNode _root;
		private FunctionRandomContext _context;
		public readonly int AmountOfNodes;


		public Function(FunctionRandomContext context, string expression)
		{
			_context = context;
			_root = Parser.Parse(context.Args, expression);
			AmountOfNodes = _root.GetAmountOfNodes();
		}


		private Function(FunctionRandomContext context, IFunctionNode root)
		{
			_context = context;
			_root = root;
			AmountOfNodes = _root.GetAmountOfNodes();
		}


		public double Calculate(params double[] argValues)
		{
			if (_context.Args.Count != argValues.Length)
			{
				throw new Exception("Amount of arguments doesn't equal to count of its names");
			}

			return _root.GetValue(argValues);
		}


		public override string ToString()
		{
			return _root.ToString();
		}


		public Function Clone()
		{
			return new Function(_context, _root.Clone());
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
			_root.AddOperatorsToList(operators);

			int randomOpIndex = _context.rnd.Next(operators.Count);
			IFunctionNode choosenNode = operators[randomOpIndex];
			IFunctionNode mutatedNode = choosenNode.GetMutatedClone(_context);

			if (choosenNode == _root)
			{
				this._root = mutatedNode;
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
			for (int i = list.Count - 1; i >= 0; i--)
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
