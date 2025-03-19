/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inventec.Common.XmlConfig
{
    public class XmlApplicationConfig
    {
        #region Private key
        private const string CONFIG_KEY_KEY = "Key";
        private const string CONFIG_KEY_KEY_CODE = "KeyCode";
        private const string CONFIG_KEY_CREATE_TIME = "CreateTime";
        private const string CONFIG_KEY_CREATOR = "Creator";
        private const string CONFIG_KEY_MODIFY_TIME = "ModifyTime";
        private const string CONFIG_KEY_MODIFIER = "Modifier";
        private const string CONFIG_KEY_TITLE = "Title";
        private const string CONFIG_KEY_VALUE = "Value";
        private const string CONFIG_KEY_DEFAULT_VALUE = "DefaultValue";
        private const string CONFIG_KEY_VALUE_TYPE = "ValueType";
        private const string CONFIG_KEY_VALUE_TYPE_DESCRIPTION = "Description";
        private const string CONFIG_KEY_TUTORIAL = "Tutorial";
        private const string CONFIG_KEY_VALUE_ALLOW = "ValueAllow";
        private const string CONFIG_KEY_VALUE_ALLOW_MIN = "Min";
        private const string CONFIG_KEY_VALUE_ALLOW_MAX = "Max";
        private const string CONFIG_KEY_VALUE_ALLOW_IN = "In";
        #endregion

        private XDocument CurrentDocument { get; set; }
        private string Language { get; set; }
        private string PathXmlFile { get; set; }

        public XmlApplicationConfig(string pathXmlFile, string lang)
        {
            this.Language = lang;
            this.PathXmlFile = pathXmlFile;
            this.CurrentDocument = XDocument.Load(pathXmlFile);
        }

        public List<ElementNode> GetElements()
        {
            List<ElementNode> listResult = new List<ElementNode>();
            try
            {
                if (this.CurrentDocument != null)
                {
                    List<XElement> partKeys = (from item in this.CurrentDocument.Descendants(CONFIG_KEY_KEY) select item).ToList();
                    if (partKeys != null && partKeys.Count() > 0)
                    {
                        foreach (XElement item in partKeys)
                        {
                            listResult.Add(CreateElementNodeByXElement(item));
                        }
                    }
                }
            }
            catch (Exception)
            {
                listResult = new List<ElementNode>();
                throw;
            }
            return listResult;
        }

        private ElementNode CreateElementNodeByXElement(XElement item)
        {
            ElementNode eNode = new ElementNode();
            try
            {
                eNode.KeyCode = (item.Attribute(CONFIG_KEY_KEY_CODE) == null ? "" : item.Attribute(CONFIG_KEY_KEY_CODE).Value);
                eNode.CreateTime = Convert.ToInt64((item.Attribute(CONFIG_KEY_CREATE_TIME) == null ? "0" : item.Attribute(CONFIG_KEY_CREATE_TIME).Value));
                eNode.Creator = (item.Attribute(CONFIG_KEY_CREATOR) == null ? "" : item.Attribute(CONFIG_KEY_CREATOR).Value);
                eNode.ModifyTime = Convert.ToInt64((item.Attribute(CONFIG_KEY_MODIFY_TIME) == null ? "0" : item.Attribute(CONFIG_KEY_MODIFY_TIME).Value));
                eNode.Modifier = (item.Attribute(CONFIG_KEY_MODIFIER) == null ? "" : item.Attribute(CONFIG_KEY_MODIFIER).Value);
                eNode.Title = (item.Element(CONFIG_KEY_TITLE).Attribute(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Language.ToLower())) == null ? "" : item.Element(CONFIG_KEY_TITLE).Attribute(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Language.ToLower())).Value);
                eNode.Value = (item.Element(CONFIG_KEY_VALUE) == null ? "" : item.Element(CONFIG_KEY_VALUE).Value);
                eNode.DefaultValue = (item.Element(CONFIG_KEY_DEFAULT_VALUE) == null ? "" : item.Element(CONFIG_KEY_DEFAULT_VALUE).Value);
                eNode.ValueType = (item.Element(CONFIG_KEY_VALUE_TYPE) == null ? "" : item.Element(CONFIG_KEY_VALUE_TYPE).Value);
                eNode.ValueTypeDescription = (item.Element(CONFIG_KEY_VALUE_TYPE).Attribute(CONFIG_KEY_VALUE_TYPE_DESCRIPTION) == null ? "" : item.Element(CONFIG_KEY_VALUE_TYPE).Attribute(CONFIG_KEY_VALUE_TYPE_DESCRIPTION).Value);
                eNode.Tutorial = (item.Element(CONFIG_KEY_TUTORIAL).Attribute(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Language.ToLower())) == null ? "" : item.Element(CONFIG_KEY_TUTORIAL).Attribute(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Language.ToLower())).Value);
                eNode.ValueAllowMin = (item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_MIN) == null ? "" : item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_MIN).Value);
                eNode.ValueAllowMax = (item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_MAX) == null ? "" : item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_MAX).Value);
                eNode.ValueAllowIn = (item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_IN) == null ? "" : item.Element(CONFIG_KEY_VALUE_ALLOW).Element(CONFIG_KEY_VALUE_ALLOW_IN).Value);
            }
            catch (Exception ex)
            {
                eNode = new ElementNode();
            }

            return eNode;
        }

        public bool UpdateElements(List<ElementNode> listData, string loginName)
        {
            bool result = false;
            try
            {
                if (this.CurrentDocument != null)
                {
                    List<XElement> partKeys = (from item in this.CurrentDocument.Descendants(CONFIG_KEY_KEY) select item).ToList();
                    if (partKeys != null && partKeys.Count() > 0)
                    {
                        foreach (XElement item in partKeys)
                        {
                            var eNode = listData.SingleOrDefault(o => o.KeyCode == item.Attribute(CONFIG_KEY_KEY_CODE).Value);
                            if (eNode != null)
                            {
                                try
                                {
                                    item.Attribute(CONFIG_KEY_MODIFY_TIME).Value = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                catch { }
                                item.Attribute(CONFIG_KEY_MODIFIER).Value = loginName;
                                item.Element(CONFIG_KEY_VALUE).Value = eNode.Value.ToString();
                            }
                        }
                        this.CurrentDocument.Save(this.PathXmlFile);
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        public object GetKeyValue(string strKeyCode)
        {
            object value = null;
            object CurrentValue = null;
            try
            {
                if (!String.IsNullOrEmpty(strKeyCode))
                {
                    XElement el = (from item in this.CurrentDocument.Descendants(CONFIG_KEY_KEY) select item).SingleOrDefault(o => o.Attribute(CONFIG_KEY_KEY_CODE).Value == strKeyCode);
                    if (el != null)
                    {
                        if (!String.IsNullOrEmpty(el.Element(CONFIG_KEY_VALUE).Value))
                            CurrentValue = el.Element(CONFIG_KEY_VALUE).Value;
                        else
                            CurrentValue = el.Element(CONFIG_KEY_DEFAULT_VALUE).Value;
                        switch (el.Element(CONFIG_KEY_VALUE_TYPE).Value)
                        {
                            case "long":
                                try
                                {
                                    value = Convert.ToInt64(CurrentValue);
                                }
                                catch { }
                                break;
                            case "short":
                                try
                                {
                                    value = Convert.ToInt16(CurrentValue);
                                }
                                catch { }
                                break;
                            case "decimal":
                                try
                                {
                                    value = Convert.ToDecimal(CurrentValue);
                                }
                                catch { }
                                break;
                            case "string":
                                try
                                {
                                    value = CurrentValue;
                                }
                                catch { }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return value;
        }
    }
}
