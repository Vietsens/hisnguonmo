using DevExpress.XtraEditors.Controls;
using Inventec.UC.ComboCommune.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Design.Template1
{
    public partial class Template1
    {
        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = listData.Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();

                cboPhuongXa.Properties.DataSource = listResult;
                cboPhuongXa.Properties.DisplayMember = "COMMUNE_NAME";
                cboPhuongXa.Properties.ValueMember = "COMMUNE_CODE";
                cboPhuongXa.Properties.ForceInitialize();

                cboPhuongXa.Properties.Columns.Clear();
                cboPhuongXa.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_CODE", "", 100));
                cboPhuongXa.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_NAME", "", 200));

                cboPhuongXa.Properties.ShowHeader = false;
                cboPhuongXa.Properties.ImmediatePopup = true;
                cboPhuongXa.Properties.DropDownRows = 20;
                cboPhuongXa.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboPhuongXa.EditValue = null;
                    txtMaPhuongXa.Text = "";
                    cboPhuongXa.Focus();
                    cboPhuongXa.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(cboPhuongXa);
                }
                else
                {
                    if (listResult.Count == 1)
                    {                        
                        cboPhuongXa.EditValue = listResult[0].COMMUNE_CODE;
                        txtMaPhuongXa.Text = listResult[0].COMMUNE_CODE;
                        if (_SetValueTHX != null) _SetValueTHX(listResult[0]);
                        //cboTHX.EditValue = listResult[0].SEARCH_CODE;
                        //txtMaTHX.Text = listResult[0].SEARCH_CODE;
                        if (isExpand)
                        {
                            if (_FocusControlNext != null) _FocusControlNext();
                            //txtMaNgheNghiep.Focus();
                            //txtMaNgheNghiep.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboPhuongXa.EditValue = null;
                        cboPhuongXa.Focus();
                        cboPhuongXa.ShowPopup();
                        PopupProcess.SelectFirstRowPopup(cboPhuongXa);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
