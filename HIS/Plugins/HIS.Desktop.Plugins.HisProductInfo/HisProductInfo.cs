using DevExpress.XtraEditors;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.HisProductInfo
{
    public partial class HisProductInfo : FormBase
    {
        Inventec.Desktop.Common.Modules.Module module = null;
        ProductInfoADO data = null;
        UCDocument ucDocument = new UCDocument();
        HIS_PRODUCT_INFO currentProductInfo = null;
        public HisProductInfo(Inventec.Desktop.Common.Modules.Module _module, ProductInfoADO ado)
            :base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.data = ado;
        }

        private void HisProductInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitDocument();
                SetDefaultValue();
                LoadDatatoControl();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisProductInfo.Resources.Lang", typeof(HisProductInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisProductInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("HisProductInfo.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("HisProductInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //uc

                //this.Text = Inventec.Common.Resource.Get.Value("HisProductInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if(this.module != null)
                {
                    this.Text = this.module.text.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitDocument()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.ucDocument != null)
                {
                    this.ucDocument.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.ucDocument);
                    this.ucDocument.SetFont(new Font("Times New Roman", 14));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                this.ucDocument.SetRtfText("");
                this.ucDocument.SetText("");
                this.ucDocument.AllowEdit(data != null && data.ProductInfoOpen == 1);
                this.btnSave.Enabled = data != null && data.ProductInfoOpen == 1;
                
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDatatoControl()
        {
            try
            {
                CommonParam param = new CommonParam();

                if (data != null && data.MedicineTypeId > 0)
                {
                    MOS.Filter.HisProductInfoFilter filter = new MOS.Filter.HisProductInfoFilter();
                    filter.MEDICINE_TYPE_ID = data.MedicineTypeId;
                    var rs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PRODUCT_INFO>>("api/HisProductInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (rs != null && rs.Count > 0)
                    {
                        this.currentProductInfo = rs.FirstOrDefault();
                        this.ucDocument.SetRtfText(rs.FirstOrDefault().PRODUCT_INFO);
                        this.ucDocument.SetFont(new Font("Times New Roman", 14));
                        
                    }
                }
                EnableControlChange();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void EnableControlChange()
        {
            try
            {
                if(this.data != null && this.data.ProductInfoOpen == 1 && this.currentProductInfo != null)
                {
                    btnDelete.Enabled = true;
                }
                if(this.data != null && (this.data.ProductInfoOpen == 0 || this.currentProductInfo == null))
                {
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                PrcocessSave();
                WaitingManager.Hide();
                
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void PrcocessSave()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                LogSystem.Debug("ProcessSave -> 1");
                if(this.currentProductInfo != null)
                {
                    //update
                    HIS_PRODUCT_INFO updateData = this.currentProductInfo;
                    updateData.PRODUCT_INFO = this.ucDocument.getRtfText();
                    LogSystem.Debug("du lieu gui len API update. " + LogUtil.TraceData("updateData", updateData));
                    var rs = new BackendAdapter(param).Post<HIS_PRODUCT_INFO>("/api/HisProductInfo/Update", ApiConsumers.MosConsumer, updateData, param);
                    if(rs != null)
                    {
                        success = true;
                        this.currentProductInfo = rs;
                        
                        EnableControlChange();
                    }
                    
                }
                else
                {
                    //create
                    HIS_PRODUCT_INFO createData = new HIS_PRODUCT_INFO();
                    createData.MEDICINE_TYPE_ID = this.data.MedicineTypeId;
                    createData.PRODUCT_INFO = this.ucDocument.getRtfText();
                    LogSystem.Debug("du lieu gui len API create. " + LogUtil.TraceData("createData", createData));
                    var rs = new BackendAdapter(param).Post<HIS_PRODUCT_INFO>("api/HisProductInfo/Create", ApiConsumers.MosConsumer, createData, param);
                    if(rs != null)
                    {
                        success = true;
                        this.currentProductInfo = rs;
                        EnableControlChange();
                    }
                }
                LogSystem.Debug("ProcessSave -> end");
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if(MessageBox.Show(this,"Bạn có chắc chắn muốn xóa dữ liệu?","Thông báo",MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    LogSystem.Debug("Goi den api xoa du lieu: ID = " + this.currentProductInfo.ID);
                    bool rs = new BackendAdapter(param).Post<bool>("api/HisProductInfo/Delete", ApiConsumers.MosConsumer, this.currentProductInfo.ID, param);
                    MessageManager.Show(this, param, rs);
                    if (rs)
                    {
                        this.currentProductInfo = null;
                        SetDefaultValue();
                        EnableControlChange();
                        
                    }
                }
                
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void HisProductInfo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.Control && e.KeyCode == Keys.S)
                {
                    btnSave.PerformClick();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
