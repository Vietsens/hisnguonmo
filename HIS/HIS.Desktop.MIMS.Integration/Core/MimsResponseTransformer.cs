using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public static class MimsResponseTransformer
    {
        public static string XmlToHtml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return "<html><body><p>No XML returned from MIMS.</p></body></html>";
            }

            var trimmed = xml.TrimStart();
            if (trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase))
            {
                // Already HTML, no need to transform
                return xml;
            }

            try
            {
                var settings = new XsltSettings(enableDocumentFunction: true, enableScript: true);
                var resolver = new XmlUrlResolver();

                var xslt = new XslCompiledTransform();
                xslt.Load(MimsConfig.StyleSheetPath, settings, resolver);

                string html;
                using (var xmlReader = XmlReader.Create(new StringReader(xml)))
                using (var sw = new StringWriter())
                using (var writer = XmlWriter.Create(sw, xslt.OutputSettings))
                {
                    xslt.Transform(xmlReader, writer);
                    html = sw.ToString();
                }

                // Inject <base> tag so relative CSS/JS paths resolve to the MIMS resources folder
                try
                {
                    var baseDir = MimsConfig.ResourceBasePath;
                    if (!baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        baseDir += Path.DirectorySeparatorChar;

                    var baseUri = new Uri(baseDir);
                    var baseTag = string.Format("<base href=\"{0}\">", baseUri.AbsoluteUri);

                    const string headTag = "<head>";
                    int idx = html.IndexOf(headTag, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        idx += headTag.Length;
                        html = html.Insert(idx, baseTag);
                    }
                }
                catch (Exception ex)
                {
                    // Ignore base injection errors and return the raw HTML
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                return html;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                string safeMessage = System.Security.SecurityElement.Escape(ex.Message);
                return "<html><body><h3>Error while transforming MIMS XML</h3><pre>" +
                       safeMessage + "</pre></body></html>";
            }
        }
    }
}
