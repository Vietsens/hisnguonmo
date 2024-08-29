#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public interface ISpecificationProvider
	{
		ISpecification GetSpecification(string id);
	}
}
