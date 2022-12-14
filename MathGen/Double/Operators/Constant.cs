using System;


namespace MathGen.Double.Operators
{
	internal class Constant : IFunctionNode
	{
		public double Value;


		public Constant(double value)
		{
			Value = value;
		}


		public override double GetValue(double[] functionArgs)
		{
			return this.Value;
		}


		public override bool IsZero()
		{
			return Math.Abs(Value) <= 1E-15;
		}


		public override string ToString()
		{
			return Value.ToString();
		}


		public override bool Equals(object obj)
		{
			if (obj is Constant constant)
			{
				return constant.Value == this.Value;
			}
			return false;
		}


		public override IFunctionNode Clone()
		{
			return new Constant(Value);
		}


		public override int GetAmountOfNodes()
		{
			return 1;
		}


		public override IFunctionNode GetMutatedClone(FunctionRandomContext ctx)
		{
			switch (ctx.rnd.Next(4))
			{
				case 0:
					return ctx.GetRandomArgument();
				case 1:
					return ctx.CreateRandom(this);
				default:
					return ChangeValue(ctx.rnd);
			}
		}


		private IFunctionNode ChangeValue(Random rnd)
		{
			double amplitude = 2;

			double divByTen_Times = rnd.Next(15);
			double divB = 1;
			for (int i = 0; i < divByTen_Times; i++)
			{
				divB *= 10;
			}

			amplitude /= divB;

			Value += rnd.NextDouble() * amplitude - (amplitude / 2);
			return this;
		}
	}
}
