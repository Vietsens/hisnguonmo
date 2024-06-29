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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MaterialUseCount
{
    public partial class frmMaterialUseCount : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module _Module;

        public frmMaterialUseCount()
        {
            InitializeComponent();
        }

        public frmMaterialUseCount(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this._Module = currentModule;
                SetIconFrm();
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<long> LIST_EXPMESTTYPEID = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM
        };

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

        V_HIS_MEDI_STOCK _MediStock { get; set; }
        private void frmMaterialUseCount_Load(object sender, EventArgs e)
        {
            try
            {
                this._DataADOs = new List<DataADO>();

                _MediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this._Module.RoomId);
                if (_MediStock == null)
                {
                    this.Enabled = false;
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không phải là kho", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoSeri_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtSoSeri.Text))
                    {
                        Process(txtSoSeri.Text.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST_MATERIAL _expMestMaterial { get; set; }
        public long? _MAX_REUSE_COUNT { get; set; }
        long _REUSE_COUNT { get; set; }
        private List<V_HIS_EXP_MEST_MATERIAL> ListData = new List<V_HIS_EXP_MEST_MATERIAL>();
        private void Process(string _seriNumber)
        {
            try
            {
                ListData = new List<V_HIS_EXP_MEST_MATERIAL>();
                this._expMestMaterial = new V_HIS_EXP_MEST_MATERIAL();
                this._MAX_REUSE_COUNT = 0;
                this._REUSE_COUNT = 0;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter filter = new MOS.Filter.HisExpMestMaterialViewFilter();
                filter.SERIAL_NUMBER__EXACT = _seriNumber;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //filter.MEDI_STOCK_ID = _MediStock.ID;
                filter.IS_EXPORT = true;

                //filter.EXP_MEST_TYPE_IDs = new List<long>();
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK);
                //filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT);

                var datas = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, filter, param);

                 foreach (var req in datas)
                 {
                     if (LIST_EXPMESTTYPEID.Contains(req.EXP_MEST_TYPE_ID))
                     {
                         ListData.Add(req);
                     }
                 }


                 if (ListData != null)
                {
                    //this._expMestMaterial = ListData.OrderByDescending(item => item.IS_EXPORT).FirstOrDefault();
                    //var maxExpTime = ListData.Max(item => item.EXP_TIME);
                    this._expMestMaterial = ListData.OrderByDescending(item => item.EXP_TIME)
                                          .ThenByDescending(item => item.ID)
                                          .FirstOrDefault();
                    this._REUSE_COUNT = ListData.Count();

                    MOS.Filter.HisMaterialFilter filterMaterial = new MOS.Filter.HisMaterialFilter();
                    filterMaterial.ID = this._expMestMaterial.MATERIAL_ID;
                    filterMaterial.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var dataMaterials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, filterMaterial, param);
                    if (dataMaterials != null && dataMaterials.Count > 0)
                    {
                        _MAX_REUSE_COUNT = dataMaterials.FirstOrDefault().MAX_REUSE_COUNT;
                    }

                    if (_MAX_REUSE_COUNT <= 0)
                    {
                        var dataMaterialTYpe = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this._expMestMaterial.MATERIAL_TYPE_ID);
                        _MAX_REUSE_COUNT = dataMaterialTYpe != null ? dataMaterialTYpe.MAX_REUSE_COUNT : 0;
                    }
                }

                FillDataToControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                if (this._expMestMaterial != null && this._expMestMaterial.ID > 0)
                {
                    txtMa.Text = this._expMestMaterial.MATERIAL_TYPE_CODE;
                    txtTen.Text = this._expMestMaterial.MATERIAL_TYPE_NAME;
                    txtToiDa.Text = this._MAX_REUSE_COUNT.ToString();
                    txtDaDung.Text = this._REUSE_COUNT.ToString();
                    long value = (this._MAX_REUSE_COUNT ?? 0) - this._REUSE_COUNT;
                    spinConLai.EditValue = value > 0 ? value : 0;
                }
                else
                {
                    txtMa.Text = "";
                    txtTen.Text = "";
                    txtToiDa.Text = "";
                    txtDaDung.Text = "";
                    spinConLai.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //gridControlData.DataSource = null;
                //var data = (DataADO)gridViewData.GetFocusedRow();
                //if (data != null && this._DataADOs != null && this._DataADOs.Count > 0)
                //{
                //    this._DataADOs.Remove(data);
                //}
                //gridControlData.DataSource = this._DataADOs;
                var data = (DataADO)gridViewData.GetFocusedRow();
                if (data != null && this._DataADOs != null && this._DataADOs.Count > 0)
                {
                    gridViewData.DeleteRow(gridViewData.FocusedRowHandle);
                    this._DataADOs.Remove(data);
                }
                gridControlData.DataSource = this._DataADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this._DataADOs = new List<DataADO>();
                this._expMestMaterial = null;
                gridControlData.DataSource = null;
                txtSoSeri.Text = "";
                txtMa.Text = "";
                txtTen.Text = "";
                txtToiDa.Text = "";
                txtDaDung.Text = "";
                spinConLai.EditValue = null;
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<DataADO> _DataADOs { get; set; }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                gridControlData.DataSource = null;
                if (this._expMestMaterial != null && this._expMestMaterial.ID > 0)
                {
                    DataADO ado = new DataADO(this._expMestMaterial);
                    var data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._expMestMaterial.MEDI_STOCK_ID);
                    ado.MEDI_STOCK_NAME = data != null ? data.MEDI_STOCK_NAME : "";
                    ado.REUSE_COUNT_STR = this.spinConLai.Value.ToString() + "/" + this._MAX_REUSE_COUNT;
                    ado.ReusCount = Inventec.Common.TypeConvert.Parse.ToInt64(this.spinConLai.Value.ToString());
                    ado.VAT_RATIO_STR = (this._expMestMaterial.VAT_RATIO * 100) + "";
                    //ado.EXP_TIME = data != null ? data.MEDI_STOCK_NAME : "";
                    _DataADOs.Add(ado);
                }

                gridControlData.DataSource = _DataADOs;

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
                if (this._DataADOs != null && this._DataADOs.Count > 0)
                {
                    WaitingManager.Show();
                    HisImpMestReuseSDO SDO = new HisImpMestReuseSDO();
                    SDO.MaterialReuseSDOs = new List<ImpMestMaterialReusableSDO>();
                    SDO.MediStockId = _MediStock.ID;
                    SDO.RequestRoomId = this._Module.RoomId;
                    foreach (var item in this._DataADOs)
                    {
                        ImpMestMaterialReusableSDO ado = new ImpMestMaterialReusableSDO();
                        ado.MaterialId = item.MATERIAL_ID ?? 0;
                        ado.SerialNumber = item.SERIAL_NUMBER;
                        ado.ReusCount = item.ReusCount;

                        SDO.MaterialReuseSDOs.Add(ado); 
                    }

                    bool success = false;
                    CommonParam param = new CommonParam();

                    var outPut = new BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/ReusableCreate", ApiConsumers.MosConsumer, SDO, param);
                    if (outPut != null)
                    {
                        success = true;
                        btnSave.Enabled = false;
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Add_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Saveee_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem__Neww_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
