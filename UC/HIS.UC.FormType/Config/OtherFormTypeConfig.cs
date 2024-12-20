using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class OtherFormTypeConfig
    {
        private static List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> heinLiveAreas;
        public static List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> HeinLiveAreas
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData));
                if (heinLiveAreas == null || heinLiveAreas.Count == 0)
                {
                    heinLiveAreas = MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.Get();
                }
                return heinLiveAreas;
            }
            set
            {
                heinLiveAreas = value;
            }
        }

        private static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> rightRouteTypes;
        public static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData));
                if (rightRouteTypes == null || rightRouteTypes.Count == 0)
                {
                    rightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                }
                return rightRouteTypes;
            }
            set
            {
                rightRouteTypes = value;
            }
        }
    }
}
