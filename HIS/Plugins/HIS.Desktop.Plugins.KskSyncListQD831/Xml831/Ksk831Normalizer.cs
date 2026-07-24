/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>
    /// Chuẩn hoá cây model trước khi serialize để MỌI thẻ XML luôn hiển thị (dù rỗng):
    /// - string null -&gt; "" (thẻ rỗng &lt;TAG&gt;&lt;/TAG&gt; thay vì bị bỏ);
    /// - object con (model) null -&gt; khởi tạo rỗng rồi đệ quy (giữ đủ khối con);
    /// - List null -&gt; danh sách rỗng (thẻ bao vẫn hiện); KHÔNG tự sinh phần tử.
    /// </summary>
    internal static class Ksk831Normalizer
    {
        private const string MODEL_NS = "HIS.Desktop.Plugins.KskSyncListQD831.Xml831";

        internal static void FillEmpty(object obj)
        {
            if (obj == null) return;
            Type t = obj.GetType();
            foreach (var p in t.GetProperties())
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                Type pt = p.PropertyType;

                if (pt == typeof(string))
                {
                    if (p.CanWrite && p.GetValue(obj, null) == null) p.SetValue(obj, "", null);
                }
                else if (IsModelType(pt))
                {
                    object child = p.GetValue(obj, null);
                    if (child == null && p.CanWrite)
                    {
                        child = Activator.CreateInstance(pt);
                        p.SetValue(obj, child, null);
                    }
                    FillEmpty(child);
                }
                else if (IsGenericList(pt))
                {
                    object list = p.GetValue(obj, null);
                    if (list == null && p.CanWrite)
                    {
                        list = Activator.CreateInstance(pt);
                        p.SetValue(obj, list, null);
                    }
                    Type itemType = pt.GetGenericArguments()[0];
                    if (list != null && IsModelType(itemType))
                        foreach (var item in (IEnumerable)list) FillEmpty(item);
                }
            }
        }

        private static bool IsModelType(Type t)
        {
            return t != null && t.IsClass && t != typeof(string) && t.Namespace == MODEL_NS;
        }

        private static bool IsGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
