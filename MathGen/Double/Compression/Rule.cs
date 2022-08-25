using MathGen.Double.Operators;


namespace MathGen.Double.Compression
{
	public class Rule
	{
		private WhereDelegate _tester;
		private ReplaceDelegate _rebuilder;


		public IFunctionNode Optimize(IFunctionNode baseExpression)
		{
			return _tester(baseExpression)
				? _rebuilder(baseExpression)
				: baseExpression;
		}


		public Rule Where(WhereDelegate tester)
		{
			this._tester = tester;
			return this;
		}


		public Rule Replace(ReplaceDelegate rebuilder)
		{
			this._rebuilder = rebuilder;
			return this;
		}
	}
}
