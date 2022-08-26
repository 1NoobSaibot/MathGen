using System.Collections.Generic;


namespace MathGen.Double.Operators
{
	internal abstract class BinaryOperator : IOperator
	{
		public IFunctionNode A;
		public IFunctionNode B;


		public BinaryOperator(IFunctionNode a, IFunctionNode b)
		{
			A = a;
			B = b;
		}


		public override int GetAmountOfNodes()
		{
			return 1 + A.GetAmountOfNodes() + B.GetAmountOfNodes();
		}


		public override void AddOperatorsToList(List<IFunctionNode> operators)
		{
			operators.Add(this);
			A.AddOperatorsToList(operators);
			B.AddOperatorsToList(operators);
		}



		public override IFunctionNode GetMutatedClone(FunctionRandomContext rndContext)
		{
			switch (rndContext.rnd.Next(3))
			{
				case 0:
					return A;
				case 1:
					return B;
				default:
					return rndContext.CreateRandom(this);
			}
		}
	}
}
