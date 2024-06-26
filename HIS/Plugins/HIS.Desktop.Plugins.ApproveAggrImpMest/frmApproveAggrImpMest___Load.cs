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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ApproveAggrImpMest.ADO;
using HIS.Desktop.Plugins.ApproveAggrImpMest.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveAggrImpMest
{
    public partial class frmApproveAggrImpMest : FormBase
    {
        private void LoadImpMest()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestView2Filter filter = new HisImpMestView2Filter();
                filter.ID = this.impMestId;
                this.impMest = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (this.impMest == null)
                    throw new Exception("impMest = null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediStockFromRoomId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediStockFilter filter = new HisMediStockFilter();
                filter.ROOM_ID = this.moduleData.RoomId;
                this.mediStock = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (this.mediStock == null)
                    throw new Exception("mediStock is null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                if (this.impMest != null)
                {
                    lblImpMestCode.Text = this.impMest.IMP_MEST_CODE;
                    lblStatus.Text = this.impMest.IMP_MEST_STT_NAME;
                    lblMediImpStock.Text = this.impMest.MEDI_STOCK_NAME;
                    lblUserRequest.Text = this.impMest.REQ_USERNAME;
                    lblDepartmentReq.Text = this.impMest.REQ_DEPARTMENT_NAME;
                    lblTimeReq.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.impMest.CREATE_TIME ?? 0);
                    lblTimeApp.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.impMest.APPROVAL_TIME ?? 0);
                    lblTimeImp.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.impMest.IMP_TIME ?? 0);
                    IntEnabledButton();
                }

                if (this.impMestDetails != null && this.impMestDetails.Count > 0)
                {
                    gridControlImpMestChild.DataSource = this.impMestDetails;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void IntEnabledButton()
        {
            try
            {
                if (this.impMest != null)
                {
                    if (this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                    {
                        btnAppro.Enabled = false;
                    }

                    if (this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                    {
                        btnAppro.Enabled = false;
                        btnImp.Enabled = false;
                    }
                }

                if (!this.CheckMediStockCurrent())
                {
                    btnAppro.Enabled = false;
                    btnImp.Enabled = false;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckMediStockCurrent()
        {
            bool result = true;
            try
            {
                if (this.impMest != null)
                {
                    if (this.mediStock == null || impMest.MEDI_STOCK_ID != mediStock.ID)
                        result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadImpMestDetail()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestView2Filter filter = new HisImpMestView2Filter();
                filter.AGGR_IMP_MEST_ID = this.impMestId;
                var impMestDetailApis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>("api/HisImpMest/GetView2", ApiConsumers.MosConsumer, filter, param);
                this.impMestDetails = new List<ImpMestSDO>();

                foreach (var item in impMestDetailApis)
                {
                    ImpMestSDO impMestSDO = new ImpMestSDO(item);
                    this.impMestDetails.Add(impMestSDO);
                }

                if (this.impMestDetails == null)
                    throw new Exception("impMestDetails = null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadImpMestAndImpMestDetail()
        {
            try
            {
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(LoadImpMest);
                methods.Add(LoadImpMestDetail);
                methods.Add(LoadMediStockFromRoomId);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadImpMestMedicineAndMaterial()
        {
            try
            {
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(LoadImpMestMedicine);
                methods.Add(LoadImpMestMaterial);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadImpMestMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.impMestDetails != null && this.impMestDetails.Count > 0)
                {
                    HisImpMestMedicineViewFilter filter = new HisImpMestMedicineViewFilter();
                    filter.IMP_MEST_IDs = this.impMestDetails.Select(o => o.ID).ToList();
                    this.impMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>>(UriRequestStore.IMP_MEST_MEDICINE__GETVIEW, ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadImpMestMaterial()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.impMestDetails != null && this.impMestDetails.Count > 0)
                {
                    HisImpMestMaterialViewFilter filter = new HisImpMestMaterialViewFilter();
                    filter.IMP_MEST_IDs = this.impMestDetails.Select(o => o.ID).ToList();
                    this.impMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL>>(UriRequestStore.IMP_MEST_MATERIAL__GETVIEW, ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridMediMate(List<ImpMestSDO> impMestCheckeds)
        {
            try
            {
                if (impMestCheckeds == null || impMestCheckeds.Count == 0)
                {
                    gridControlMediMate.DataSource = null;
                    return;
                }
                ImpMestMediMateADOs = new List<ImpMestMediMateADO>();
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicineTemps = new List<V_HIS_IMP_MEST_MEDICINE>();
                List<V_HIS_IMP_MEST_MATERIAL> impMestMaterialTemps = new List<V_HIS_IMP_MEST_MATERIAL>();

                List<long> impMestIds = impMestCheckeds.Select(o => o.ID).ToList();
                if (this.impMestMedicines != null && this.impMestMedicines.Count > 0)
                {
                    impMestMedicineTemps = this.impMestMedicines.Where(o => impMestIds.Contains(o.IMP_MEST_ID)).ToList();
                    if (impMestMedicineTemps != null && impMestMedicineTemps.Count > 0)
                    {
                        ImpMestMediMateADOs.AddRange(from r in impMestMedicineTemps select new ImpMestMediMateADO(r));
                    }
                }

                if (this.impMestMaterials != null && this.impMestMaterials.Count > 0)
                {
                    impMestMaterialTemps = this.impMestMaterials.Where(o => impMestIds.Contains(o.IMP_MEST_ID)).ToList();
                    if (impMestMaterialTemps != null && impMestMaterialTemps.Count > 0)
                    {
                        ImpMestMediMateADOs.AddRange(from r in impMestMaterialTemps select new ImpMestMediMateADO(r));
                    }
                }

                List<ImpMestMediMateADO> ImpMestMediMateADOTemps = new List<ImpMestMediMateADO>();
                if (toggleSMediMateType.IsOn)
                {
                    var impMestMediMateADOGroups = ImpMestMediMateADOs.GroupBy(o => new { o.MEDI_MATE_TYPE_ID, o.IsMedicine, o.IsMaterial });
                    foreach (var impMestMediMateADOGroup in impMestMediMateADOGroups)
                    {
                        ImpMestMediMateADO impMestMediMateADO = impMestMediMateADOGroup.First();
                        impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);

                        string packageNumber = "";
                        foreach (var item in impMestMediMateADOGroup.ToList())
                        {
                            if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER) && !packageNumber.Contains(item.PACKAGE_NUMBER))
                                packageNumber += (item.PACKAGE_NUMBER + (impMestMediMateADOGroup.ToList().IndexOf(item) < (impMestMediMateADOGroup.ToList().Count - 1) ? ", " : ""));
                        }
                        impMestMediMateADO.PACKAGE_NUMBER = packageNumber;
                        ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                    }
                }
                else
                {
                    var impMestMediMateADOGroups = ImpMestMediMateADOs.GroupBy(o => new { o.MEDI_MATE_ID, o.IsMedicine, o.IsMaterial });
                    foreach (var impMestMediMateADOGroup in impMestMediMateADOGroups)
                    {
                        ImpMestMediMateADO impMestMediMateADO = impMestMediMateADOGroup.First();
                        impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                        ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                    }
                }

                //Thuốc --> Vật tư
                //NUM_ORDER  trong HIS_MEDICINE_TYPE/HIS_MATERIAL_TYPE
                ImpMestMediMateADOTemps = ImpMestMediMateADOTemps.OrderBy(o => o.TYPE).ThenBy(o => o.numOrder).ThenBy(o => o.MEDI_MATE_TYPE_NAME).ToList();

                gridControlMediMate.DataSource = ImpMestMediMateADOTemps;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
