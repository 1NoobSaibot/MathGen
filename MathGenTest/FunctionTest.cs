﻿using MathGen.Double;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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


		[TestMethod]
		public void TestToString()
		{
			const string originalExpression = "x * x + 4";
			ArgsDescription args = new ArgsDescription("x");
			Function f = new Function(args, originalExpression);

			Assert.AreEqual(originalExpression, f.ToString());
		}
	}
}
