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
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TextLibrary
{
    /// <summary>
    /// "Share by account" (IS_PUBLIC_IN_ACCOUNT + PUBLIC_LOGINNAMES) of the Content Library form.
    /// </summary>
    public partial class FormTextLibrary
    {
        /// <summary>Login names the current item is shared with (kept even when the check box is off).</summary>
        List<string> shareLoginnames = new List<string>();

        /// <summary>
        /// PUBLIC_LOGINNAMES ("a,b,c") to a list: trimmed, no empty item, no duplicate.
        /// </summary>
        internal static List<string> SplitLoginnames(string loginnames)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(loginnames)) return new List<string>();
                return loginnames.Split(',').Select(o => o.Trim()).Where(o => o.Length > 0).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new List<string>();
        }

        /// <summary>
        /// New item: true. Existing item: only its creator or an admin may edit it.
        /// </summary>
        private bool CanEditCurrentTextLib()
        {
            try
            {
                if (this.ActionType != GlobalVariables.ActionEdit || this.textLib == null) return true;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                return this.textLib.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Enable state of the check box / "+" button and the "(N accounts)" label.
        /// Idempotent: called from every CheckedChanged and again after a row is loaded.
        /// </summary>
        private void UpdateShareAccountState()
        {
            try
            {
                bool canEdit = CanEditCurrentTextLib();
                // Hospital-wide sharing already covers everyone: lock the check box together with "+" so both read the same (the saved list is kept)
                chkIsShareAccount.Enabled = canEdit && !chkIsPublic.Checked;
                btnShareAccount.Enabled = chkIsShareAccount.Enabled && chkIsShareAccount.Checked;
                string format = Inventec.Common.Resource.Get.Value("FormTextLibrary.lblShareAccountCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (String.IsNullOrEmpty(format)) format = "({0})";
                lblShareAccountCount.Text = String.Format(format, this.shareLoginnames != null ? this.shareLoginnames.Count : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetShareAccount()
        {
            try
            {
                this.shareLoginnames = new List<string>();
                chkIsShareAccount.Checked = false;
                UpdateShareAccountState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadShareAccountFromRow(HIS_TEXT_LIB row)
        {
            try
            {
                this.shareLoginnames = SplitLoginnames(row != null ? row.PUBLIC_LOGINNAMES : null);
                chkIsShareAccount.Checked = row != null && row.IS_PUBLIC_IN_ACCOUNT == IS_TRUE;
                UpdateShareAccountState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataShareAccount(HIS_TEXT_LIB data)
        {
            try
            {
                data.IS_PUBLIC_IN_ACCOUNT = chkIsShareAccount.Checked ? IS_TRUE : (short)0;
                data.PUBLIC_LOGINNAMES = (this.shareLoginnames != null && this.shareLoginnames.Count > 0) ? String.Join(",", this.shareLoginnames) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Choose" on an item shared to me: return a copy showing the on-screen content, never call Update on the creator's item.
        /// </summary>
        private void ChooseWithoutSave()
        {
            try
            {
                AutoMapper.Mapper.CreateMap<HIS_TEXT_LIB, HIS_TEXT_LIB>();
                HIS_TEXT_LIB chosen = AutoMapper.Mapper.Map<HIS_TEXT_LIB, HIS_TEXT_LIB>(this.textLib);
                SetData(chosen);
                Inventec.Common.Logging.LogSystem.Debug("ChooseWithoutSave: ID=" + chosen.ID + ", CREATOR=" + chosen.CREATOR);
                if (this.dataTextLib != null)
                {
                    this.dataTextLib(chosen);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateShareAccountState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsShareAccount_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateShareAccountState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnShareAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnShareAccount.Enabled) return;
                // The creator is never listed / returned: new item = me, existing item = its CREATOR (admin case)
                string creatorLoginname = (this.ActionType == GlobalVariables.ActionEdit && this.textLib != null && !String.IsNullOrWhiteSpace(this.textLib.CREATOR))
                    ? this.textLib.CREATOR : this.creator;
                ShareAccount.frmShareAccount frm = new ShareAccount.frmShareAccount(this.currentModuleBase, new List<string>(this.shareLoginnames ?? new List<string>()), creatorLoginname, ReceiveShareLoginnames);
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReceiveShareLoginnames(object data)
        {
            try
            {
                List<string> loginnames = data as List<string>;
                if (loginnames != null)
                {
                    this.shareLoginnames = loginnames;
                    UpdateShareAccountState();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
