using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.UCD
{
    public class InitUCD
    {
        public delegate void ProcessFormOwner(string lang);
        public delegate void ReloadLoginnameAfterLeave(string loginname);

        internal Inventec.Common.WebApiClient.ApiConsumer sdaCosumer;
        internal string APPLICATION_CODE;
        internal string SYSTEM_FOLDER;
        internal string APP_FOLDER;
        internal ProcessFormOwner processFormOwner;
        public ReloadLoginnameAfterLeave reloadLoginnameAfterLeave;
        public object Branchs;
        public long? FirstBranchId;
        public LabelString LabelString;
        public string LoginnameDefault { get; set; }

        public InitUCD(Inventec.Common.WebApiClient.ApiConsumer sdaconsumer, string AppCode, string SysFolder, string AppFolder)
            : this(sdaconsumer, AppCode, SysFolder, AppFolder, null)
        {
        }

        public InitUCD(Inventec.Common.WebApiClient.ApiConsumer sdaconsumer, string AppCode, string SysFolder, string AppFolder, ProcessFormOwner _processFormOwner)
        {
            this.sdaCosumer = sdaconsumer;
            this.APPLICATION_CODE = AppCode;
            this.SYSTEM_FOLDER = SysFolder;
            this.APP_FOLDER = AppFolder;
            this.processFormOwner = _processFormOwner;
        }
    }
}
