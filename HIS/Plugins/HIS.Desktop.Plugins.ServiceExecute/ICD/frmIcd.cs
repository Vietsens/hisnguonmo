using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Fields;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinForms.Documents.Layout;

namespace HIS.Desktop.Plugins.ServiceExecute.ICD
{
    public partial class frmIcd : Form
    {
        Inventec.Desktop.Common.Modules.Module CurrentModuleData;
        List<HIS_ICD_SKIN_PATHOLOGY> lstIcdSkinPathology = new List<HIS_ICD_SKIN_PATHOLOGY>();
        List<IcdSkinPathologyADO> lsticdSkinPathologyADO = new List<IcdSkinPathologyADO>();
        HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ(); 
        private List<string> selectedICDCodes = new List<string>();
        private bool IscheckTextIcd = false;
        public frmIcd(HIS_SERVICE_REQ serviceReq, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                this.CurrentModuleData = moduleData;
                this.currentServiceReq = serviceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisIcdSkinPathology").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisIcdSkinPathology'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.CurrentModuleData.RoomId;
                    moduleData.RoomTypeId = this.CurrentModuleData.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                        listArgs.Add(txtKeyword.Text.Trim());
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();   
                }

                lstIcdSkinPathology.Clear();    
                lsticdSkinPathologyADO.Clear();
                FillDataToGrid();
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
                

                if (lstIcdSkinPathology == null || lstIcdSkinPathology.Count <= 0)
                {
                    CommonParam param = new CommonParam();
                    HisIcdSkinPathologyFilter icd = new HisIcdSkinPathologyFilter();
                    icd.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    lstIcdSkinPathology = new BackendAdapter(param).Get<List<HIS_ICD_SKIN_PATHOLOGY>>(HisRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_GET, ApiConsumers.MosConsumer, icd, param);
                }

                if (lstIcdSkinPathology != null && lstIcdSkinPathology.Count > 0 && lsticdSkinPathologyADO.Count <= 0)
                {
                    foreach (var item in lstIcdSkinPathology)
                    {
                        IcdSkinPathologyADO ado = new IcdSkinPathologyADO();
                        ado.Id = item.ID;
                        ado.ICD_SKIN_PATHOLOGY_NAME = item.ICD_SKIN_PATHOLOGY_NAME;
                        ado.ICD_SKIN_PATHOLOGY_CODE = item.ICD_SKIN_PATHOLOGY_CODE;

                        this.lsticdSkinPathologyADO.Add(ado);
                    }
                }

                if (this.currentServiceReq.ICD_SKIN_PATHOLOGY_CODE != null && IscheckTextIcd == false)
                {
                    this.selectedICDCodes = this.currentServiceReq.ICD_SKIN_PATHOLOGY_CODE.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    foreach (var item in lsticdSkinPathologyADO)
                    {
                        if (selectedICDCodes.Contains(item.ICD_SKIN_PATHOLOGY_CODE))
                        {
                            item.IS_CHECK = true;   
                        }
                    }
                }
                IscheckTextIcd = false;

                UpdateCheckState();

                string keyword = txtKeyword.Text.Trim();
                keyword = keyword.Trim().ToLower();
                List<IcdSkinPathologyADO> lsticd = null;
                if (!String.IsNullOrEmpty(keyword))
                {
                    lsticd = lsticdSkinPathologyADO.Where(o =>
                        o.ICD_SKIN_PATHOLOGY_CODE.ToLower().Contains(keyword)
                        || o.ICD_SKIN_PATHOLOGY_NAME.ToLower().Contains(keyword)
                        ).ToList();
                }
                else
                {
                    lsticd = lsticdSkinPathologyADO;
                }


                lsticd = lsticd.OrderByDescending(x => x.IS_CHECK).ThenBy(x => x.ICD_SKIN_PATHOLOGY_CODE).ToList();
                gridControlIcd.DataSource = null;
                gridControlIcd.DataSource = lsticd;

                for (int i = 0; i < gridViewIcd.RowCount; i++)
                {
                    var row = gridViewIcd.GetRow(i) as IcdSkinPathologyADO;    
                    if (row != null && row.IS_CHECK)
                    {
                        gridViewIcd.SelectRow(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
       {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                UpdateCheckState();
                List<IcdSkinPathologyADO> lsticd = null;
                if (lsticdSkinPathologyADO != null && !String.IsNullOrEmpty(keyword.Trim()))
                {
                    lsticd = lsticdSkinPathologyADO.Where(o => o.ICD_SKIN_PATHOLOGY_CODE.ToLower().Contains(keyword.Trim().ToLower()) || (o.ICD_SKIN_PATHOLOGY_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())).Distinct().ToList();
                }
                else
                {
                    lsticd = lsticdSkinPathologyADO;
                }
                
                gridControlIcd.BeginUpdate();
                gridControlIcd.DataSource = lsticd;
                gridControlIcd.EndUpdate();

                for (int i = 0; i < gridViewIcd.RowCount; i++)
                {
                    var row = gridViewIcd.GetRow(i) as IcdSkinPathologyADO;
                    if (row != null && row.IS_CHECK)
                    {
                        gridViewIcd.SelectRow(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmIcd_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    IscheckTextIcd = true;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditCheck_Click(object sender, EventArgs e)
        {
            try
            {
                var currentCheck = (IcdSkinPathologyADO)gridViewIcd.GetFocusedRow();
                var Alls = gridViewIcd.DataSource as List<IcdSkinPathologyADO>;
                int currentIndex = 0;
                for (int i = 0; i < Alls.Count; i++)
                {
                    var item = Alls[i];
                    if (item.Id == currentCheck.Id)
                    {
                        currentIndex = i;
                        item.IS_CHECK = false;
                        item.ICD_SKIN_PATHOLOGY_CODE = null;
                        item.ICD_SKIN_PATHOLOGY_NAME = null;
                        break;
                    }
                }

                gridControlIcd.DataSource = new List<IcdSkinPathologyADO>();
                gridControlIcd.DataSource = Alls;
                gridViewIcd.FocusedRowHandle = currentIndex;

                foreach (var item in lsticdSkinPathologyADO)
                {
                    if (item.Id == currentCheck.Id)
                    {
                        item.IS_CHECK = false;
                        item.ICD_SKIN_PATHOLOGY_CODE = null;
                        item.ICD_SKIN_PATHOLOGY_NAME = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditUcCheck_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_ICD_SKIN_PATHOLOGY> lstGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD_SKIN_PATHOLOGY>();
                var currentUnCheck = (IcdSkinPathologyADO)gridViewIcd.GetFocusedRow();
                var Alls = gridControlIcd.DataSource as List<IcdSkinPathologyADO>;
                int currentIndex = 0;
                for (int i = 0; i < Alls.Count; i++)
                {
                    var item = Alls[i];
                    if (item.Id == currentUnCheck.Id)
                    {
                        currentIndex = i;
                        item.IS_CHECK = true;
                        item.ICD_SKIN_PATHOLOGY_NAME =  lstGroup.Where(o =>o.ID == currentUnCheck.Id).FirstOrDefault().ICD_SKIN_PATHOLOGY_NAME;
                        item.ICD_SKIN_PATHOLOGY_CODE = lstGroup.Where(o => o.ID == currentUnCheck.Id).FirstOrDefault().ICD_SKIN_PATHOLOGY_CODE;

                        break;
                    }
                }

                gridControlIcd.DataSource = new List<IcdSkinPathologyADO>();
                gridControlIcd.DataSource = Alls;
                gridViewIcd.FocusedRowHandle = currentIndex;

                foreach (var item in lsticdSkinPathologyADO)
                {
                    if (item.Id == currentUnCheck.Id)
                    {
                        item.IS_CHECK = true;
                        item.ICD_SKIN_PATHOLOGY_NAME = lstGroup.Where(o => o.ID == currentUnCheck.Id).FirstOrDefault().ICD_SKIN_PATHOLOGY_NAME;
                        item.ICD_SKIN_PATHOLOGY_CODE = lstGroup.Where(o => o.ID == currentUnCheck.Id).FirstOrDefault().ICD_SKIN_PATHOLOGY_CODE;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsticdSkinPathologyADO != null && lsticdSkinPathologyADO.Count > 0)
                {
                    var listCheck = lsticdSkinPathologyADO.Where(o => o.IS_CHECK).ToList();
                    if (listCheck == null || listCheck.Count <= 0)
                    {
                        if (XtraMessageBox.Show(ResourceMessage.BanChuaChonPhuongPhapNao, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                            return;
                    }

                    var selected = lsticdSkinPathologyADO.Where(x => x.IS_CHECK).Select(x => x.ICD_SKIN_PATHOLOGY_CODE).ToList();

                    this.currentServiceReq.ICD_SKIN_PATHOLOGY_CODE = string.Join(";", lsticdSkinPathologyADO.Where(x => x.IS_CHECK).Select(x => x.ICD_SKIN_PATHOLOGY_CODE).ToList());
                    this.currentServiceReq.ICD_SKIN_PATHOLOGY_NAME = string.Join(";", lsticdSkinPathologyADO.Where(x => x.IS_CHECK).Select(x => x.ICD_SKIN_PATHOLOGY_NAME).ToList());

                }

                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void gridViewIcd_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (e.ControllerRow >= 0)
            {
                var row = gridViewIcd.GetRow(e.ControllerRow) as IcdSkinPathologyADO;
                if (row != null)
                {   
                    row.IS_CHECK = gridViewIcd.IsRowSelected(e.ControllerRow);
                }
            }
            else
            {
                // Trường hợp Chọn hoặc Bỏ chọn tất cả
                for (int i = 0; i < gridViewIcd.RowCount; i++)
                {
                    var row = gridViewIcd.GetRow(i) as IcdSkinPathologyADO;
                    if (row != null)
                    {
                        row.IS_CHECK = gridViewIcd.IsRowSelected(i);
                    }
                }
            }
        }

        private void UpdateCheckState()
        {
            for (int i = 0; i < gridViewIcd.RowCount; i++)
            {
                var row = gridViewIcd.GetRow(i) as IcdSkinPathologyADO;
                if (row != null)
                {
                    var sourceRow = lsticdSkinPathologyADO.FirstOrDefault(x => x.Id == row.Id);
                    if (sourceRow != null)
                    {
                        sourceRow.IS_CHECK = gridViewIcd.IsRowSelected(i);
                    }
                }
            }
        }
    }
}
