using MathGen.Double;
using MathGen.Double.Compression;
using MathGen.Double.Compression.Std;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MathGenTest
{
	[TestClass]
	public class StandartOptimizerTest
	{
		Optimizer optimizer = new StandartOptimizer();


		[TestMethod]
		public void Level_Infinity()
		{
			Function fOriginal = HardFunctionExamples.GetFunction();

			Function fOptimized = optimizer.Optimize(fOriginal.Clone());

			double maxError = 0;
			for (int alpha = -90; alpha <= 180; alpha += 90)
			{
				double c = Math.Cos(alpha);
				double s = Math.Sin(alpha);
				double n = 1 - c;

				for (int xy = -1; xy <= 1; xy++)
				{
					for (int xz = -1; xz <= 1; xz++)
					{
						for (int xq = -1; xq <= 1; xq++)
						{
							for (int yz = -1; yz <= 1; yz++)
							{
								for (int yq = -1; yq <= 1; yq++)
								{
									for (int zq = -1; zq <= 1; zq++)
									{
										maxError = Math.Max(maxError, Math.Abs(fOriginal[c, s, n, xy, xz, xq, yz, yq, zq] - fOptimized[c, s, n, xy, xz, xq, yz, yq, zq]));
									}
								}
							}
						}
					}
				}

				Assert.AreEqual(457, fOriginal.AmountOfNodes);
				Assert.AreEqual(437, fOptimized.AmountOfNodes);
				AssertAreLessThan(maxError, 1.0E-13);
			}
		}


		private void AssertAreLessThan(double value, double limit)
		{
			if (value > limit)
			{
				throw new Exception("Value " + value + " is not less than " + limit);
			}
		}


		private void ApproximatelyEqual(double a, double b)
		{
			if (Math.Abs(a - b) > 1.0E-13)
			{
				Assert.AreEqual(a, b);
			}
		}
	}
}
