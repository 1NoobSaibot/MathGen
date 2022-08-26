using MathGen.Double;
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


		[TestMethod]
		public void Clone()
		{
			const string originalExpression = "x * x + 4";
			ArgsDescription args = new ArgsDescription("x");
			Function f = new Function(args, originalExpression);
			Function clone = f.Clone();

			Assert.AreEqual(4, clone[0]);
			Assert.AreEqual(8, clone[2]);
			Assert.AreEqual(13, clone[-3]);
		}


		[TestMethod]
		public void AmountOfNode()
		{
			const string originalExpression = "x * x + 4";
			ArgsDescription args = new ArgsDescription("x");
			Function f = new Function(args, originalExpression);

			Assert.AreEqual(5, f.GetAmountOfNodes());
		}
	}
}
