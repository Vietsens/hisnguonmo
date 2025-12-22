using DevExpress.XtraEditors;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Helper Methods - Shared

        private short? GetRadioGroupValue(string groupName)
        {
            try
            {
                if (radioGroups != null && radioGroups.ContainsKey(groupName))
                {
                    var checkedItem = radioGroups[groupName].FirstOrDefault(c => c.Checked);
                    if (checkedItem != null)
                    {
                        int index = radioGroups[groupName].IndexOf(checkedItem);
                        return (short)index;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void SetRadioGroupValue(string groupName, short? value)
        {
            try
            {
                if (radioGroups != null && radioGroups.ContainsKey(groupName))
                {
                    foreach (var check in radioGroups[groupName])
                    {
                        check.CheckedChanged -= RadioCheck_CheckedChanged;
                        check.Checked = false;
                        check.CheckedChanged += RadioCheck_CheckedChanged;
                    }

                    if (value.HasValue && value.Value >= 0 && value.Value < radioGroups[groupName].Count)
                    {
                        radioGroups[groupName][value.Value].CheckedChanged -= RadioCheck_CheckedChanged;
                        radioGroups[groupName][value.Value].Checked = true;
                        radioGroups[groupName][value.Value].CheckedChanged += RadioCheck_CheckedChanged;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetComboValue(GridLookUpEdit combo)
        {
            try
            {
                if (combo != null && combo.EditValue != null)
                {
                    string value = combo.EditValue.ToString();
                    // Trả về null nếu là chuỗi rỗng hoặc chỉ có khoảng trắng
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return null;
                    }
                    return value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void SetComboValue(GridLookUpEdit combo, string code)
        {
            try
            {
                if (combo != null)
                {
                    // Nếu code null hoặc chuỗi rỗng thì set EditValue = null
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        combo.EditValue = null;
                    }
                    else
                    {
                        combo.EditValue = code;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetSpinEditStringValue(SpinEdit spinEdit)
        {
            try
            {
                if (spinEdit != null && spinEdit.EditValue != null && spinEdit.Value != 0)
                {
                    return spinEdit.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void SetSpinEditStringValue(SpinEdit spinEdit, string value)
        {
            try
            {
                if (spinEdit != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        spinEdit.EditValue = null;
                    }
                    else
                    {
                        decimal decimalValue;
                        if (decimal.TryParse(value, out decimalValue))
                        {
                            spinEdit.EditValue = decimalValue;
                        }
                        else
                        {
                            spinEdit.EditValue = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? ConvertDateToTimeNumber(object dateValue, bool isDateOnly = true)
        {
            try
            {
                if (dateValue != null && dateValue is DateTime)
                {
                    DateTime dt = (DateTime)dateValue;
                    if (isDateOnly)
                    {
                        return long.Parse(dt.ToString("yyyyMMdd") + "000000");
                    }
                    else
                    {
                        return long.Parse(dt.ToString("yyyyMMddHHmmss"));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private DateTime? ConvertTimeNumberToDate(long timeNumber)
        {
            try
            {
                return Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private string GetUserNameByLoginName(string loginname)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(loginname))
                {
                    var user = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>()
                        .FirstOrDefault(o => o.LOGINNAME == loginname);
                    if (user != null)
                    {
                        return user.TDL_USERNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private string GetEthnicNameByCode(string code)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var ethnic = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA_ETHNIC>()
                        .FirstOrDefault(o => o.ETHNIC_CODE == code);
                    return ethnic != null ? ethnic.ETHNIC_NAME : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private string GetTextEditValue(TextEdit textEdit)
        {
            try
            {
                if (textEdit != null && !string.IsNullOrWhiteSpace(textEdit.Text))
                {
                    return textEdit.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void SetTextEditValue(TextEdit textEdit, string value)
        {
            try
            {
                if (textEdit != null)
                {
                    textEdit.Text = value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
