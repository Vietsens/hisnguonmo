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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AntibioticRequest.ADO;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using MOS.Filter;

namespace HIS.Desktop.Plugins.AntibioticRequest.Run
{
	public partial class frmAntibioticRequest
	{
		private bool DelegateRunPrinter(string printTypeCode, string fileName)
		{
			bool result = false;
			try
			{
				switch (printTypeCode)
				{
					case "Mps000462":
						LoadBieuMauMps462(printTypeCode, fileName, ref result);
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return result;
		}

		private void LoadBieuMauMps462(string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				WaitingManager.Show();
				MPS.Processor.Mps000462.PDO.Mps000462PDO pdo = new MPS.Processor.Mps000462.PDO.Mps000462PDO(
					this.currentAntibiotiRequestView,
					this.currentAntibioticMicrobi,
					this.currentAntibioticNewRegView,
					this.currentAntibioticOldReg,
					this.currentDhst);
				WaitingManager.Hide();
				string printerName = "";
				if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
				{
					printerName = GlobalVariables.dicPrinter[printTypeCode];
				}

				Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.currentAntibioticRequest.AntibioticRequest != null ? this.currentAntibioticRequest.AntibioticRequest.TREATMENT_CODE : "", printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);

				if (ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW") == "1")
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
				}
				else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
				}
				else
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
				}


			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
				WaitingManager.Hide();
			}
		}
	}
}
