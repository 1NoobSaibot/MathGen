using MathGen.Double.Operators;
using MathGen.Double.Text;
using System;


namespace MathGen.Double
{
	public class Function
	{
		private IFunctionNode _root;
		private ArgsDescription _args;


		public Function(ArgsDescription args, string expression)
		{
			_args = args;
			_root = Parser.Parse(args, expression);
		}


		public double Calculate(params double[] argValues)
		{
			if (_args.Count != argValues.Length)
			{
				throw new Exception("Amount of arguments doesn't equal to count of its names");
			}

			return _root.GetValue(argValues);
		}


		public double this[params double[] args] => Calculate(args);
	}
}
