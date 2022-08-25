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


		public double GetValue(double[] functionArgs)
		{
			return functionArgs[index];
		}


		public bool IsZero()
		{
			return false;
		}


		public override string ToString()
		{
			return name;
		}
	}
}
