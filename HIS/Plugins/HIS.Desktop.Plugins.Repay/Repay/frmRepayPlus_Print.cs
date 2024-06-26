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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Repay.Repay
{
    public partial class frmRepay : DevExpress.XtraEditors.XtraForm
    {
        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menuMedicine = new DXPopupMenu();
                //TODO da ngon ngu
                DXMenuItem itemPhieuHoanUng = new DXMenuItem("In phiếu hoàn ứng", new EventHandler(OnClickInPhieuHoanUng));
                itemPhieuHoanUng.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110;
                menuMedicine.Items.Add(itemPhieuHoanUng);
                ddbPrint.DropDownControl = menuMedicine;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuHoanUng(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                var btnItem = sender as DXMenuItem;
                string type = (string)(btnItem.Tag);
                switch (type)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110:
                        InPhieuHoanUng(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (this.HisRepay == null)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong));
                    return;
                }

                var patientADO = PrintGlobalStore.getPatient(this.HisRepay.TREATMENT_ID);
                PatyAlterBhytADO patyAlterBhytADO = PrintGlobalStore.getPatyAlterBhyt(this.HisRepay.TREATMENT_ID, 0);
                string deparmentName = "";
                List<SereServGroupPlusADO> sereServNotHiTechs = new List<SereServGroupPlusADO>();

                MOS.Filter.HisSereServViewFilter sereServFilter = new MOS.Filter.HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = HisTreatment.ID;
                MOS.Filter.HisDereDetailViewFilter dereDetailFiter = new MOS.Filter.HisDereDetailViewFilter();
                dereDetailFiter.REPAY_ID = this.HisRepay.ID;
                var sereServIds = new BackendAdapter(param).Get<List<V_HIS_DERE_DETAIL>>(HisRequestUriStore.HIS_DERE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, dereDetailFiter, param).Select(o => o.SERE_SERV_ID).ToList();
                sereServFilter.IDs = sereServIds;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, param);


                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, HisTreatment.ID, param);

                long totalDay = PrintGlobalStore.GetToTalDayTreatments(HisTreatment.ID, departmentTrans);
                string departmentName="";
                //departmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentName();
              
                MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranpati = PrintGlobalStore.getTranPatiByTypeIdIn(HisTreatment.ID);

                MPS.Core.Mps000110.Mps000110RDO mps000110RDO = new MPS.Core.Mps000110.Mps000110RDO(
                    patientADO,
                    patyAlterBhytADO,
                    departmentName,
                    sereServs,
                    departmentTrans,
                    this.HisTreatment,
                    hisTranpati,
                    this.HisRepay,
                    totalDay
               );
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000110RDO, MPS.Printer.PreviewType.PrintNow);
                }
                else
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000110RDO, MPS.Printer.PreviewType.ShowDialog);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
