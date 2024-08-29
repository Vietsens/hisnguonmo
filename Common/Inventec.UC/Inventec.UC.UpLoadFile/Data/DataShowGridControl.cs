using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.UpLoadFile.Data
{
    internal class DataShowGridControl
    {
        internal string _FILE_NAME;
        internal string _FILE_LENGTH;
        internal int _FILE_STATUS;

        internal DataShowGridControl()
        {

        }

        public string FILE_NAME
        {
            get { return _FILE_NAME; }
            set { _FILE_NAME = value; }
        }

        public string FILE_LENGTH
        {
            get { return _FILE_LENGTH; }
            set { _FILE_LENGTH = value; }
        }

        public int FILE_STATUS
        {
            get { return _FILE_STATUS; }
            set { _FILE_STATUS = value; }
        }
    }
}
