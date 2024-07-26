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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private void CreateThreadInitWCFReadCard()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(InitWCFReadCardThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void InitWCFReadCardThread()
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { this.CallPatient(); }));
                //}
                //else
                //{
                CARD.WCF.Service.TapCardService.TapCardServiceManager.OpenHost();
                CARD.WCF.Service.TapCardService.TapCardServiceManager.SetDelegate(this.CheckServiceCodeDelegate);
                //xaundv  #1136 Bo Ket Noi CDA ==> THE
                // Inventec.WCFService.ReadServiceCodeService.ReadServiceCodeServiceManager.SetDelegate(this.CheckServiceCodeDelegate, null);
                // Inventec.WCFService.ReadServiceCodeService.ReadServiceCodeServiceManager.OpenHost();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        bool CheckServiceCodeDelegate(string serviceCode)
        {
            bool success = false;
            try
            {
                this.SearchAndFillDataCardInfo(serviceCode);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        void SearchAndFillDataCardInfo(string serviceCode)
        {
            try
            {
                IsReadCardTheViet = false;
                CommonParam param = new CommonParam();
                var patientInRegisterSearchByCard = new BackendAdapter(param).Get<HisCardSDO>(RequestUriStore.HIS_CARD_GETVIEWBYSERVICECODE, ApiConsumers.MosConsumer, serviceCode, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (patientInRegisterSearchByCard != null)
                {
                    this.actionType = GlobalVariables.ActionAdd;
                    this.cardSearch = patientInRegisterSearchByCard;
                    IsReadCardTheViet = true;
                    var data = this.SearchByCode(patientInRegisterSearchByCard.PatientCode);
                    if (data != null && data.Result != null && data.Result is HisPatientSDO)
                    {
                        //xuandv --- ThongBaoCu
                        // DevExpress.Utils.WaitDialogForm waitLoad = new DevExpress.Utils.WaitDialogForm(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));
                        //Benh nhan da dang ky tren he thong benh vien, da co thong tin ho so
                        currentPatientSDO = data.Result as HisPatientSDO;
                        currentPatientSDO.HT_COMMUNE_NAME = HtCommuneName = patientInRegisterSearchByCard.HtCommuneName;
                        currentPatientSDO.HT_DISTRICT_NAME = HtDistrictName = patientInRegisterSearchByCard.HtDistrictName;
                        currentPatientSDO.HT_PROVINCE_NAME = HtProvinceName = patientInRegisterSearchByCard.HtProvinceName;
                        currentPatientSDO.HT_COMMUNE_CODE = HtCommuneCode = patientInRegisterSearchByCard.HtCommuneCode;
                        currentPatientSDO.HT_DISTRICT_CODE = HtDistrictCode = patientInRegisterSearchByCard.HtDistrictCode;
                        currentPatientSDO.HT_PROVINCE_CODE = HtProvinceCode = patientInRegisterSearchByCard.HtProvinceCode;
                        this.SetPatientSearchPanel(true);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            this.ProcessPatientCodeKeydown(data.Result);
                        }));
                        //try
                        //{
                        //    if (!waitLoad.IsDisposed) waitLoad.Invoke(new MethodInvoker(delegate() { waitLoad.Dispose(); }));
                        //}
                        //catch (Exception ex)
                        //{
                        //    LogSystem.Debug("Dispose waitLoad fail.", ex);
                        //}
                    }
                    else
                    {
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            //DevExpress.Utils.WaitDialogForm waitLoad = new DevExpress.Utils.WaitDialogForm(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));

                            //An button lam moi khi co du lieu benh nhan cu
                            this.SetPatientSearchPanel(false);

                            //Benh nhan chua dang ky tren he thong benh vien, chua co thong tin ho so
                            HisPatientSDO patientByCard = new HisPatientSDO();
                            this.SetPatientDTOFromCardSDO(patientInRegisterSearchByCard, patientByCard);
                            this.FillDataPatientToControl(patientByCard);

                            this.FillDataToHeinCardControlByCardSDO(patientInRegisterSearchByCard);
                            //try
                            //{
                            //    if (!waitLoad.IsDisposed) waitLoad.Invoke(new MethodInvoker(delegate() { waitLoad.Dispose(); }));
                            //}
                            //catch (Exception ex)
                            //{
                            //    LogSystem.Debug("Dispose waitLoad fail.", ex);
                            //}
                        }));
                    }
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        this.cardSearch = null;
                        if (param.Messages == null || param.Messages.Count == 0)
                        {
                            param.Messages.Add(ResourceMessage.ThongBaoKetQuaTimKiemBenhNhanKhiQuetTheDuLieuTraVeNull);
                        }
                        MessageManager.Show(param, null);
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool IsReadCardTheViet = false;
        public string HtCommuneCode = null;
        public string HtDistrictCode = null;
        public string HtProvinceCode = null;
        public string HtCommuneName = null;
        public string HtDistrictName = null;
        public string HtProvinceName = null;
        private void SetPatientDTOFromCardSDO(HisCardSDO cardSDO, HisPatientSDO patientByCard)
        {
            try
            {
                if (cardSDO == null) throw new ArgumentNullException("cardSDO");
                if (patientByCard == null) throw new ArgumentNullException("patientByCard");

                patientByCard.ID = (cardSDO.PatientId ?? 0);
                patientByCard.PATIENT_CODE = cardSDO.PatientCode;
                patientByCard.FIRST_NAME = cardSDO.FirstName;
                patientByCard.LAST_NAME = cardSDO.LastName;
                patientByCard.ADDRESS = cardSDO.Address;
                patientByCard.CAREER_ID = cardSDO.CareerId;
                //patientByCard.CMND_DATE = cardSDO.CmndDate;
                //patientByCard.CMND_NUMBER = cardSDO.CmndNumber;
                //patientByCard.CMND_PLACE = cardSDO.CmndPlace;
                patientByCard.COMMUNE_NAME = cardSDO.CommuneName;
                patientByCard.DISTRICT_NAME = cardSDO.DistrictName;
                patientByCard.PROVINCE_NAME = cardSDO.ProvinceName;
                patientByCard.DOB = cardSDO.Dob;
                patientByCard.EMAIL = cardSDO.Email;
                patientByCard.ETHNIC_NAME = cardSDO.EthnicName;
                if (cardSDO.Dob > 0 && cardSDO.Dob.ToString().Length == 4)
                    patientByCard.IS_HAS_NOT_DAY_DOB = 1;
                else
                    patientByCard.IS_HAS_NOT_DAY_DOB = 0;
                patientByCard.PHONE = cardSDO.Phone;
                //patientByCard.RECENT_ROOM_ID = cardSDO.ReligionName;//TODO
                //patientByCard.RECENT_SERVICE_ID = cardSDO.Address;//TODO
                patientByCard.RELIGION_NAME = cardSDO.ReligionName;
                patientByCard.VIR_ADDRESS = cardSDO.VirAddress;
                patientByCard.VIR_PATIENT_NAME = patientByCard.LAST_NAME + " " + patientByCard.FIRST_NAME;
                //patientByCard.GENDER_CODE = cardSDO.GenderCode;
                //patientByCard.GENDER_NAME = cardSDO.GenderName;            
                patientByCard.GENDER_ID = cardSDO.GenderId;

                patientByCard.HT_COMMUNE_NAME = HtCommuneName = cardSDO.HtCommuneName;
                patientByCard.HT_DISTRICT_NAME = HtDistrictName = cardSDO.HtDistrictName;
                patientByCard.HT_PROVINCE_NAME = HtProvinceName = cardSDO.HtProvinceName;
                patientByCard.HT_COMMUNE_CODE = HtCommuneCode = cardSDO.HtCommuneCode;
                patientByCard.HT_DISTRICT_CODE = HtDistrictCode = cardSDO.HtDistrictCode;
                patientByCard.HT_PROVINCE_CODE = HtProvinceCode = cardSDO.HtProvinceCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToHeinCardControlByCardSDO(HisCardSDO cardSDO)
        {
            try
            {
                if (!String.IsNullOrEmpty(cardSDO.HeinCardNumber))
                {
                    if (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(cardSDO.HeinCardNumber))
                    {
                        HIS_PATIENT_TYPE_ALTER patientTypeALter = new HIS_PATIENT_TYPE_ALTER();
                        patientTypeALter.HEIN_CARD_NUMBER = cardSDO.HeinCardNumber;
                        patientTypeALter.HEIN_CARD_FROM_TIME = cardSDO.HeinCardFromTime;
                        patientTypeALter.HEIN_CARD_TO_TIME = cardSDO.HeinCardToTime;
                        patientTypeALter.HEIN_MEDI_ORG_CODE = cardSDO.HeinOrgCode;
                        patientTypeALter.HEIN_MEDI_ORG_NAME = cardSDO.HeinOrgName;
                        patientTypeALter.ADDRESS = cardSDO.HeinAddress;
                        patientTypeALter.JOIN_5_YEAR = cardSDO.Join5Year;
                        patientTypeALter.PAID_6_MONTH = cardSDO.Paid6Month;
                        patientTypeALter.LEVEL_CODE = cardSDO.LevelCode;
                        patientTypeALter.LIVE_AREA_CODE = cardSDO.LiveAreaCode;
                        patientTypeALter.RIGHT_ROUTE_CODE = cardSDO.RightRouteCode;
                        if (this.mainHeinProcessor != null && ucHeinBHYT != null)
                            this.mainHeinProcessor.FillDataHeinInsuranceInfoByPatientTypeAlter(this.ucHeinBHYT, patientTypeALter);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("So the bhyt (tu du lieu tra ve khi quet the thong minh vao dau doc) khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cardSDO), cardSDO));
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
