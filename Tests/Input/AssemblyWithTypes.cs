#region Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>
/// <copyright>
/// Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// </copyright>
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TestClasses
{
	internal enum TestEnum
	{
		Default = 0,
		Test = 1
	}

	internal class InternalClass
	{
		public void PublicMethod()
		{ }

		public class PublicNestedClass
		{
		}
	}

	public class PublicClass
	{
		private void PrivateMethod(string myName)
		{ }

		public void PublicMethod(string yourName)
		{ }

		protected void ProtectedMethod(string itsName)
		{ }

		internal protected void InternalProtectedMethod(string theirNames)
		{ }

		protected internal void ProtectedInternalMethod()
		{ }
	}
}

namespace TestClasses1
{
	public class PublicClass
	{
		public void PublicMethod()
		{ }

		public class PublicNestedClass
		{
		}
	}
}
