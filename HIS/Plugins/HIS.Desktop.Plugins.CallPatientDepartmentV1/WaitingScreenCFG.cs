using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    class WaitingScreenCFG
    {
        private const string TIMER_FOR_AUTO_LOAD_WAITING_SCREEN = "EXE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS";
        private const string BACKGROUND_PARENT_COLOR_CODES = "EXE.WAITING_SCREEN.BACKGROUND_PARENT.COLOR_CODES";
        private const string GRID_NUM_ORDER_BACK_COLOR_CODES = "EXE.WAITING_SCREEN.BACK_COLOR_GRID_NUM_ORDER.COLOR_CODES";
        private const string GRID_NUM_ORDER_HEADER_BACK_COLOR_CODES = "EXE.WAITING_SCREEN.BACK_COLOR_GRID_NUM_ORDER_HEADER.COLOR_CODES";
        private const string GRID_NUM_ORDER_HEADER_FORCE_COLOR_CODES = "EXE.WAITING_SCREEN.FORCE_COLOR_GRID_NUM_ORDER_HEADER.COLOR_CODES";

        public static int TIMER_FOR_AUTO_LOAD_WAITING_SCREENS
        {
            get
            {
                return GetId(TIMER_FOR_AUTO_LOAD_WAITING_SCREEN);
            }
        }
        public static List<int> PARENT_BACK_COLOR_CODES
        {
            get
            {
                return GetIds(BACKGROUND_PARENT_COLOR_CODES);
            }
        }
        public static List<int> GRID_NUM_ORDERS_BACK_COLOR_CODES
        {
            get
            {
                return GetIds(GRID_NUM_ORDER_BACK_COLOR_CODES);
            }
        }
        public static List<int> GRID_NUM_ORDERS_HEADER_BACK_COLOR_CODES
        {
            get
            {
                return GetIds(GRID_NUM_ORDER_HEADER_BACK_COLOR_CODES);
            }
        }
        public static List<int> GRID_NUM_ORDERS_HEADER_FORCE_COLOR_CODES
        {
            get
            {
                return GetIds(GRID_NUM_ORDER_HEADER_FORCE_COLOR_CODES);
            }
        }

        private static List<int> GetIds(string code)
        {
            List<int> result = new List<int>();
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                string pattern = ",";
                Regex myRegex = new Regex(pattern);
                string[] Codes = myRegex.Split(value);
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes)
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private static int GetId(string code)
        {
            int result = 0;
            try
            {
                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                int value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(code);
                result = value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
