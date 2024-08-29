using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Data
{
    class DataShow
    {
        internal string _KEYCODE;
        internal string _VALUE;
        internal string _DESCRIPTION;
        internal string _DEFAULT_VALUE;

        internal DataShow()
        {
        }

        public string KEYCODE
        {
            get { return _KEYCODE; }
            set { _KEYCODE = value; }
        }

        public string VALUE
        {
            get { return _VALUE; }
            set { _VALUE = value; }
        }

        public string DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set { _DESCRIPTION = value; }
        }

        public string DEFAULT_VALUE
        {
            get { return _DEFAULT_VALUE; }
            set { _DEFAULT_VALUE = value; }
        }
    }
}
