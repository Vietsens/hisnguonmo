﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.Common
{
    internal class Common
    {
        public string CreatedXmlFile<T>(T input, bool encode, bool displayNamspacess, bool saveFile, string path)
        {
            string result;
            try
            {
                var enc = Encoding.UTF8;
                using (var ms = new MemoryStream())
                {
                    var xmlNamespaces = new XmlSerializerNamespaces();
                    if (displayNamspacess)
                        xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                    else
                        xmlNamespaces.Add("", "");

                    var xmlWriterSettings = new System.Xml.XmlWriterSettings
                    {
                        CloseOutput = false,
                        Encoding = enc,
                        OmitXmlDeclaration = false,
                        Indent = true,
                    };
                    using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                    {
                        var s = new XmlSerializer(typeof(T));
                        s.Serialize(xw, input, xmlNamespaces);
                    }
                    result = enc.GetString(ms.ToArray());
                    //kiểm tra người dùng có cần lưu file không
                    if (saveFile)
                    {
                        using (var file = new StreamWriter(path))
                        {
                            file.Write(result);
                        }
                        //return ResultReturn.True;
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            //kiểm tra nếu cần mã hóa file thì mã hóa sau đó trả lại cho ng dùng
            return encode ? EncodeBase64(Encoding.UTF8, result) : result;
        }

        private string EncodeBase64(Encoding encoding, string text)
        {
            if (text == null)
                return null;
            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }
    }
}
