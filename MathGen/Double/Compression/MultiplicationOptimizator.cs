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
      ClearZeros(allMultiplications);
			ReducedNode reduced = new ReducedNode(allMultiplications);
      return reduced.ToFunctionNode();
		}


		private void ClearZeros(List<Multiplication> muls)
		{
			for (int i = 0; i < muls.Count; i++)
			{
        if (Math.Abs(muls[i].Scalar) < 1E-14)
				{
          muls.RemoveAt(i);
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
  }
}
