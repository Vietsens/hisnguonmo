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

using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.SecondaryIcd.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.SecondaryIcd.Validate.ValidateRule
{
    class BenhPhuValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit maBenhPhuTxt;
        internal DevExpress.XtraEditors.TextEdit tenBenhPhuTxt;
        private string[] icdSeparators = new string[] { ";" };
        internal List<HIS_ICD> listIcd;
        internal List<V_HIS_ICD> listViewIcd;
        internal DelegateGetIcdMain getIcdMain;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && ValidLength(maBenhPhuTxt);
                valid = valid && ValidMaBenhPhu(maBenhPhuTxt);
                valid = valid && ValidIcdWrongCode(maBenhPhuTxt);
                valid = valid && ValidDuplicateWithIcdMain(getIcdMain, maBenhPhuTxt.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        bool ValidLength(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                List<string> _mess = new List<string>();
                bool _check = false;
                if ((!String.IsNullOrEmpty(maBenhPhuTxt.Text)
                    && Inventec.Common.String.CountVi.Count(maBenhPhuTxt.Text) > 500))
                {
                    _mess.Add("Mã bệnh phụ quá dài (>500 ký tự)");
                    _check = true;
                }
                if ((!String.IsNullOrEmpty(tenBenhPhuTxt.Text)
                    && Inventec.Common.String.CountVi.Count(tenBenhPhuTxt.Text) > 4000))
                {
                    _mess.Add("Tên bệnh phụ quá dài (>4000 ký tự)");
                    _check = true;
                }
                if (_check)
                {
                    this.ErrorText = string.Join(";", _mess);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }
        bool ValidMaBenhPhu(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                if (!String.IsNullOrEmpty(tenBenhPhuTxt.Text) && String.IsNullOrEmpty(maBenhPhuTxt.Text))
                {
                    this.ErrorText = "Mã icd chưa được nhập";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        bool ValidIcdWrongCode(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                if (!string.IsNullOrEmpty(maBenhPhuTxt.Text.Trim()))
                {
                    if (maBenhPhuTxt.Text.Trim().Equals(";"))
                    {
                        this.ErrorText = "Mã ICD bạn nhập không hợp lệ";
                        return false;
                    }

                    if (maBenhPhuTxt.Text.Trim().Contains(";;"))
                    {
                        this.ErrorText = "Mã ICD bạn nhập không hợp lệ";
                        return false;
                    }
                    else
                    {
                        string icdNames = "";
                        string wrongsubCodes = "";
                        bool valid = CheckIcdWrongCode(maBenhPhuTxt.Text.Trim(), ref icdNames, ref wrongsubCodes);
                        if (!valid)
                        {
                            this.ErrorText = String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, wrongsubCodes);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        bool ValidDuplicateWithIcdMain(DelegateGetIcdMain getIcdMain, string icdSubCode)
        {
            bool valid = true;
            try
            {
                if (getIcdMain != null && !String.IsNullOrEmpty(getIcdMain()) && !String.IsNullOrEmpty(icdSubCode))
                {
                    string icdmainCode = getIcdMain();
                    Inventec.Common.Logging.LogSystem.Debug("ValidDuplicateWithIcdMain. 2__icdmainCode=" + icdmainCode + ", icdSubCode=" + icdSubCode);
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = icdSubCode.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidDuplicateWithIcdMain. 3__valid=" + valid);
                        if (arrIcdExtraCodes.Any(o => o.Equals(icdmainCode)))
                        {
                            this.ErrorText = String.Format(Resources.ResourceMessage.MabenhPhuDaDuocSuDungChoMaBenhChinh, icdmainCode);
                            return false;
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("getIcdMain is null hoac icdmainCode is null" + ", icdSubCode=" + icdSubCode);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(string icdSubCode, ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(icdSubCode))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = icdSubCode.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            HIS_ICD icdByCode = null;

                            if (listIcd != null && listIcd.Count > 0)
                                icdByCode = listIcd.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.Trim().ToLower());
                            else if(listViewIcd != null && listViewIcd.Count > 0)
                            {
                                var ViewicdByCode = listViewIcd.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.Trim().ToLower());
                                icdByCode = new HIS_ICD();
                                icdByCode.ID = ViewicdByCode.ID;
                                icdByCode.ICD_CODE = ViewicdByCode.ICD_CODE;
                                icdByCode.ICD_NAME = ViewicdByCode.ICD_NAME;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Du lieu danh sach ICD null");
                            }

                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            strWrongIcdCodes = (arrWrongCodes.Count == 1 ? strWrongIcdCodes.Replace(IcdUtil.seperator, "") : strWrongIcdCodes);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
