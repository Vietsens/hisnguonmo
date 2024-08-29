#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public interface IExpressionFactory
	{
		Expression CreateExpression(string text);
	}
}
