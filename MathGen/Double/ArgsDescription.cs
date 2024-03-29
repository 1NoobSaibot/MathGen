﻿using MathGen.Double.Operators;
using System;


namespace MathGen.Double
{
	public class ArgsDescription
	{
		private readonly string[] _names;
		public int Count => _names.Length;

		public ArgsDescription (params string[] names) {
			_names = names;
		}


		public string GetName(int argIndex)
		{
			return _names[argIndex];
		}


		public int GetIndex(string name)
		{
			for (int i = 0; i < _names.Length; i++)
			{
				if (_names[i] == name)
				{
					return i;
				}
			}

			throw new Exception("Unknown argument name: " + name);
		}


		public Argument CreateArgument(Random rnd)
		{
			int index = rnd.Next(_names.Length);
			return new Argument(index, _names[index]);
		}


		internal IFunctionNode CreateArgument(string symbol)
		{
			int index = GetIndex(symbol);
			return new Argument(index, symbol);
		}
	}
}
