#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Specifications
{
	public class SpecificationFactory : ISpecificationProvider
	{
		class SingleDocumentSource : ISpecificationXmlSource
		{
			private readonly XmlDocument _xmlDoc;

			public SingleDocumentSource(Stream xml)
			{
				_xmlDoc = new XmlDocument();
				_xmlDoc.Load(xml);
			}

			public SingleDocumentSource(TextReader xml)
			{
				_xmlDoc = new XmlDocument();
				_xmlDoc.Load(xml);
			}



			#region ISpecificationXmlSource Members

			public string DefaultExpressionLanguage
			{
				get
				{
					var exprLang = _xmlDoc.DocumentElement.GetAttribute("expressionLanguage");

					// if not specified, assume jscript
					return string.IsNullOrEmpty(exprLang) ? "jscript" : exprLang;
				}
			}

			public XmlElement GetSpecificationXml(string id)
			{
				var specNode = (XmlElement)CollectionUtils.SelectFirst(_xmlDoc.GetElementsByTagName("spec"),
					node => ((XmlElement) node).GetAttribute("id") == id);

				if (specNode == null)
					throw new UndefinedSpecificationException(id);

				return specNode;
			}

			public IDictionary<string, XmlElement> GetAllSpecificationsXml()
			{
				var specs = new Dictionary<string, XmlElement>();
				foreach (XmlElement specNode in _xmlDoc.GetElementsByTagName("spec"))
				{
					specs.Add(specNode.GetAttribute("id"), specNode);
				}
				return specs;
			}

			#endregion
		}



		private readonly XmlSpecificationCompiler _builder;
		private readonly ISpecificationXmlSource _xmlSource;

		private readonly Dictionary<string, ISpecification> _cache;

		public SpecificationFactory(Stream xml)
			: this(new SingleDocumentSource(xml))
		{
		}

		public SpecificationFactory(TextReader xml)
			: this(new SingleDocumentSource(xml))
		{
		}


		public SpecificationFactory(ISpecificationXmlSource xmlSource)
		{
			_builder = new XmlSpecificationCompiler(xmlSource.DefaultExpressionLanguage, this);
			_cache = new Dictionary<string, ISpecification>();
			_xmlSource = xmlSource;
		}

		public ISpecification GetSpecification(string id)
		{
			if (_cache.ContainsKey(id))
			{
				return _cache[id];
			}
			var specNode = _xmlSource.GetSpecificationXml(id);
			return _cache[id] = _builder.Compile(specNode, false);
		}

		public IDictionary<string, ISpecification> GetAllSpecifications()
		{
			var specs = new Dictionary<string, ISpecification>();
			foreach (var kvp in _xmlSource.GetAllSpecificationsXml())
			{
				specs.Add(kvp.Key, _builder.Compile(kvp.Value, false));
			}
			return specs;
		}
	}
}
