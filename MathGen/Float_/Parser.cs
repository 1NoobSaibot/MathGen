using MathGen.Float;
using System;
using System.Collections.Generic;

namespace MathGen
{
	internal class Parser
	{
		private ArgsDescription _args;
		private readonly Stack<MathSymbol> operators = new Stack<MathSymbol>();
		private readonly Stack<IFunctionNode> operands = new Stack<IFunctionNode>();


		public Parser(ArgsDescription args)
		{
			this._args = args;
		}


		internal static IFunctionNode Parse(ArgsDescription args, string expression)
		{
			Parser parser = new Parser(args);
			return parser.Parse(expression);
		}


		/// <summary>
		/// This function requires and expression where every symbol is separated from each other by space-symbol
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private IFunctionNode Parse(string expression)
		{
			string[] symbols = expression.Split(' ');

			// Parsing every symbol and filling up stacks;
			for (int i = 0; i < symbols.Length; i++)
			{
				MathSymbol s = new MathSymbol(symbols[i]);

				if (s.IsOperator)
				{
					PushNewOperator(s);
				}
				else
				{
					operands.Push(ReadArgumentOrConstant(s.OriginalString));
				}
			}

			// Extract operators and build them
			while (operators.Count > 0)
			{
				PopOperator();
			};

			// Last operand should be a root-node of the function
			return operands.Pop();
		}


		private void PushNewOperator(MathSymbol newOperator)
		{
			if (newOperator.OriginalString == "(")
			{
				operators.Push(newOperator);
				return;
			}

			if (newOperator.OriginalString == ")")
			{
				while (operators.Peek().OriginalString != "(")
				{
					PopOperator();
				};
				operators.Pop();
				return;
			}

			while ((operators.Count > 0) && (newOperator.Priority <= operators.Peek().Priority))
			{
				PopOperator();
			}
			operators.Push(newOperator);
		}


		private void PopOperator()
		{
			MathSymbol prevOperator = operators.Pop();
			IFunctionNode b = operands.Pop();
			IFunctionNode a = operands.Pop();
			IOperator res = prevOperator.ToOperator(a, b);
			operands.Push(res);
		}


		private IFunctionNode ReadArgumentOrConstant(string symbol)
		{
			try
			{
				return new Constant(Double.Parse(symbol));
			}
			catch (Exception)
			{ }

			return _args.CreateArgument(symbol);
		}


		private class MathSymbol
		{
			public readonly string OriginalString;
			public readonly bool IsOperator;
			public readonly int Priority;


			public MathSymbol(string str)
			{
				OriginalString = str;

				if (str == ")")
				{
					IsOperator = true;
					Priority = 0;
				}
				else if (str == "+" || str == "-")
				{
					IsOperator = true;
					Priority = 1;
				}
				else if (str == "*")
				{
					IsOperator = true;
					Priority = 2;
				}
				else if (str == "(")
				{
					IsOperator = true;
					Priority = 0;
				}
			}


			internal IOperator ToOperator(IFunctionNode a, IFunctionNode b)
			{
				switch (OriginalString[0])
				{
					case '+': return new Sum(a, b);
					case '-': return new Sub(a, b);
					case '*': return new Mul(a, b);
				}
				throw new Exception("Invalid type of Operator: Original string = " + OriginalString);
			}
		}
	}
}
