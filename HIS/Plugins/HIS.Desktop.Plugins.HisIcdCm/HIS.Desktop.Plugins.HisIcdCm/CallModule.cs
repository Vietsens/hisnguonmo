using HIS.Desktop.ModuleExt;
using System;
using System.Collections.Generic;
namespace HIS.Desktop.Plugins.HisIcdCm
{
    internal class CallModule
    {
        internal const string MobaExamPresCreate = "HIS.Desktop.Plugins.MobaExamPresCreate";
        public CallModule(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            this.CallModuleProcess(_moduleLink, _roomId, _roomTypeId, _listObj);
        }
        private void CallModuleProcess(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            PluginInstanceBehavior.ShowModule(_moduleLink, _roomId, _roomTypeId, _listObj);
        }
    }
}
