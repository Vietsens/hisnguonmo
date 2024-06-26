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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Library.CacheClient;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;

using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.Plugins.InfomationExecute.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.InfomationExecute
{
    public partial class frmInfomationExecute : HIS.Desktop.Utility.FormBase
    {
        internal Module currentModule;

        long treatmentId = 0;

        HIS.Desktop.Common.DelegateSelectData _DelegateSelectData;


        List<InformationADO> lstAdo;
        List<InformationADO> lstDataSer; 
        List<string> _Content;
        bool isReturnObject = false;
        public frmInfomationExecute()
        {
            InitializeComponent();
        }
        public frmInfomationExecute(Inventec.Desktop.Common.Modules.Module currentModule, long _treatmentId, HIS.Desktop.Common.DelegateSelectData _delegateSelectData, bool returnObject, List<string> content)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = _treatmentId;
                this._DelegateSelectData = _delegateSelectData;
                this.isReturnObject = returnObject;
                this._Content = content;
                SetIconFrm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmInfomationExecute_Load(object sender, EventArgs e)
        {
            try
            {

                FillDataToGrid();

                SetCaptionByLanguageKey();



            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmInfomationExecute
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InfomationExecute.Resources.Lang", typeof(frmInfomationExecute).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmInfomationExecute.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmInfomationExecute.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmInfomationExecute.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmInfomationExecute.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmInfomationExecute.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInfomationExecute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                lstAdo = new List<InformationADO>();
                CommonParam param = new CommonParam();
                #region ListData
                HisExpMestMedicineViewFilter hemmeFilter = new HisExpMestMedicineViewFilter();
                hemmeFilter.TDL_TREATMENT_ID = treatmentId;
                var lstHisExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, hemmeFilter, param);

                if (lstHisExpMestMedicine != null && lstHisExpMestMedicine.Count > 0)
                {
                    var groupMedicine = lstHisExpMestMedicine.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.CONCENTRA }).ToList();
                    foreach (var itemMedi in groupMedicine)
                    {
                        InformationADO ado = new InformationADO(itemMedi.ToList());
                        lstAdo.Add(ado);
                    }
                }

                HisExpMestMaterialViewFilter hemmFilter = new HisExpMestMaterialViewFilter();
                hemmFilter.TDL_TREATMENT_ID = treatmentId;
                var lstHisExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, hemmFilter, param);

                if (lstHisExpMestMaterial != null && lstHisExpMestMaterial.Count > 0)
                {
                    var groupMaterial = lstHisExpMestMaterial.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID }).ToList();
                    foreach (var itemMate in groupMaterial)
                    {
                        InformationADO ado = new InformationADO(itemMate.ToList());
                        lstAdo.Add(ado);
                    }
                }

                HisExpMestBloodViewFilter hembFilter = new HisExpMestBloodViewFilter();
                hembFilter.TDL_TREATMENT_ID = treatmentId;
                var lstHisExpMestBlood = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, hembFilter, param);

                if (lstHisExpMestBlood != null && lstHisExpMestBlood.Count > 0)
                {
                    var groupBlood = lstHisExpMestBlood.GroupBy(o => new { o.BLOOD_TYPE_ID, o.VOLUME }).ToList();

                    foreach (var itemBlood in groupBlood)
                    {
                        InformationADO ado = new InformationADO(itemBlood.ToList());
                        lstAdo.Add(ado);
                    }
                }

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = treatmentId;
                ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>();
                ssFilter.TDL_SERVICE_TYPE_IDs.AddRange(new List<long>
                    {
                    
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL, 
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC, 
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, 
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                    });
                List<HIS_SERE_SERV> dataSereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
              
                #endregion
                string keyword = txtSearch.Text.Trim();

                if (dataSereServs != null && dataSereServs.Count > 0)
                {
                    dataSereServs = dataSereServs.Where(o => o.IS_DELETE == 0 && o.IS_NO_EXECUTE == null).Distinct().ToList();
                    var sereServs = (from r in dataSereServs select new InformationADO(r)).ToList();
                    lstAdo.AddRange(sereServs);
                    lstAdo = lstAdo.Where(o => o.TDL_SERVICE_NAME.ToLower().Contains(keyword.ToLower())).ToList();
                }

                gridView1.BeginUpdate();

                gridView1.GridControl.DataSource = lstAdo;

                gridView1.EndUpdate();
                gridView1.ExpandAllGroups();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Content), _Content));
             //   var lstData = gridControl1.DataSource as List<InformationADO>;
                var dt = lstAdo.OrderBy(o => o.TDL_SERVICE_TYPE_NAME).ToList();
                if (_Content != null)
                {
                    foreach (var item in _Content)
                    {
                        for (int i = 0; i < dt.Count; i++)
                        {
                            if (item.Contains(dt[i].TDL_SERVICE_NAME))
                            {
                                gridView1.SelectRow(i);
                            }
                        }
                    }
                }
             
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
               
                var lstService = new List<string>();
                var rowSelected = gridView1.GetSelectedRows();
                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    for (int i = 0; i < rowSelected.Length; i++)
                    {

                        var row = (InformationADO)gridView1.GetRow(rowSelected[i]);
                //        lstService.Add(row.TDL_SERVICE_NAME);
                        _Content.Add(row.TDL_SERVICE_NAME);
                    }
                }               
                FillDataToGrid();           
                //var dt = lstAdo.OrderBy(o => o.TDL_SERVICE_TYPE_NAME).ToList();
                //for (int i = 0; i < lstService.Count; i++)
                //{
                //    for (int j = 0; j < dt.Count; j++)
                //    {
                //        if (dt[j].TDL_SERVICE_NAME.Equals(lstService[i]))
                //        {
                           
                //             gridView1.SelectRow(j);
                //             _Content.Add(dt[j].TDL_SERVICE_NAME);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lst = new List<string>();
                var rowSelected = gridView1.GetSelectedRows();
                if (rowSelected != null && rowSelected.Count() > 0)
                {
                    foreach (var i in rowSelected)
                    {
                        var row = (InformationADO)gridView1.GetRow(i);

                        if (row.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            lst.Add(string.Format("{0} - hàm lượng: {1} - số lượng: {2} - đơn vị tính: {3} - HDSD: {4}",
                                row.TDL_SERVICE_NAME,
                                row.CONCENTRA,
                                row.AMOUNT,
                                row.SERVICE_UNIT_NAME,
                                row.TUTORIAL
                                ));
                        }
                        else if (row.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            lst.Add(string.Format("{0} - số lượng: {1} - đơn vị tính: {2}",
                                row.TDL_SERVICE_NAME,
                                row.AMOUNT,
                                row.SERVICE_UNIT_NAME
                                ));
                        }
                        else if (row.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                        {
                            lst.Add(string.Format("{0} - dung tích: {1} - số lượng: {2}",
                               row.TDL_SERVICE_NAME,
                               row.VOLUME,
                               row.AMOUNT
                               ));
                        }
                        else
                        {
                            lst.Add(string.Format("{0} ",
                               row.TDL_SERVICE_NAME
                               ));
                        }

                    }
                    

                        this._DelegateSelectData(string.Join("; ", lst.Distinct().ToList()));          
                }
                else
                {
                    this._DelegateSelectData("");
                }
                this.Close();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
