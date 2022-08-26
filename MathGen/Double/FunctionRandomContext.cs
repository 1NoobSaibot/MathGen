using MathGen.Double.Operators;
using System;


namespace MathGen.Double
{
	public class FunctionRandomContext
	{
		public readonly ArgsDescription Args;
		public readonly Random rnd;


		public FunctionRandomContext(ArgsDescription args, Random rnd)
		{
			Args = args;
			this.rnd = rnd;
		}


		public Argument GetRandomArgument()
		{
			return Args.CreateArgument(rnd);
		}


		public IFunctionNode CreateRandom(IFunctionNode arg)
		{
			IFunctionNode anotherArg;
			if (arg is Constant)
			{
				anotherArg = GetRandomArgument();
			}
			else
			{
				anotherArg = (rnd.Next() < 0.5)
					? (IFunctionNode) new Constant(rnd.NextDouble())
					: (IFunctionNode) GetRandomArgument();
			}

			switch (rnd.Next(4))
			{
				case 0:
					return new Sub(arg, anotherArg);
				case 1:
					return new Sub(anotherArg, arg);
				case 2:
					return new Mul(arg, anotherArg);
				default:
					return new Sum(arg, anotherArg);
			}
		}
	}
}
