namespace MathGen.Double
{
	internal class Argument : IFunctionNode
	{
		public int index;
		public string name;


		public Argument(int index, string name)
		{
			this.index = index;
			this.name = name;
		}


		public override double GetValue(double[] functionArgs)
		{
			return functionArgs[index];
		}


		public override bool IsZero()
		{
			return false;
		}


		public override string ToString()
		{
			return name;
		}

		public override bool Equals(object obj)
		{
			if (obj is Argument arg)
			{
				return arg.index == index;
			}
			return false;
		}
	}
}
