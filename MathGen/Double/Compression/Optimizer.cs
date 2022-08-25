namespace MathGen.Double.Compression
{
	public static class Optimizer
	{
		private static Rule[] _rules = new Rule[]
		{
			#region Пусті операції
			// 0 * Any => 0
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& (mul.A.IsZero() || mul.B.IsZero());
				})
				.Replace(_ => new Constant(0))
			,


			// A + 0 => A;
			new Rule()
				.Where(op => {
					return op is Sum b && b.B.IsZero();
				})
				.Replace(op => (op as Sum).A)
			,


			// 0 + B = B
			new Rule()
				.Where(op => {
					return op is Sum b && b.A.IsZero();
				})
				.Replace(op => (op as Sum).B)
			,


			// A - A => 0
			new Rule()
				.Where(op =>
				{
					return op is Sub sub && sub.A.Equals(sub.B);
				})
				.Replace(_ => new Constant(0))
			,


			// A - 0 => A
			new Rule()
				.Where(op =>
				{
					return op is Sub sub && sub.B.IsZero();
				})
				.Replace(op => (op as Sub).A)
			,


			// 1 * B => B
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Constant ca
						&& ca.Value == 1;
				})
				.Replace(op => (op as Mul).B)
			,


			// A * 1 => A
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.B is Constant cb
						&& cb.Value == 1;
				})
				.Replace(op => (op as Mul).A)
			,
			#endregion

			#region Підйом констант, переміщення їх вправо
			#region Sum
			// Any + (Any + Const) => Const + (Any + Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.B is Sum sumB
						&& sumB.B is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sum sumB = sum.B as Sum;
					return sumB.B + (sum.A + sumB.A);
				})
			,


			// Any + (Const + Any) => Const + (Any + Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.B is Sum sumB
						&& sumB.A is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sum sumB = sum.B as Sum;
					return sumB.A + (sum.A + sumB.B);
				})
			,


			// (Any + Const) + Any => Const + (Any + Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Sum sumA
						&& sumA.B is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sum sumA = sum.A as Sum;
					return sumA.B + (sum.B + sumA.A);
				})
			,


			// (Const + Any) + Any => Const + (Any + Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Sum sumA
						&& sumA.A is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sum sumA = sum.A as Sum;
					return sumA.A + (sum.B + sumA.B);
				})
			,
			#endregion

			#region Multiplication
			// Any * (Any * Const) => Const * (Any * Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.B is Mul mulB
						&& mulB.B is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Mul mulB = mul.B as Mul;
					return mulB.B * (mul.A * mulB.A);
				})
			,


			// Any * (Const * Any) => Const * (Any * Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.B is Mul mulB
						&& mulB.A is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Mul mulB = mul.B as Mul;
					return mulB.A * (mul.A * mulB.B);
				})
			,


			// (Any * Const) * Any => Const * (Any * Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Mul mulA
						&& mulA.B is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Mul mulA = mul.A as Mul;
					return new Mul(mulA.B, new Mul(mul.B, mulA.A));
				})
			,


			// (Const * Any) * Any => Const * (Any * Any)		Constants go up
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Mul mulA
						&& mulA.A is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Mul mulA = mul.A as Mul;
					return new Mul(mulA.A, new Mul(mul.B, mulA.B));
				})
			,
			#endregion

			#region Substract
			// Any - (Any - Constant)		=> 	Constant + (Any - Any)
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.B is Sub subB
						&& subB.B is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sub subB = sub.B as Sub;

					Sub newSub = new Sub(sub.A, subB.A);
					return new Sum(subB.B, newSub);
				})
			,


			// Any - (Constant - Any)		=>	(Any + Any)	- Constant
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.B is Sub subB
						&& subB.A is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sub subB = sub.B as Sub;

					Sum newSum = new Sum(sub.A, subB.B);
					return new Sub(newSum, subB.A);
				})
			,


			// (Any - Constant) - Any		=>	(Any - Any)	- Constant
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.A is Sub subA
						&& subA.B is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sub subA = sub.A as Sub;

					Sub newSub = new Sub(subA.A, sub.B);
					return new Sub(newSub, subA.B);
				})
			,


			// (Constant - Any) - Any		=>	Constant - (Any + Any)
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.A is Sub subA
						&& subA.A is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sub subA = sub.A as Sub;

					Sum newSum = new Sum(subA.B, sub.B);
					return new Sub(subA.A, newSum);
				})
			,
			#endregion
			
			#region Sum with Substract
			// Any + (Any - Const) 		=>		(Any + Any) - Const
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.B is Sub sub
						&& sub.B is Constant;
				})
				.Replace(op => {
					Sum sum = op as Sum;
					Sub sub = sum.B as Sub;

					Sum newSum = new Sum(sum.A, sub.A);
					return new Sub(newSum, sub.B);
				})
			,


			// Any + (Const - Any)		=>		Const + (Any - Any)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.B is Sub sub
						&& sub.A is Constant;
				})
				.Replace(op => {
					Sum sum = op as Sum;
					Sub sub = sum.B as Sub;

					Sub newSub = new Sub(sum.A, sub.B);
					return new Sum(sub.A, newSub);
				})
			,


			// (Any - Const) + Any		=>		(Any + Any)	- Const
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Sub sub
						&& sub.B is Constant;
				})
				.Replace(op => {
					Sum sum = op as Sum;
					Sub sub = sum.A as Sub;

					Sum newSum = new Sum(sub.A, sum.B);
					return new Sub(newSum, sub.B);
				})
			,


			// (Const - An1) + An2		=>		Const + (An2 - An1)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Sub sub
						&& sub.A is Constant;
				})
				.Replace(op => {
					Sum sum = op as Sum;
					Sub sub = sum.A as Sub;

					Sub newSub = new Sub(sum.B, sub.B);
					return new Sum(sub.A, newSub);
				})
			,
			#endregion
			
			#region Substract sith Sum
			// Any - (Any + Const)		=>		(Any - Any) - Const
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.B is Sum sum
						&& sum.B is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sum sum = sub.B as Sum;

					Sub newSub = new Sub(sub.A, sum.A);
					return new Sub(newSub, sum.B);
				})
			,


			// Any - (Const + Any)		=>		(Any - Any) - Const
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.B is Sum sum
						&& sum.A is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sum sum = sub.B as Sum;

					Sub newSub = new Sub(sub.A, sum.B);
					return new Sub(newSub, sum.A);
				})
			,


			// (An1 + Const) - An2		=>		Const + (An1 - An2)
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.A is Sum sum
						&& sum.B is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sum sum = sub.A as Sum;

					Sub newSub = new Sub(sum.A, sub.B);
					return new Sum(sum.B, newSub);
				})
			,


			// (Const + An1) - An2		=>		Const + (An1 - An2)
			new Rule()
				.Where(op => {
					return op is Sub sub
						&& sub.A is Sum sum
						&& sum.A is Constant;
				})
				.Replace(op => {
					Sub sub = op as Sub;
					Sum sum = sub.A as Sum;

					Sub newSub = new Sub(sum.B, sub.B);
					return new Sum(sum.A, newSub);
				})
			,
			#endregion
			#endregion

			#region Зведення констант
			#region Primitive C +-* C => C
			// Constant + Constant => Constant
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Constant
						&& sum.B is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					return sum.A + sum.B; // It should return a constant-node, not a sum.
				})
			,


			// Constant - Constant => Constant
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Constant
						&& sub.B is Constant;
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;
					return sub.A - sub.B;
				})
			,


			// Constant * Constant => Constant
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					return mul.A * mul.B;
				})
			,
			#endregion

			#region Sum
			// Const + (Const + Any) => NewConst + Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Constant
						&& sum.B is Sum sumB
						&& sumB.A is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;

					Constant ca = sum.A as Constant;
					Sum sumB = sum.B as Sum;
					Constant cb = sumB.A as Constant;

					Constant a = new Constant(ca.Value + cb.Value);
					IFunctionNode b = sumB.B;
					return new Sum(a, b);
				})
			,


			// Const + (Any + Const) => NewConst + Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Constant
						&& sum.B is Sum sumB
						&& sumB.B is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;

					Constant ca = sum.A as Constant;
					Sum sumB = sum.B as Sum;
					Constant cb = sumB.B as Constant;

					Constant a = new Constant(ca.Value + cb.Value);
					IFunctionNode b = sumB.A;
					return new Sum(a, b);
				})
			,


			// (Const + Any) + Const => NewConst + Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Sum sumA
						&& sum.B is Constant
						&& sumA.A is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;

					Sum sumA = sum.A as Sum;
					Constant ca = sum.B as Constant;
					Constant cb = sumA.A as Constant;

					Constant a = new Constant(ca.Value + cb.Value);
					IFunctionNode b = sumA.B;
					return new Sum(a, b);
				})
			,


			// (Any + Const) + Const => NewConst + Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Sum sumA
						&& sum.B is Constant
						&& sumA.B is Constant;
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;

					Sum sumA = sum.A as Sum;
					Constant ca = sum.B as Constant;
					Constant cb = sumA.B as Constant;

					Constant a = new Constant(ca.Value + cb.Value);
					IFunctionNode b = sumA.A;
					return new Sum(a, b);
				})
			,
			#endregion

			#region Sub
			// Const - (Const - Any) => NewConst + Any
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Constant
						&& sub.B is Sub subB
						&& subB.A is Constant;
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;

					Constant ca = sub.A as Constant;
					Sub subB = sub.B as Sub;
					Constant cb = subB.A as Constant;

					IFunctionNode newConstant = ca - cb;
					return newConstant + subB.B;
				})
			,


			// Const - (Any - Const) => NewConst - Any
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Constant
						&& sub.B is Sub subB
						&& subB.B is Constant;
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;

					Constant ca = sub.A as Constant;
					Sub subB = sub.B as Sub;
					Constant cb = subB.B as Constant;

					IFunctionNode newConst = ca + cb;
					return newConst - subB.A;
				})
			,


			// (Const - Any) - Const => NewConst - Any
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Sub subA
						&& sub.B is Constant
						&& subA.A is Constant;
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;

					Sub subA = sub.A as Sub;
					Constant ca = sub.B as Constant;
					Constant cb = subA.A as Constant;

					IFunctionNode newConstant = ca - cb;
					return newConstant - subA.B;
				})
			,


			// (Any - Const) - Const => Any - newConst
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Sub subA
						&& sub.B is Constant
						&& subA.B is Constant;
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;

					Sub subA = sub.A as Sub;
					Constant ca = sub.B as Constant;
					Constant cb = subA.B as Constant;

					IFunctionNode newConst = ca + cb;
					return subA.A - newConst;
				})
			,//*/
			#endregion

			#region Multiplication
			// Const * (Const * Any) => NewConst * Any
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Mul mulB
						&& mulB.A is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;

					Constant ca = mul.A as Constant;
					Mul mulB = mul.B as Mul;
					Constant cb = mulB.A as Constant;

					Constant a = new Constant(ca.Value * cb.Value);
					IFunctionNode b = mulB.B;
					return new Mul(a, b);
				})
			,


			// Const * (Any * Const) => NewConst * Any
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Mul mulB
						&& mulB.B is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;

					Constant ca = mul.A as Constant;
					Mul mulB = mul.B as Mul;
					Constant cb = mulB.B as Constant;

					Constant a = new Constant(ca.Value * cb.Value);
					IFunctionNode b = mulB.A;
					return new Mul(a, b);
				})
			,


			// (Const * Any) * Const => NewConst * Any
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Mul mulA
						&& mul.B is Constant
						&& mulA.A is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;

					Mul mulA = mul.A as Mul;
					Constant ca = mul.B as Constant;
					Constant cb = mulA.A as Constant;

					Constant a = new Constant(ca.Value * cb.Value);
					IFunctionNode b = mulA.B;
					return new Mul(a, b);
				})
			,


			// (Any * Const) * Const => NewConst * Any
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Mul mulA
						&& mul.B is Constant
						&& mulA.B is Constant;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;

					Mul mulA = mul.A as Mul;
					Constant ca = mul.B as Constant;
					Constant cb = mulA.B as Constant;

					Constant a = new Constant(ca.Value * cb.Value);
					IFunctionNode b = mulA.A;
					return new Mul(a, b);
				})
			,
			#endregion
			
			#region Multiplication with Sum
			// Const1 * (Any + Const2)		=>		NewConst + (Const1 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Sum sum
						&& sum.B is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sum sum = mul.B as Sum;

					Constant ca = mul.A as Constant;
					Constant cb = sum.B as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.A, sum.A);

					return new Sum(newConst, newMul);
				})
			,


			// Const1 * (Const2 + Any)		=>		NewConst + (Const1 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Sum sum
						&& sum.A is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sum sum = mul.B as Sum;

					Constant ca = mul.A as Constant;
					Constant cb = sum.A as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.A, sum.B);

					return new Sum(newConst, newMul);
				})
			,


			// (Any + Const1) * Const2		=>		NewConst + (Const2 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.B is Constant
						&& mul.A is Sum sum
						&& sum.B is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sum sum = mul.A as Sum;

					Constant ca = mul.B as Constant;
					Constant cb = sum.B as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.B, sum.A);

					return new Sum(newConst, newMul);
				})
			,


			// (Const1 + Any) * Const2		=>		NewConst + (Const2 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.B is Constant
						&& mul.A is Sum sum
						&& sum.A is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sum sum = mul.A as Sum;

					Constant ca = mul.B as Constant;
					Constant cb = sum.A as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.B, sum.B);

					return new Sum(newConst, newMul);
				})
			,
			#endregion

			#region Multiplication with Substract
			// Const1 * (Any - Const2)		=>		(Const1 * Any) - NewConst
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Sub sub
						&& sub.B is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sub sub = mul.B as Sub;

					Constant ca = mul.A as Constant;
					Constant cb = sub.B as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.A, sub.A);

					return new Sub(newMul, newConst);
				})
			,


			// Const1 * (Const2 - Any)		=>		NewConst - (Const1 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.A is Constant
						&& mul.B is Sub sub
						&& sub.A is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sub sub = mul.B as Sub;

					Constant ca = mul.A as Constant;
					Constant cb = sub.A as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.A, sub.B);

					return new Sub(newConst, newMul);
				})
			,


			// (Any - Const1) * Const2		=>		(Const2 * Any) - NewConst
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.B is Constant
						&& mul.A is Sub sub
						&& sub.B is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sub sub = mul.A as Sub;

					Constant ca = mul.B as Constant;
					Constant cb = sub.B as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.B, sub.A);

					return new Sub(newMul, newConst);
				})
			,


			// (Const1 - Any) * Const2		=>		NewConst - (Const2 * Any)
			new Rule()
				.Where(op => {
					return op is Mul mul
						&& mul.B is Constant
						&& mul.A is Sub sub
						&& sub.A is Constant;
				})
				.Replace(op => {
					Mul mul = op as Mul;
					Sub sub = mul.A as Sub;

					Constant ca = mul.B as Constant;
					Constant cb = sub.A as Constant;
					Constant newConst = new Constant(ca.Value * cb.Value);
					Mul newMul = new Mul(mul.B, sub.B);

					return new Sub(newConst, newMul);
				})
			,
			#endregion
			#endregion

			#region Скорочення подібних
			// A + A => 2 * A
			new Rule()
				.Where(op =>
				{
					return op is Sum s && s.A.Equals(s.B);
				})
				.Replace(op => new Mul(new Constant(2), (op as Sum).A))
			,

			#region Positives and Negatives
			// A + (Any - A) => Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.B is Sub subB
						&& subB.B.Equals(sum.A);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sub subB = sum.B as Sub;
					return subB.A;
				})
			,


			// (Any - B) + B => Any
			new Rule()
				.Where(op =>
				{
					return op is Sum sum
						&& sum.A is Sub subA
						&& subA.B.Equals(sum.B);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Sub subA = sum.A as Sub;
					return subA.A;
				})
			,


			// A - (Any + A) => Any
			// A - (A + Any) => Any
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.B is Sum sum
						&& (sub.A.Equals(sum.A) || sub.A.Equals(sum.B));
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;
					Sum sum = sub.B as Sum;
					if (sub.A.Equals(sum.A))
					{
						return sum.B;
					}
					return sum.A;
				})
			,


			// (Any + A) - A => Any
			// (A + Any) - A => Any
			new Rule()
				.Where(op =>
				{
					return op is Sub sub
						&& sub.A is Sum sum
						&& (sub.B.Equals(sum.A) || sub.B.Equals(sum.B));
				})
				.Replace(op =>
				{
					Sub sub = op as Sub;
					Sum sum = sub.A as Sum;
					if (sub.B.Equals(sum.A))
					{
						return sum.B;
					}
					return sum.A;
				})
			,
			#endregion

			#region (A * B) + (A * C) = A * (B + C)
			// (A * B) + (A * C) => A * (B + C)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Mul mulA
						&& sum.B is Mul mulB
						&& mulA.A.Equals(mulB.A);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Mul mulA = sum.A as Mul;
					Mul mulB = sum.B as Mul;

					Sum newSum = new Sum(mulA.B, mulB.B);
					return new Mul(mulA.A, newSum);
				})
			,


			// (A * B) + (C * A) => A * (B + C)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Mul mulA
						&& sum.B is Mul mulB
						&& mulA.A.Equals(mulB.B);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Mul mulA = sum.A as Mul;
					Mul mulB = sum.B as Mul;

					Sum newSum = new Sum(mulA.B, mulB.A);
					return new Mul(mulA.A, newSum);
				})
			,


			// (A * B) + (B * C) => B * (A + C)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Mul mulA
						&& sum.B is Mul mulB
						&& mulA.B.Equals(mulB.A);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Mul mulA = sum.A as Mul;
					Mul mulB = sum.B as Mul;

					Sum newSum = new Sum(mulA.A, mulB.B);
					return new Mul(mulA.B, newSum);
				})
			,


			// (A * B) + (C * B) => B * (A + C)
			new Rule()
				.Where(op => {
					return op is Sum sum
						&& sum.A is Mul mulA
						&& sum.B is Mul mulB
						&& mulA.B.Equals(mulB.B);
				})
				.Replace(op =>
				{
					Sum sum = op as Sum;
					Mul mulA = sum.A as Mul;
					Mul mulB = sum.B as Mul;

					IFunctionNode newSum = mulA.A + mulB.A;
					return mulA.B * newSum;
				})
			,//*/
			#endregion
			#endregion

			#region Розкриття множення
			// (A + B) * C		=>		(A * C) + (B * C)
			/*new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Sum sum;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Sum sum = mul.A as Sum;

					Mul mulA = new Mul(sum.A, mul.B);
					Mul mulB = new Mul(sum.B, mul.B);
					return new Sum(mulA, mulB);
				})
			,


			// C * (A + B)		=>		(A * C) + (B * C)
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.B is Sum sum;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Sum sum = mul.B as Sum;

					Mul mulA = new Mul(sum.A, mul.A);
					Mul mulB = new Mul(sum.B, mul.A);
					return new Sum(mulA, mulB);
				})
			,


			// (A - B) * C		=>		(A * C) - (B * C)
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.A is Sub;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Sub sub = mul.A as Sub;

					Mul mulA = new Mul(sub.A, mul.B);
					Mul mulB = new Mul(sub.B, mul.B);
					return new Sub(mulA, mulB);
				})
			,


			// C * (A - B)		=>		(A * C) - (B * C)
			new Rule()
				.Where(op =>
				{
					return op is Mul mul
						&& mul.B is Sub;
				})
				.Replace(op =>
				{
					Mul mul = op as Mul;
					Sub sub = mul.B as Sub;

					Mul mulA = new Mul(sub.A, mul.A);
					Mul mulB = new Mul(sub.B, mul.A);
					return new Sub(mulA, mulB);
				})
			,//*/
			#endregion
		};


		public static IFunctionNode Optimize(IFunctionNode root)
		{
			if (root is BinaryOperator b)
			{
				b.A = Optimize(b.A);
				b.B = Optimize(b.B);
			}

			for (int i = 0; i < _rules.Length; i++)
			{
				root = _rules[i].Optimize(root);
			}

			return root;
		}
	}
}
