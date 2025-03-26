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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
		List<MOS.EFMODEL.DataModels.HIS_POSITION> dataPosition = null;
		List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> dataWorkPlace = null;
		List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> dataMilitaryRank = null;
		List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY> dataClassify = null;
		List<HIS_BHYT_WHITELIST> dataWhiteList = null;
		HIS_POSITION currentPosition = null;
		HIS_WORK_PLACE currentWorkPlace = null;
		HIS_MILITARY_RANK currentMilitaryRank = null;
		HIS_PATIENT_CLASSIFY currentPatientClassify = null;
		private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
		{
			try
			{
				InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
		{
			try
			{
				int popupWidth = 0;
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				if (!String.IsNullOrEmpty(displayMemberCode))
				{
					columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
					popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
				}
				if (!String.IsNullOrEmpty(displayMember))
				{
					columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
					popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
				}
				ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
				ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public bool GetIsChild()
		{
			try
			{
				isChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return isChild;
		}


		private void LoadDataWhiteList()
		{
			try
			{
				if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>())
				{
					dataWhiteList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>();
				}
				else
				{
					CommonParam paramCommon = new CommonParam();
					dynamic filter = new System.Dynamic.ExpandoObject();
					dataWhiteList = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>>("api/HisBhytWhiteList/Get", ApiConsumers.MosConsumer, filter, paramCommon);

					if (dataWhiteList != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST), dataWhiteList, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				}

				if (dataWhiteList != null && dataWhiteList.Count > 0)
				{
					dataWhiteList = dataWhiteList.Where(o => o.IS_ACTIVE == 1).ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void LoadPatientClassify()
		{
			try
			{
				if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>())
				{
					dataClassify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>();
				}
				else
				{
					CommonParam paramCommon = new CommonParam();
					dynamic filter = new System.Dynamic.ExpandoObject();
					dataClassify = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, paramCommon);

					if (dataClassify != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY), dataClassify, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				}

				if (dataClassify != null && dataClassify.Count > 0)
				{
					dataClassify = dataClassify.Where(o => o.IS_ACTIVE == 1).ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadMilitaryRank()
		{
			try
			{
				if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>())
				{
					dataMilitaryRank = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
				}
				else
				{
					CommonParam paramCommon = new CommonParam();
					dynamic filter = new System.Dynamic.ExpandoObject();
					dataMilitaryRank = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>("api/HisMilitaryRank/Get", ApiConsumers.MosConsumer, filter, paramCommon);

					if (dataMilitaryRank != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK), dataMilitaryRank, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				}

				if (dataMilitaryRank != null && dataMilitaryRank.Count > 0)
				{
					dataMilitaryRank = dataMilitaryRank.Where(o => o.IS_ACTIVE == 1).ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadPosition()
		{
			try
			{
				if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_POSITION>())
				{
					dataPosition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_POSITION>();
				}
				else
				{
					CommonParam paramCommon = new CommonParam();
					dynamic filter = new System.Dynamic.ExpandoObject();
					dataPosition = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_POSITION>>("api/HisPosition/Get", ApiConsumers.MosConsumer, filter, paramCommon);

					if (dataPosition != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_POSITION), dataPosition, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				}

				if (dataPosition != null && dataPosition.Count > 0)
				{
					dataPosition = dataPosition.Where(o => o.IS_ACTIVE == 1).ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadWorkPlace()
		{
			try
			{
				if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>())
				{
					dataWorkPlace = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
				}
				else
				{
					CommonParam paramCommon = new CommonParam();
					dynamic filter = new System.Dynamic.ExpandoObject();
					dataWorkPlace = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filter, paramCommon);

					if (dataWorkPlace != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE), dataWorkPlace, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				}

				if (dataWorkPlace != null && dataWorkPlace.Count > 0)
				{
					dataWorkPlace = dataWorkPlace.Where(o => o.IS_ACTIVE == 1).ToList();
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
