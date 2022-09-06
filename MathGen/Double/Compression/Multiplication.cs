using MathGen.Double.Operators;
using System;
using System.Collections.Generic;

namespace MathGen.Double.Compression
{
	internal class Multiplication
	{
		public double Scalar = 1;
		internal List<ArgumentPower> arguments { get; private set; } = new List<ArgumentPower>();


		public Multiplication(double scalar)
		{
			Scalar = scalar;
		}


		public Multiplication(Argument argument)
		{
			arguments.Add(new ArgumentPower(argument));
		}


		internal bool IsConstant()
		{
			return arguments.Count == 0;
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


		public bool Contains(Argument arg)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				if (arguments[i].Arg.Equals(arg))
				{
					return true;
				}
			}

			return false;
		}


		internal Multiplication DivideBy(Multiplication divider)
		{
			Multiplication res = new Multiplication(Scalar / divider.Scalar);

			for (int i = 0; i < arguments.Count; i++)
			{
				ArgumentPower arg = arguments[i];
				int divPow = divider.GetPowerOfArgument(arg.Arg);
				if (arg.Power - divPow > 0)
				{
					res.Mul(new ArgumentPower(arg.Arg, arg.Power - divPow));
				}
			}

			return res;
		}


		internal Multiplication GetCommonWith(Multiplication other)
		{
			Multiplication common = new Multiplication(1);

			for (int i = 0; i < arguments.Count; i++)
			{
				ArgumentPower arg = arguments[i];
				int power = other.GetPowerOfArgument(arg.Arg);
				power = Math.Min(arg.Power, power);

				if (power > 0)
				{
					common.Mul(new ArgumentPower(arg.Arg, power));
				}
			}

			return common;
		}


		private int GetPowerOfArgument(Argument arg)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				if (arguments[i].Arg.Equals(arg))
				{
					return arguments[i].Power;
				}
			}

			return 0;
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

			if (Scalar == 1)
			{
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


		internal class ArgumentPower
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
