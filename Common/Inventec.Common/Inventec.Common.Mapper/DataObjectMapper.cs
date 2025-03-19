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
using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Inventec.Common.Mapper
{
    public static class DataObjectMapper
    {
        /// <summary>
        /// Su dung trong noi bo du an copy tu RAW --> DTO.
        /// Vi du: Map<AbcDefDTO>(dtoObject, rawObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objDestination"></param>
        /// <param name="objSource"></param>
        public static void Map<T>(object objDestination, object objSource)
        {
            try
            {
                if (objSource != null && objDestination != null)
                {
                    PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(objSource.GetType());

                    Dictionary<string, object> dicValue = new Dictionary<string, object>();
                    foreach (var prop in pi)
                    {
                        if (!dicValue.ContainsKey(prop.Name))
                        {
                            dicValue[prop.Name] = prop.GetValue(objSource, null);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Inventec.Common.Mapper___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prop), prop));
                        }
                    }

                    if (dicValue != null)
                    {
                        Map<T>(objDestination, dicValue);
                    }
                    else
                    {
                        LogSystem.Debug("Khong serialize duoc objectSource.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Map<ObjectType>(object, jsonStringSerializeFromSourceObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="jsonText"></param>
        public static void Map<T>(object obj, Dictionary<string, object> dicValue)
        {
            try
            {
                if (dicValue != null && dicValue.Count > 0 && obj != null)
                {
                    string iName = "";
                    string strType = "";
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(typeof(T));
                    foreach (var item in pi)
                    {
                        try
                        {
                            strType = item.PropertyType.ToString();
                            iName = item.Name;
                            if (dicValue.ContainsKey(iName))
                            {
                                if (dicValue[iName] != null)
                                {
                                    if (item.PropertyType.Equals(typeof(short)) || item.PropertyType.Equals(typeof(short?)))
                                    {
                                        item.SetValue(obj, (short)(decimal.Parse(dicValue[iName].ToString())));
                                    }
                                    else if (item.PropertyType.Equals(typeof(decimal)) || item.PropertyType.Equals(typeof(decimal?)))
                                    {
                                        item.SetValue(obj, decimal.Parse(dicValue[iName].ToString()));
                                    }
                                    else if (item.PropertyType.Equals(typeof(long)) || item.PropertyType.Equals(typeof(long?)))
                                    {
                                        item.SetValue(obj, (long)decimal.Parse(dicValue[iName].ToString()));
                                    }
                                    else if (item.PropertyType.Equals(typeof(int)) || item.PropertyType.Equals(typeof(int?)))
                                    {
                                        item.SetValue(obj, (int)decimal.Parse(dicValue[iName].ToString()));
                                    }
                                    else if (item.PropertyType.Equals(typeof(double)) || item.PropertyType.Equals(typeof(double?)))
                                    {
                                        item.SetValue(obj, double.Parse(dicValue[iName].ToString()));
                                    }
                                    else
                                    {
                                        item.SetValue(obj, dicValue[iName]);
                                    }
                                }
                                else
                                {
                                    item.SetValue(obj, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Mapper loi.");
                            sb.AppendLine("T = " + typeof(T).ToString());
                            sb.AppendLine(Inventec.Common.Logging.LogUtil.TraceData("dicValue = ", dicValue));
                            sb.AppendLine("iName = " + iName);
                            sb.AppendLine("strType = " + strType);
                            if (dicValue[iName] != null)
                            {
                                sb.AppendLine("Value = " + dicValue[iName]);
                                sb.AppendLine("Valuetype = " + dicValue[iName].GetType());
                            }

                            sb.AppendLine(Inventec.Common.Logging.LogUtil.TraceData("obj = ", obj));
                            Inventec.Common.Logging.LogSystem.Debug(sb.ToString(), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
    }
}
