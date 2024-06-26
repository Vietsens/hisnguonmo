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
using Inventec.Core;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Patient;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using TYT.EFMODEL.DataModels;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.Patient
{
    public partial class UCListPatient : UserControlBase
    {
        void PatientMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this._PatientShowPopup != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PatientPopupMenuProcessor.ModuleType type = (PatientPopupMenuProcessor.ModuleType)(e.Item.Tag);
                    switch (type)
                    {
                        case PatientPopupMenuProcessor.ModuleType.ScnPersonalHealth:
                            btnScnPersonalHealthClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.ScnAccidentHurt:
                            btnScnAccidentHurtClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.PatientUpdateExt:
                            PatientUpdateExtClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.PatientProgram:
                            btnCreatePatientProgram_ButtonClick(null, null);
                            break;
                        case PatientPopupMenuProcessor.ModuleType.ScnNutrition:
                            btnScnNutritionClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.ScnDeath:
                            btnScnDeathClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.EvenLog:
                            Btn_EvenLog_ButtonClick(null, null);
                            break;
                        case PatientPopupMenuProcessor.ModuleType.PatientInfo:
                            Btn_PatientInfo_ButtonClick(null, null);
                            break;
                        case PatientPopupMenuProcessor.ModuleType.FamilyInformation:
                            Btn_FamilyInformation_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.BenhNhanTamThan:
                            Btn_BenhNhanTamThan_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.KhamThai:
                            Btn_KhamThai_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.KeHoachHoa:
                            Btn_KeHoachHoa_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.PhaThai:
                            Btn_PhaThai_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.BenhNhanHIV:
                            Btn_BenhNhanHIV_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.TYTTuVong:
                            Btn_TytTuVong_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.TYTSotRet:
                            Btn_TytSotRet_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.TYTSinhDe:
                            Btn_SinhDe_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.BenhNhanLao:
                            Btn_TytLao_ButtonClick();
                            break;
                        case PatientPopupMenuProcessor.ModuleType.TheDiUng:
                            Btn_TheDiUng_ButtonClick();
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thong tin tien su suc khoe
        /// </summary>
        private void btnScnPersonalHealthClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this._PatientShowPopup);
                    CallModule callModule = new CallModule(CallModule.ScnPersonalHealth, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientUpdateExtClick()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(_PatientShowPopup.ID);
                CallModule callModule = new CallModule(CallModule.UpdatePatientExt, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tai nan thuong tich
        /// </summary>
        private void btnScnAccidentHurtClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup.PERSON_CODE);
                    CallModule callModule = new CallModule(CallModule.ScnAccidentHurt, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dinh duong
        /// </summary>
        private void btnScnNutritionClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup.PERSON_CODE);
                    CallModule callModule = new CallModule(CallModule.ScnNutrition, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dinh duong
        /// </summary>
        private void btnScnDeathClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup.PERSON_CODE);
                    CallModule callModule = new CallModule(CallModule.ScnDeath, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cap Nhat Bo me
        /// </summary>
        private void Btn_FamilyInformation_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup.PERSON_CODE);
                    CallModule callModule = new CallModule("HID.Desktop.Plugins.FamilyInformation", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_BenhNhanTamThan_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);

                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.Nerves", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_KhamThai_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TYTFetusExam", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_KeHoachHoa_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TYTKhh", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_PhaThai_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.FetusAbortion", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_SinhDe_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TYTFetusBorn", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_BenhNhanHIV_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TytHivCreate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_TytTuVong_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TytDeathCreate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_TytSotRet_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TYTMalaria", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_TytLao_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("TYT.Desktop.Plugins.TYTTuberculosis", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_TheDiUng_ButtonClick()
        {
            try
            {
                if (this._PatientShowPopup != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_PatientShowPopup);
                    CallModule callModule = new CallModule("HIS.Desktop.Plugins.AllergyCard", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
