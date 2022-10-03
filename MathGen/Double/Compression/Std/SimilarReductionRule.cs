using MathGen.Double.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MathGen.Double.Compression.Std
{
	public class SimilarReductionRule : Rule
	{
		public SimilarReductionRule ()
		{
			this.Where(op => op is Sum || op is Sub);
			this.Replace(this._ReplaceFunc);
		}


		private IFunctionNode _ReplaceFunc(IFunctionNode op)
		{
			List<Element> elements = _ToElements(op);
			List<IFunctionNode> toAdd = new List<IFunctionNode>();
			List<IFunctionNode> toSub = new List<IFunctionNode>();

			for (int i = 0; i < elements.Count; i++)
			{
				double scalar = elements[i].Scalar;
				IFunctionNode node = elements[i].Node;

				if (scalar == 0)
				{
					continue;
				}

				if (scalar > 0)
				{
					if (scalar == 1)
					{
						toAdd.Add(node);
					}
					else
					{
						toAdd.Add(new Mul(new Constant(scalar), node));
					}
				}
				else
				{
					scalar *= -1;

					if (scalar == 1)
					{
						toSub.Add(node);
					}
					else
					{
						toSub.Add(new Mul(new Constant(scalar), node));
					}
				}
			}

			IFunctionNode adds = _ToSumTree(toAdd);
			IFunctionNode subs = _ToSumTree(toSub);
			IFunctionNode newRoot = new Sub(adds, subs);

			if (op.GetAmountOfNodes() > newRoot.GetAmountOfNodes())
			{
				return newRoot;
			}
			return op;
		}


		private IFunctionNode _ToSumTree(List<IFunctionNode> elements)
		{
			if (elements.Count == 0)
			{
				return new Constant(0);
			}

			IFunctionNode res = elements[0];
			for (int i = 1; i < elements.Count; i++)
			{
				res = new Sum(res, elements[i]);
			}
			return res;
		}


		private List<Element> _ToElements(IFunctionNode root)
		{
			Debug.Assert(root is Sum || root is Sub);

			List<Element> elements;
			BinaryOperator bin = root as BinaryOperator;
			if (bin.A is Sum || bin.A is Sub)
			{
				elements = _ToElements(bin.A);
			}
			else
			{
				elements = new List<Element> (1);
				elements.Add(_ToElement(bin.A));
			}

			if (root is Sum sum)
			{
				if (sum.B is Sum || sum.B is Sub)
				{
					return _AddElementsLists(elements, _ToElements(sum.B));
				}
				return _AddOneElementInto(elements, _ToElement(sum.B));
			}

			// roor is Substract. We have to substract B from list
			if (bin.B is Sum || bin.B is Sub)
			{
				return _SubElementsFromList(elements, _ToElements(bin.B));
			}
			return _SubOneElementFrom(elements, _ToElement(bin.B));
		}


		private List<Element> _SubElementsFromList(List<Element> source, List<Element> newEls)
		{
			for (int i = 0; i < newEls.Count; i++)
			{
				_SubOneElementFrom(source, newEls[i]);
			}

			return source;
		}


		private List<Element> _AddElementsLists(List<Element> source, List<Element> newEls)
		{
			for (int i = 0; i < newEls.Count; i++)
			{
				_AddOneElementInto(source, newEls[i]);
			}

			return source;
		}


		private List<Element> _SubOneElementFrom(List<Element> source, Element element)
		{
			element.Scalar *= -1;
			return _AddOneElementInto(source, element);
		}


		private List<Element> _AddOneElementInto(List<Element> source, Element element)
		{
			for (int i = 0; i < source.Count; i++)
			{
				if (source[i] == element)
				{
					source[i] = source[i] + element;
					return source;
				}
			}

			source.Add(element);
			return source;
		}


		private Element _ToElement(IFunctionNode op)
		{
			Debug.Assert(!(op is Sum || op is Sub));

			if (op is Constant || op is Argument)
			{
				return new Element(1, op);
			}

			if (op is Mul mul)
			{
				if (mul.A is Constant c1)
				{
					return new Element(c1.Value, mul.B);
				}
				if (mul.B is Constant c2)
				{
					return new Element(c2.Value, mul.A);
				}

				return new Element(1, op);
			}

			throw new Exception("Unknown type of node");
		}


		private struct Element
		{
			public double Scalar;
			public IFunctionNode Node;


			public Element(double scalar, IFunctionNode node)
			{
				this.Scalar = scalar;
				this.Node = node;
			}


			public static bool operator == (Element a, Element b)
			{
				return (a.Node is Constant && b.Node is Constant)
					|| a.Node.Equals(b.Node);
			}


			public static bool operator != (Element a, Element b)
			{
				return !(a == b);
			}


			public static Element operator + (Element a, Element b)
			{
				Debug.Assert(a == b);

				if (a.Node is Constant ca)
				{
					Constant cb = b.Node as Constant;
					var res = new Constant(ca.Value * a.Scalar + cb.Value * b.Scalar);
					return new Element(1, res);
				}
				return new Element(a.Scalar + b.Scalar, a.Node);
			}


			public static Element operator - (Element a, Element b)
			{
				Debug.Assert(a == b);

				if (a.Node is Constant ca)
				{
					Constant cb = b.Node as Constant;
					var res = new Constant(ca.Value * a.Scalar - cb.Value * b.Scalar);
					return new Element(1, res);
				}
				return new Element(a.Scalar - b.Scalar, a.Node);
			}
		}
	}
}
