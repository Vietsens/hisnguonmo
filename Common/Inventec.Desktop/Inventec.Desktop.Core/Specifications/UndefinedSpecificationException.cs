#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class UndefinedSpecificationException : SpecificationException
	{
		public UndefinedSpecificationException(string id)
			: base(string.Format(SR.ExceptionInvalidSpecificationId, id))
		{
		}
	}
}
