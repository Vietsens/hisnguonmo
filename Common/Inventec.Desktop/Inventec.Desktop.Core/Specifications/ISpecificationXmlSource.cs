#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Collections.Generic;
using System.Xml;

namespace Inventec.Desktop.Core.Specifications
{
	public interface ISpecificationXmlSource
	{
		string DefaultExpressionLanguage { get; }
		XmlElement GetSpecificationXml(string id);
		IDictionary<string, XmlElement> GetAllSpecificationsXml();
	}
}
