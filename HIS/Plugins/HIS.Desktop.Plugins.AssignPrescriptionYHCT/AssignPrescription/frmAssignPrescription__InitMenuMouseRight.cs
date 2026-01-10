using DevExpress.XtraBars;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal PopupMenu menu;

        public enum MOUSE_RIGHT_TYPE
        {
            INFORMATION,
            INFORMATION_EVALUATION
        }

        private void InitMenu()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("InitMenu.1");
                if (menu == null)
                    menu = new PopupMenu(barManager1);

                menu.ItemLinks.Clear();
                var selectedItemsForMenu = GetMediMatySelected();
                if (selectedItemsForMenu == null || selectedItemsForMenu.Count == 0)
                    return;

                if (HisConfigCFG.ConnectDrugInterventionInfo == "2")
                {
                    if (selectedItemsForMenu.Count == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Info("InitMenu.5");
                        BarButtonItem itemInformation = new BarButtonItem(barManager1, ResourceMessage.ThongTinThuoc, 1);
                        itemInformation.Tag = MOUSE_RIGHT_TYPE.INFORMATION;
                        itemInformation.ItemClick += new ItemClickEventHandler(setProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemInformation });
                    }

                    if (selectedItemsForMenu.Count > 1)
                    {
                        Inventec.Common.Logging.LogSystem.Info("InitMenu.6");
                        BarButtonItem itemInformationEvaluation = new BarButtonItem(barManager1, ResourceMessage.DanhGiaThongTinThuoc, 1);
                        itemInformationEvaluation.Tag = MOUSE_RIGHT_TYPE.INFORMATION_EVALUATION;
                        itemInformationEvaluation.ItemClick += new ItemClickEventHandler(setProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemInformationEvaluation });
                    }
                }

                if (menu.ItemLinks.Count > 0)
                    menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void setProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                var btn = e.Item as BarButtonItem;
                MOUSE_RIGHT_TYPE processType = (MOUSE_RIGHT_TYPE)btn.Tag;
                switch (processType)
                {
                    case MOUSE_RIGHT_TYPE.INFORMATION:
                        List<MediMatyTypeADO> MediMatyTypeInformation = this.GetMediMatySelected();
                        if (MediMatyTypeInformation != null && MediMatyTypeInformation.Count > 0)
                        {
                            var service = new HIS.Desktop.MIMS.Integration.Modules.DrugInfomationService();
                            MimsDrugType mimsDrugType = new MimsDrugType();
                            switch (MediMatyTypeInformation.FirstOrDefault().MIMS_TYPE)
                            {
                                case 1:
                                    mimsDrugType = MimsDrugType.GGPI;
                                    break;
                                case 2:
                                    mimsDrugType = MimsDrugType.Product;
                                    break;
                                case 3:
                                    mimsDrugType = MimsDrugType.GenericItem;
                                    break;
                                default:
                                    mimsDrugType = MimsDrugType.GenericItem;
                                    break;
                            }
                            service.ShowResultAsync(new HIS.Desktop.MIMS.Integration.Models.DrugItem(MediMatyTypeInformation.FirstOrDefault().MEDICINE_TYPE_CODE, null, null, mimsDrugType));
                        }
                        break;
                    case MOUSE_RIGHT_TYPE.INFORMATION_EVALUATION:
                        List<MediMatyTypeADO> MediMatyTypeInformationEvluation = this.GetMediMatySelected();
                        if (MediMatyTypeInformationEvluation != null && MediMatyTypeInformationEvluation.Count > 0)
                        {
                            List<HIS.Desktop.MIMS.Integration.Models.DrugItem> lstDrugItem = new List<MIMS.Integration.Models.DrugItem>();
                            var service = new HIS.Desktop.MIMS.Integration.Modules.DrugHealthService();

                            foreach (var item in MediMatyTypeInformationEvluation)
                            {
                                MimsDrugType mimsDrugType = new MimsDrugType();
                                switch (item.MIMS_TYPE)
                                {
                                    case 1:
                                        mimsDrugType = MimsDrugType.GGPI;
                                        break;
                                    case 2:
                                        mimsDrugType = MimsDrugType.Product;
                                        break;
                                    case 3:
                                        mimsDrugType = MimsDrugType.GenericItem;
                                        break;
                                    default:
                                        mimsDrugType = MimsDrugType.GenericItem;
                                        break;
                                }
                                HIS.Desktop.MIMS.Integration.Models.DrugItem drugItem = new HIS.Desktop.MIMS.Integration.Models.DrugItem(item.MEDICINE_TYPE_CODE, null, null, mimsDrugType);
                                lstDrugItem.Add(drugItem);
                            }

                            List<string> lstICD = new List<string>();
                            var icdValue = (IcdInputADO)this.icdProcessor.GetValue(this.ucIcd);
                            lstICD.Add(icdValue.ToString());
                            var icdValueSecond = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                            if (!string.IsNullOrWhiteSpace(icdValueSecond.ICD_SUB_CODE))
                            {
                                lstICD.AddRange(icdValueSecond.ICD_SUB_CODE.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
                            }
                            service.ShowResultAsync(lstDrugItem, lstICD);
                        }
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

        private List<MediMatyTypeADO> GetMediMatySelected()
        {
            List<MediMatyTypeADO> result = new List<MediMatyTypeADO>();
            try
            {
                int[] selectRows = gridViewServiceProcess.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var mediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetRow(selectRows[i]);
                        result.Add(mediMatyTypeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
