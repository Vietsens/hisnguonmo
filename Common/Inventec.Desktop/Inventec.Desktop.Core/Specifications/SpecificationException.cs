#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System;

namespace Inventec.Desktop.Core.Specifications
{
	public class SpecificationException : Exception
	{
		public SpecificationException(string message)
			: base(message)
		{

		}

		public SpecificationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
