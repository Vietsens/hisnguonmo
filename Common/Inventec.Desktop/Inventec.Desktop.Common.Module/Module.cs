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
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Inventec.Desktop.Core;
using System.Drawing;

namespace Inventec.Desktop.Common.Modules
{
    public class Module
    {
        public const long MODULE_TYPE_ID__UC = 1;
        public const long MODULE_TYPE_ID__FORM = 2;
        public const long MODULE_TYPE_ID__COMBO = 3;

        public Module()
        {
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, "", "", 0, false, null, true);
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, false, null, true);
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, isPlugin, null, true);
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin, ExtensionInfo extensionInfo)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, isPlugin, extensionInfo, true);
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin, ExtensionInfo extensionInfo, PluginInfo pluginInfo)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, isPlugin, extensionInfo, true);
            this.PluginInfo = pluginInfo;
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin, ExtensionInfo extensionInfo, PluginInfo pluginInfo, bool visible)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, isPlugin, extensionInfo, visible);
            this.PluginInfo = pluginInfo;
        }

        public Module(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, string icon, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin, ExtensionInfo extensionInfo, PluginInfo pluginInfo, bool visible)
        {
            Init(moduleCode, moduleLink, moduleName, isLeaf, moduletypeId, imageIndex, numberOrder, parentCode, pageGroupName, pageGroupCaption, roomtypeId, isPlugin, extensionInfo, visible);
            this.PluginInfo = pluginInfo;
            this.Icon = icon;
        }

        public void Init(string moduleCode, string moduleLink, string moduleName, bool isLeaf, long moduletypeId, int imageIndex, int numberOrder, string parentCode, string pageGroupName, string pageGroupCaption, long roomtypeId, bool isPlugin, ExtensionInfo extensionInfo, bool vislble)
        {
            this.ModuleCode = moduleCode;
            this.ModuleLink = moduleLink;
            this.ParentCode = parentCode;
            this.PageGroupName = pageGroupName;
            this.PageGroupCaption = pageGroupCaption;

            this.leaf = isLeaf;
            this.ModuleTypeId = moduletypeId;
            this.Visible = vislble;
            this.text = moduleName;
            this.ImageIndex = imageIndex;
            this.NumberOrder = numberOrder;
            this.RoomTypeId = roomtypeId;
            this.IsPlugin = isPlugin;
            this.ExtensionInfo = extensionInfo;
        }
        public PluginInfo PluginInfo { get; set; }
        public ExtensionInfo ExtensionInfo { get; set; }
        public long RoomTypeId { get; set; }
        public long RoomId { get; set; }
        public long ModuleTypeId { get; set; }
        public int NumberOrder { get; set; }
        public string text { get; set; }
        public string Description { get; set; }
        public bool leaf { get; set; }
        public bool Visible { get; set; }
        public bool IsPlugin { get; set; }
        public bool? IsNotShowDialog { get; set; }
        public int ImageIndex { get; set; }
        public string Icon { get; set; }
        public List<Module> Children { get; set; }

        public string ModuleCode { get; set; }
        public string ModuleLink { get; set; }
        public string ParentCode { get; set; }
        public string PageGroupName { get; set; }
        public string PageGroupCaption { get; set; }

        public static List<Module> Clone(List<Module> originalList)
        {
            return originalList != null ? originalList.Select(item => (Module)item.MemberwiseClone()).ToList() : null;
        }
    }
}
