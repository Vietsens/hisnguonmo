#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Collections.Generic;

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class CompositeSpecification : Specification
	{
		private readonly List<ISpecification> _childSpecs = new List<ISpecification>();

		protected CompositeSpecification()
		{
		}

		public void Add(ISpecification spec)
		{
			_childSpecs.Add(spec);
		}

		public void AddRange(IEnumerable<ISpecification> specs)
		{
			_childSpecs.AddRange(specs);
		}

		public bool IsEmpty
		{
			get { return _childSpecs.Count == 0; }
		}

		public IEnumerable<ISpecification> Elements
		{
			get { return _childSpecs; }
		}
	}
}
