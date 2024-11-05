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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.QrCodeBHYT;
using MOS.SDO;
using System.Windows.Forms;
using DevExpress.XtraLayout;

namespace HIS.Desktop.DelegateRegister
{
    public delegate void DelegateFocusNextUserControl();
    public delegate void FocusMoveOutRoomExamService(object uc);
    public delegate bool RemoveRoomExamService(object ucName);
    public delegate void DelegateValidationUserControl(bool _isValidation);
    public delegate void DelegateVisible(bool _isVisble);
    public delegate void DelegateEnableOrDisableControl(bool? _isChild, bool? isQN);
    public delegate void DelegateEnableOrDisableBtnPatientNew(bool _isEnable);
    public delegate void DelegateUpdatePersonHomeInfo(object dataPersonHome);
    public delegate void DelegateSearchPatient(string _strSearch, string typeCodeFind);
    public delegate void DelegateSetDataRegisterBeforeSerachPatient(object dataBeforeSearch);
    public delegate void UpdatePatientInfo(MOS.SDO.HisPatientSDO patient);
    public delegate void DelegateCheckTT(HeinCardData heinCard, Action focusNextControl);
    public delegate void DelegateVisibleUCHein(long patienTypeID);
    public delegate void DelegateSetAddressUCHein(string address);
    public delegate void DelegateSetAddressUCProvince(object data);
    public delegate void DelegateSetCareerByHeinCardNumber(string heinCardNumber);
    public delegate void DelegateSetFocusWhenPatientIsChild(bool _isPatientChild);
    public delegate void DelegateSetHeinRightRouteTypeIsCC(bool _isDangKyNgoaiGio);

    public delegate List<UserControl> DelegateSetControl();
    public delegate void DelegateSetValueForUCPlusInfo(object dataSet, bool callByUCAddress);
    public delegate void DelegateNextControl(object sender, PreviewKeyDownEventArgs e);

    public delegate void DelegateSetAddressUCPlusInfo(object data, bool callByUCAddress);

    public delegate void DelegateReloadData(bool isReload);

    public delegate void DelegateShowControlHrmKskCode(bool _isShow);

    public delegate void DelegateShowControlHrmKskCodeNotValid(bool _isShow);

    public delegate void DelegateShowControlGuaranteeLoginname(bool _isShow);

    public delegate void DelegateSendPatientName(string _patientName);

    public delegate void DelegateSendCodeProvince(string codeProvince);
    public delegate void DelegateEnableButtonSave(bool isEnable);
    public delegate void DelegateHeinEnableButtonSave(bool isEnable);

    public delegate DateTime DelegateGetIntructionTime();
    public delegate void DelegateSend3WBhytCode(string code);
    public delegate void DelegateSendPatientSDO(HisPatientSDO patientSDO);
    public delegate void DelegateSendCardSDO(HisCardSDO cardSDO);
    public delegate void DelegateSendIdData(long? PatientClassifyId);
    public delegate void DelegateEnableFindType(bool IsEnableCheckBox);
    public delegate void DelegateCheckExamOnline(bool IsChecked);
    public delegate void DelegateCheckSS(bool isCheck);
}
