﻿namespace ACS.LibraryBug
{
    public partial class Bug
    {
        public string code;
        public Enum enumBC;

        private static string defaultViMessage = "ACS000";

        public Bug(Enum en)
        {
            enumBC = en;
            code = GetCode(en);
        }
    }
}
