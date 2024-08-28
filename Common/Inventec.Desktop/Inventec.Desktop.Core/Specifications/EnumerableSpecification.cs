#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Collections;

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class EnumerableSpecification : Specification
	{
		private readonly ISpecification _elementSpecification;

		protected EnumerableSpecification(ISpecification elementSpecification)
		{
			Platform.CheckForNullReference(elementSpecification, "elementSpecification");
			_elementSpecification = elementSpecification;
		}

		protected internal ISpecification ElementSpec
		{
			get { return _elementSpecification; }
		}

		protected static IEnumerable AsEnumerable(object obj)
		{
			var enumerable = obj as IEnumerable;
			if (enumerable == null)
				throw new SpecificationException(SR.ExceptionCastExpressionEnumerable);

			return enumerable;
		}
	}
}
