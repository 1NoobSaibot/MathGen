using MathGen;
using MathGen.Float;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MathGenTest
{
	[TestClass]
	public class FunctionTest
	{
		[TestMethod]
		public void FunctionWorks()
		{
			ArgsDescription args = new ArgsDescription("x");
			Function f = new Function(args, "x * x + 4");

			Assert.AreEqual(4, f[0]);
			Assert.AreEqual(8, f[2]);
			Assert.AreEqual(13, f[-3]);
		}
	}
}
