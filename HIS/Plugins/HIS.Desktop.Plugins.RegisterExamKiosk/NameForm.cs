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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    class NameForm
    {
        public const string frmCheckHeinCardGOV = "frmCheckHeinCardGOV";
        public const string frmDetail = "frmDetail";
        public const string frmRegisterExamKiosk = "frmRegisterExamKiosk";
        public const string frmWaitingScreen = "frmWaitingScreen";
        public const string frmInputSave1 = "frmInputSave1";
        public const string frmServiceRoom = "frmServiceRoom";
        public const string frmInputSave = "frmInputSave";
        public const string frmRegisteredExam = "frmRegisteredExam";
        public const string frmChooseObject = "frmChooseObject";
        public const string frmInformationObject = "frmInformationObject";
        public static void CloseAllForm()
        {
            try
            {
                List<String> lsNameForm = new List<string>();
                lsNameForm.Add(NameForm.frmDetail);
                lsNameForm.Add(NameForm.frmInputSave1);
                lsNameForm.Add(NameForm.frmInputSave);
                lsNameForm.Add(NameForm.frmServiceRoom);
                lsNameForm.Add(NameForm.frmCheckHeinCardGOV);
                lsNameForm.Add(NameForm.frmRegisterExamKiosk);
                lsNameForm.Add(NameForm.frmRegisteredExam);
                lsNameForm.Add(NameForm.frmChooseObject);
                lsNameForm.Add(NameForm.frmInformationObject);
                foreach (var item in lsNameForm)
                {
                    Form fc = Application.OpenForms[item];
                    if (fc != null)
                    {
                        fc.Dispose();
                        fc.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static void CloseOtherForm()
        {
            try
            {
                List<String> lsNameForm = new List<string>();
                lsNameForm.Add(NameForm.frmDetail);
                lsNameForm.Add(NameForm.frmRegisterExamKiosk);
                lsNameForm.Add(NameForm.frmInputSave1);
                lsNameForm.Add(NameForm.frmInputSave);
                lsNameForm.Add(NameForm.frmServiceRoom);
                lsNameForm.Add(NameForm.frmCheckHeinCardGOV);
                lsNameForm.Add(NameForm.frmRegisteredExam);
                lsNameForm.Add(NameForm.frmChooseObject);
                lsNameForm.Add(NameForm.frmInformationObject);
                foreach (var item in lsNameForm)
                {
                    Form fc = Application.OpenForms[item];
                    if (fc != null)
                    {
                        fc.Dispose();
                        fc.Close();
                    }
                }

                Form Mc = Application.OpenForms[NameForm.frmWaitingScreen];
                if (Mc != null)
                {
                    Mc.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
