using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
{
    internal class TroubleCache
    {
        private static List<string> troubles = new List<string>();

        internal static bool Add(string trouble)
        {
            bool result = false;
            try
            {
                troubles.Add(trouble);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static List<string> GetAndClear()
        {
            List<string> result = new List<string>();
            try
            {
                result.AddRange(troubles);
                troubles.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
