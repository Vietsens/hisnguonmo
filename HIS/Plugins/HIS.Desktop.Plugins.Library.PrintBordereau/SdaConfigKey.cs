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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    public class SdaConfigKey
    {
        public const string BORDEREAU_TYPE_CONFIG = "HIS.Desktop.Plugins.Library.Bordereau.BordereauType";
        public const string PRINT_DEFAULT_MPS_REPLACE = "HIS.Desktop.Plugins.Library.Bordereau.PrintDefaultMpsReplace";
        public const string POLICY_CODE_3DAY7DAY = "MOS.HIS_PACKAGE.PACKAGE_CODE.3DAY7DAY";
        public const string IS_SHOW_MEDICINE_LINE = "HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine";
        public const string IS_PRICE_WITH_DIFFERENCE = "HIS.DESKTOP.PLUGINS.LIBRARY.PRINTBORDEREAU.IS_PRICE_WITH_DIFFERENCE";
        public const string MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION = "MOS.BHYT.CALC_MATERIAL_PACKAGE_PRICE_OPTION";
        public const string IS_GROUP_REQUEST_DEPARTMENT = "HIS.Desktop.Plugins.Library.Bordereau.IsGroupReqDepartment";

        public const string CALC_ARISING_SURG_PRICE_OPTION = "MOS.BHYT.CALC_ARISING_SURG_PRICE_OPTION";

        internal const string KEY_IsPrintPrescriptionNoThread = "HIS.Desktop.IsPrintPrescriptionNoThread";

        internal const string ConfigKey_NotIncludeIsExpend = "HIS.Desktop.Plugins.Library.Bordereau.Mps.321.NotIncludeIsExpend";

        internal const string OnlyAllowPrintingIfTreatmentIsFinishedCFG = "HIS.Desktop.Plugins.Bordereau.OnlyAllowPrintingIfTreatmentIsFinished";

        internal const string QrCodeBillInfoCFG = "HIS.Desktop.Plugins.Bordereau.QRCode.BillInfo";
        internal const string QRCodeConnectInfoCFG = "HIS.Desktop.Plugins.Bordereau.QRCode.ConnectInfo";
        internal const string CreateQRCodeBillCFG = "HIS.Desktop.Plugins.Library.Bordereau.CreateQRCodeBill";
        internal const string MERGE_SERVICE_NOT_HEIN = "HIS.Desktop.Plugins.Library.Bordereau.MergeServiceNotHein";
    }
}
