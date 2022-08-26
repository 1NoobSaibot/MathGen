using MathGen.Double.Operators;
using System;


namespace MathGen.Double.Text
{
	internal class MathSymbol
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


		internal IFunctionNode ToOperator(IFunctionNode a, IFunctionNode b)
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
