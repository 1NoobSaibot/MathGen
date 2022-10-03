using MathGen.Double;
using MathGen.Double.Compression;
using MathGen.Double.Compression.Std;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MathGenTest
{
	[TestClass]
	public class SimilarReductionTest
	{
		Optimizer optimizer = new Optimizer(new Rule[1]
		{
			new SimilarReductionRule()
		});


		[TestMethod]
		public void Level_Infinity()
		{
			string originalExpression = "s * ( s + yz ) * ( 0,844326766323846 + -0,0271747846169783 * ( n - ( 1,4287128877013 + 2,7957262618556 * ( n - xy - xy * 0,634973513284935 ) ) * xq * c - xq * ( xq + zq - n ) + n + s + ( ( 2,00097182937421 + xy ) * ( 2,72823081505385 + ( 0,30264451175868 + xq + c ) * ( c + s - 2,23063758736094 * ( zq - s ) * n - xq * 0,331811316949894 ) + ( 2,02973149585472 * ( n + s - xy * n ) - 0,53554156578837 ) * xq + c - yz ) - ( xq + xy ) + n ) * ( n - s * n * ( 0,0584830651902247 + n + c - xq - 1,98344054360079 * s ) ) - zq * ( 4,70920376372091 * s + -1,00213792441996 * ( ( xq * s + 2,07302951807892 * ( n - xq ) * zq * n + ( 2,78896428146689 + n ) * ( 0,199924932554426 + 0,105584992507686 * ( zq * c * ( n + zq ) - yq - n * 2,09097008489253 ) - n - 0,356054682889698 ) * yq ) * n + n * ( n * ( 2,52448162890456 + -1,01492445019347 * ( zq * n - c ) ) + yq ) * xq - ( c * ( 0,70510159499692 + -1,00014196815159 * ( zq + yq + zq + zq * n + xq * zq - n + 2,34365823235381 * ( n * yq + c + c ) * n ) ) + ( 3,01162223829725 + xq ) * ( c * ( 1,89664090746623 + yq * 0,818601627640113 ) - 2,08281904875657 ) * ( 0,074357083224775 + xq - 1,20001142375732 * yq ) ) - yq * ( n + -0,999854287100262 * ( xq * ( 0,874440268080747 + xq ) - ( 7,99114000345699 * ( 2,02393979328449 * n * n - c ) + n + yq + c ) ) ) ) ) - ( c + xy - yz ) * ( 2,40065797404522 + s + xq ) + n + s * yz - ( s + 2,76055526863125 * ( n - zq ) * yq * zq - yz + ( n * n + ( xz * 2,29673144363359 + xy ) * 4,70059237043608 ) * xz - ( ( xy - ( xz + yz ) * 2,1261483470087 ) * ( xy + xz * ( 2,44167693374845 + n ) + ( xy - c ) * ( n + xq * ( 0,360890693986019 + xz * n ) - ( c + xz * 1,76703936170984 - s ) - -0,614336968339011 ) - ( 0,708630540254896 + -0,99989273413736 * n * c ) * ( n - c ) * ( 1,10182723631509 + -0,999975183373175 * ( xz + 4,87306658600689 * n * n * xy ) ) ) - 0,171981266372955 * ( ( xz - s * s - ( yz + xq + yz ) - 0,417051578599731 ) * ( 0,889919455247249 + n + yz + n * ( yz + c + 2,62453623135351 * yz + yz ) - -0,174373227696027 ) - 2,08948192243778 * zq + yz ) + n + n * c * ( 1,24246692381305 + yq - xy * ( 2,1886083137361 + 2,4290656576368 * n * xz + n ) ) + n ) - ( yq * ( 7,05236691189784 + s - n ) * yq - xy + xq ) ) ) - 0,0629160890331812 )";
			ArgsDescription args = new ArgsDescription("c", "s", "n", "xy", "xz", "xq", "yz", "yq", "zq");
			FunctionRandomContext rndCtx = new FunctionRandomContext(args, new Random());
			Function fOriginal = new Function(rndCtx, originalExpression);

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
				Assert.AreEqual(439, fOptimized.AmountOfNodes);
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
