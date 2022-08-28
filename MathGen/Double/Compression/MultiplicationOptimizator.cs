using MathGen.Double.Operators;
using System;
using System.Collections.Generic;

namespace MathGen.Double.Compression
{
  public class MultiplicationOptimizator
  {
    private readonly Function function;
    private MultiplicationOptimizator(Function f)
		{
      IFunctionNode newRoot = Compress(f.Root);
      function = new Function(f.RndContext, newRoot);
		}


    public static Function Optimize(Function f)
		{
      MultiplicationOptimizator optimizator = new MultiplicationOptimizator(f);
      return optimizator.function;
      // return f.Clone();
		}


    private IFunctionNode Compress(IFunctionNode origin)
		{
      List<Multiplication> allMultiplications = Decompress(origin);
      RemoveZeros(allMultiplications);

      IFunctionNode res = allMultiplications[0].ToFunctionNode();
      for (int i = 1; i < allMultiplications.Count; i++)
			{
        Multiplication multiplication = allMultiplications[i];
        if (multiplication.Scalar < 0)
				{
          multiplication.Scalar *= -1;
          res = new Sub(res, multiplication.ToFunctionNode());
				}
        else
				{
          res = new Sum(res, multiplication.ToFunctionNode());
				}
			}

      return res;
		}


		private void RemoveZeros(List<Multiplication> allMultiplications)
		{
			for (int i = 0; i < allMultiplications.Count; i++)
			{
        if (Math.Abs(allMultiplications[i].Scalar) < 1.0E-10)
				{
          allMultiplications.RemoveAt(i);
          i--;
				}
			}
		}


		private List<Multiplication> Decompress(IFunctionNode root)
		{
      List<Multiplication> multiplications = new List<Multiplication>();

      if (root is Constant constant)
			{
        multiplications.Add(new Multiplication(constant.Value));
        return multiplications;
			}

      if (root is Argument argument)
			{
        multiplications.Add(new Multiplication(argument));
        return multiplications;
			}

      if (root is BinaryOperator bin)
			{
        List<Multiplication> fromA = Decompress(bin.A);
        // List fromB will be changed. It will be a result of next operations;
        List<Multiplication> fromB = Decompress(bin.B);

        if (bin is Sum)
				{
          SumAToB(fromA, fromB);
          return fromB;
				}

        if (bin is Sub)
				{
          MakeNegative(fromB);
          SumAToB(fromA, fromB);
          return fromB;
				}

        if (bin is Mul)
				{
          return MulTwoLists(fromA, fromB);
				}
      }

      throw new Exception("Unknown IFunctionNode type");
		}

		
		private void SumAToB(List<Multiplication> a, List<Multiplication> b)
		{
      for (int i = 0; i < a.Count; i++)
			{
        AddTo(a[i], b);
			}
		}


    private void AddTo(Multiplication mul, List<Multiplication> toList)
		{
      for (int j = 0; j < toList.Count; j++)
      {
        if (mul.HasTheSameArgumentPowers(toList[j]))
        {
          toList[j].Scalar += mul.Scalar;
          return;
        }
      }

      toList.Add(mul);
    }


    private void MakeNegative(List<Multiplication> list)
		{
      for (int i = 0; i < list.Count; i++)
			{
        list[i].Scalar *= -1;
			}
		}


    private List<Multiplication> MulTwoLists(List<Multiplication> fromA, List<Multiplication> fromB)
    {
      List<Multiplication> result = new List<Multiplication>();
      
      for (int i = 0; i < fromA.Count; i++)
			{
        for (int j = 0; j < fromB.Count; j++)
				{
          Multiplication res = fromA[i] * fromB[j];
          AddTo(res, result);
				}
			}

      return result;
    }


    private class Multiplication
		{
      public double Scalar = 1;
      private List<ArgumentPower> arguments = new List<ArgumentPower>();


      public Multiplication(double scalar)
			{
        Scalar = scalar;
			}

			public Multiplication(Argument argument)
			{
        arguments.Add(new ArgumentPower(argument));
			}


			internal bool HasTheSameArgumentPowers(Multiplication anotherMul)
			{
				if (arguments.Count != anotherMul.arguments.Count)
				{
          return false;
				}

        for (int i = 0; i < arguments.Count; i++)
				{
          if (anotherMul.HasTheSamePowerOfArgument(arguments[i]) == false)
					{
            return false;
					}
				}

        return true;
			}


			private bool HasTheSamePowerOfArgument(ArgumentPower argumentPower)
			{
        for (int i = 0; i < arguments.Count; i++)
				{
          if (argumentPower.Arg.Equals(arguments[i].Arg) && argumentPower.Power == arguments[i].Power)
					{
            return true;
					}
				}

        return false;
			}


      public static Multiplication operator *(Multiplication a, Multiplication b)
			{
        Multiplication res = new Multiplication(a.Scalar * b.Scalar);

        for (int i = 0; i < a.arguments.Count; i++)
				{
          ArgumentPower argPow = a.arguments[i];
          res.Mul(argPow);
				}
        for (int i = 0; i < b.arguments.Count; i++)
        {
          ArgumentPower argPow = b.arguments[i];
          res.Mul(argPow);
        }

        return res;
      }


			private void Mul(ArgumentPower argPow)
			{
				for (int i = 0; i < arguments.Count; i++)
				{
          if (arguments[i].Arg.Equals(argPow.Arg))
					{
            arguments[i].IncPower(argPow.Power);
            return;
					}
				}

        // It's really matter to clone object here, because you can break calculations for yourself and feel headache
        ArgumentPower copy = new ArgumentPower(argPow.Arg, argPow.Power);
        arguments.Add(copy);
			}


			internal IFunctionNode ToFunctionNode()
			{
        if (arguments.Count == 0)
				{
          return new Constant(Scalar);
				}

        IFunctionNode res = arguments[0].ToFunctionNode();
        for (int i = 1; i < arguments.Count; i++)
				{
          res = new Mul(res, arguments[i].ToFunctionNode());
				}

        if (Math.Abs(Scalar - 1) < 1.0E-10) {
          return res;
        }
        return new Mul(new Constant(Scalar), res);
			}


			public override string ToString()
			{
        string str = Scalar + " ";

        for (int i = 0; i < arguments.Count; i++)
				{
          str += arguments[i].ToString() + " ";
				}

				return str;
			}


			private class ArgumentPower
			{
        public readonly Argument Arg;
        public int Power;

        public ArgumentPower(Argument arg)
				{
          Arg = arg;
          Power = 1;
				}


        public ArgumentPower(Argument arg, int power)
        {
          Arg = arg;
          Power = power;
        }

				internal void IncPower(int power)
				{
          Power += power;
				}

				internal IFunctionNode ToFunctionNode()
				{
					/*if (Power == 1)
					{
            return Arg;
					}*/

          IFunctionNode res = Arg;
          for (int i = 1; i < Power; i++)
					{
            res = new Mul(res, Arg);
					}
          return res;
				}


				public override string ToString()
				{
					return Arg.ToString() + "^" + Power;
				}
			}
		}
  }
}
