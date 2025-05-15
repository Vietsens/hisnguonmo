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
#region License

// Author: Phuongdt

#endregion

using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System.Threading.Tasks;

namespace HIS.Desktop.ModuleExt
{
    public partial class PluginInstanceBehavior
    {
        public PluginInstanceBehavior()
        {
        }

        public List<object> AddItemIntoListArg(System.Reflection.Assembly asm)
        {
            List<object> results = new List<object>();
            try
            {
                long numpageSize = ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE") == 0 ? ConfigApplications.NumPageSize : ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE");

                if (asm.FullName.Contains(ModuleCodeConstant.MODULE_CONFIG_APPLICATION))
                {
                    results.Add((Inventec.UC.ConfigApplication.Refesh)RefeshDataConfigApplication);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_CHANGE_PASSWORD))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add((Inventec.UC.ChangePassword.HasExceptionApi)HasExceptionApi);
                    results.Add(System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_TRACKING_USER))
                {
                    results.Add(ApiConsumers.AcsConsumer);
                    results.Add(GlobalVariables.APPLICATION_CODE);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.SDA_NOTIFY))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_VVA_VOICE_COMMAND))
                {
                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.VvaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    //results.Add(numpageSize);
                    //results.Add((Action)RefeshDataVoiceCommand);
                    //results.Add(GlobalVariables.APPLICATION_CODE);

                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action> listAction = new List<Action>();
                    listAction.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);

                    List<Action> listActionType = new List<Action>();
                    listActionType.Add((Action)RefeshDataVoiceCommand);

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listAction);
                    results.Add(listActionType);
                    results.Add(listString);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.CONFIG_APP_USER))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add((Action)RefeshDataConfigApplication);
                    results.Add(GlobalVariables.APPLICATION_CODE);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.SDA_CONFIG_APP_USER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action> listAction = new List<Action>();
                    listAction.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listAction);
                    results.Add(listString);
                    results.Add(listObj);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.SDA_CONFIG_APP))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);
                    results.Add(listString);
                    results.Add(BackendDataWorker.Get<ACS_APPLICATION>());
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_USER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_APPLICATION_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }

                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_APPLICATION))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_CONTROL_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_CONTROL))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_MODULE_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_MODULE_GROUP))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_MODULE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                //else if (asm.FullName.Contains(ModuleCodeConstant.ACS_ROLE_BASE))
                //{
                //    List<string> listString = new List<string>();
                //    listString.Add(GlobalVariables.APPLICATION_CODE);
                //    listString.Add(System.IO.Path.Combine
                //  (THE.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                //  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                //    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                //    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                //    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                //    List<long> listLong = new List<long>();
                //    listLong.Add(numpageSize);

                //    List<Action<Type>> listActionType = new List<Action<Type>>();
                //    listActionType.Add((Action<Type>)ResetRamData);

                //    List<Action> listAction = new List<Action>();
                //    //listActionType.Add((Action)ResetRamData);

                //    List<object> listObj = new List<object>();
                //    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                //    results.Add(listApiConsumer);
                //    results.Add(listLong);
                //    results.Add(listActionType);
                //    results.Add(listString);
                //    results.Add(listObj);
                //    results.Add(listAction);
                //}
                else if (asm.FullName.Contains(ModuleCodeConstant.ACS_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action<List<object>, Inventec.Desktop.Common.Modules.Module, Inventec.Desktop.Common.Modules.Module>> listActionCallModule = new List<Action<List<object>, Inventec.Desktop.Common.Modules.Module, Inventec.Desktop.Common.Modules.Module>>();
                    listActionCallModule.Add(DelegateCallModule);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(GlobalVariables.currentModuleRaws);

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                    results.Add(listActionCallModule);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_EVENT_LOG))
                {
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "");

                    results.Add(dataInit3);
                    results.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_CHOOSE_ROOM))
                {
                    results.Add((HIS.Desktop.Common.RefeshReference)RefeshReference);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.SDA_HIDE_CONTROL))
                {
                    List<string> listString = new List<string>();
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);
                    //string branchcode = GetBranch().BRANCH_CODE;
                    Dictionary<string, string> dicBranch = new Dictionary<string, string>();
                    dicBranch = BackendDataWorker.Get<HIS_BRANCH>().ToDictionary(o => o.BRANCH_CODE, o => o.BRANCH_NAME);
                    results.Add(dicBranch);
                    results.Add(listApiConsumer);
                    results.Add(listString);
                    results.Add(dicBranch);
                }
                else if (asm.FullName.Contains(ModuleCodeConstant.MODULE_CRM_REMOTE_SUPPORT_MANAGER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    string customerInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.VPLUS_CUSTOMER_INFO");
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 1)
                        {
                            string customerCode = cusInfoArr[0];

                            string người_dùng_cuối = String.Format("{0}:{1}:{2}", cusInfoArr[0], "hispro", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                            listString.Add(người_dùng_cuối);
                        }
                    }

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.CrmConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    results.Add(listApiConsumer);
                    results.Add(listString);
                    results.Add(listLong);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return results;
        }

        public List<object> AddItemIntoListArg(string moduleLink)
        {
            List<object> results = new List<object>();
            try
            {
                long numpageSize = ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE") == 0 ? ConfigApplications.NumPageSize : ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE");

                if (moduleLink.Contains(ModuleCodeConstant.MODULE_CONFIG_APPLICATION))
                {
                    results.Add((Inventec.UC.ConfigApplication.Refesh)RefeshDataConfigApplication);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_CHANGE_PASSWORD))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add((Inventec.UC.ChangePassword.HasExceptionApi)HasExceptionApi);
                    results.Add(System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_TRACKING_USER))
                {
                    results.Add(ApiConsumers.AcsConsumer);
                    results.Add(GlobalVariables.APPLICATION_CODE);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.SDA_NOTIFY))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_VVA_VOICE_COMMAND))
                {
                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.VvaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    //results.Add(numpageSize);
                    //results.Add((Action)RefeshDataVoiceCommand);
                    //results.Add(GlobalVariables.APPLICATION_CODE);

                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action> listAction = new List<Action>();
                    listAction.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);

                    List<Action> listActionType = new List<Action>();
                    listActionType.Add((Action)RefeshDataVoiceCommand);

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listAction);
                    results.Add(listActionType);
                    results.Add(listString);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.CONFIG_APP_USER))
                {
                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add((Action)RefeshDataConfigApplication);
                    results.Add(GlobalVariables.APPLICATION_CODE);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.SDA_CONFIG_APP_USER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action> listAction = new List<Action>();
                    listAction.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listAction);
                    results.Add(listString);
                    results.Add(listObj);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.SDA_CONFIG_APP))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    results.Add(ApiConsumers.SdaConsumer);
                    results.Add(numpageSize);
                    results.Add((Action)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init);
                    results.Add(listString);
                    results.Add(BackendDataWorker.Get<ACS_APPLICATION>());
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_USER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_APPLICATION_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }

                else if (moduleLink.Contains(ModuleCodeConstant.ACS_APPLICATION))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_CONTROL_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_CONTROL))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_MODULE_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_MODULE_GROUP))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_MODULE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                }
                //else if (moduleLink.Contains(ModuleCodeConstant.ACS_ROLE_BASE))
                //{
                //    List<string> listString = new List<string>();
                //    listString.Add(GlobalVariables.APPLICATION_CODE);
                //    listString.Add(System.IO.Path.Combine
                //  (THE.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                //  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                //    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                //    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                //    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                //    List<long> listLong = new List<long>();
                //    listLong.Add(numpageSize);

                //    List<Action<Type>> listActionType = new List<Action<Type>>();
                //    listActionType.Add((Action<Type>)ResetRamData);

                //    List<Action> listAction = new List<Action>();
                //    //listActionType.Add((Action)ResetRamData);

                //    List<object> listObj = new List<object>();
                //    //listObj.Add(BackendDataWorker.Get<ACS_APPLICATION>());

                //    results.Add(listApiConsumer);
                //    results.Add(listLong);
                //    results.Add(listActionType);
                //    results.Add(listString);
                //    results.Add(listObj);
                //    results.Add(listAction);
                //}
                else if (moduleLink.Contains(ModuleCodeConstant.ACS_ROLE))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    List<Action<Type>> listActionType = new List<Action<Type>>();
                    listActionType.Add((Action<Type>)ResetRamData);

                    List<Action<List<object>, Inventec.Desktop.Common.Modules.Module, Inventec.Desktop.Common.Modules.Module>> listActionCallModule = new List<Action<List<object>, Inventec.Desktop.Common.Modules.Module, Inventec.Desktop.Common.Modules.Module>>();
                    listActionCallModule.Add(DelegateCallModule);

                    List<Action> listAction = new List<Action>();
                    //listActionType.Add((Action)ResetRamData);

                    List<object> listObj = new List<object>();
                    listObj.Add(GlobalVariables.currentModuleRaws);

                    results.Add(listApiConsumer);
                    results.Add(listLong);
                    results.Add(listActionType);
                    results.Add(listString);
                    results.Add(listObj);
                    results.Add(listAction);
                    results.Add(listActionCallModule);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_EVENT_LOG))
                {
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "");

                    results.Add(dataInit3);
                    results.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_CHOOSE_ROOM))
                {
                    results.Add((HIS.Desktop.Common.RefeshReference)RefeshReference);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.SDA_HIDE_CONTROL))
                {
                    List<string> listString = new List<string>();
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.AcsConsumer);
                    //string branchcode = GetBranch().BRANCH_CODE;
                    Dictionary<string, string> dicBranch = new Dictionary<string, string>();
                    dicBranch = BackendDataWorker.Get<HIS_BRANCH>().ToDictionary(o => o.BRANCH_CODE, o => o.BRANCH_NAME);
                    results.Add(dicBranch);
                    results.Add(listApiConsumer);
                    results.Add(listString);
                    results.Add(dicBranch);
                }
                else if (moduleLink.Contains(ModuleCodeConstant.MODULE_CRM_REMOTE_SUPPORT_MANAGER))
                {
                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                    string customerInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.VPLUS_CUSTOMER_INFO");
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 1)
                        {
                            string customerCode = cusInfoArr[0];

                            string người_dùng_cuối = String.Format("{0}:{1}:{2}", cusInfoArr[0], "hispro", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                            listString.Add(người_dùng_cuối);
                        }
                    }

                    List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = new List<Inventec.Common.WebApiClient.ApiConsumer>();
                    listApiConsumer.Add(ApiConsumers.SdaConsumer);
                    listApiConsumer.Add(ApiConsumers.CrmConsumer);

                    List<long> listLong = new List<long>();
                    listLong.Add(numpageSize);

                    results.Add(listApiConsumer);
                    results.Add(listString);
                    results.Add(listLong);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return results;
        }

        public HIS_BRANCH GetBranch()
        {
            HIS_BRANCH branch = null;
            try
            {
                branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => o.ID == BranchDataWorker.GetCurrentBranchId()).SingleOrDefault();

                if (branch == null) branch = new MOS.EFMODEL.DataModels.HIS_BRANCH();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return branch;
        }

        /// <summary>
        /// Gọi module
        /// </summary>
        public void DelegateCallModule(List<object> data, Inventec.Desktop.Common.Modules.Module moduleLink, Inventec.Desktop.Common.Modules.Module moduleWithRoom)
        {
            try
            {
                ShowModule(moduleLink, moduleWithRoom.RoomId, moduleWithRoom.RoomTypeId, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        /// <summary>
        /// Load lại dữ liệu trong ram
        /// </summary>
        public void ResetRamData(Type type)
        {
            try
            {
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset(type.GetType());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        /// <summary>
        /// Load lại giao diện main form sau khi chọn phòng
        /// </summary>
        public void RefeshReference()
        {
            try
            {
                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    WaitingManager.Show();
                    //Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                    MethodResetAllTabpageToDefaultThread(formMain);
                    //Reload lại thông tin phòng, chi nhánh,... trong vùng status bar trong formmain                
                    MethodInitStatusBarThread(formMain);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        void MethodRefeshReferenceThread()
        {
            try
            {
                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    if (formMain.InvokeRequired)
                    {
                        formMain.Invoke(new MethodInvoker(delegate
                        {
                            WaitingManager.Show();
                            //Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                            MethodResetAllTabpageToDefaultThread(formMain);

                            //Reload lại thông tin phòng, chi nhánh,... trong vùng status bar trong formmain
                            MethodInitStatusBarThread(formMain);
                            WaitingManager.Hide();
                        }));
                    }
                    else
                    {
                        WaitingManager.Show();
                        //Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                        MethodResetAllTabpageToDefaultThread(formMain);

                        //Reload lại thông tin phòng, chi nhánh,... trong vùng status bar trong formmain
                        MethodInitStatusBarThread(formMain);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void MethodResetAllTabpageToDefaultThread(object formMain)
        {
            try
            {
                Type type = formMain.GetType();
                if (type != null)
                {
                    MethodInfo methodInfo__ResetAllTabpageToDefault = type.GetMethod("ResetAllTabpageToDefault");
                    if (methodInfo__ResetAllTabpageToDefault != null)
                        methodInfo__ResetAllTabpageToDefault.Invoke(formMain, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void MethodInitStatusBarThread(object formMain)
        {
            try
            {
                Type type = formMain.GetType();
                if (type != null)
                {
                    MethodInfo methodInfo__InitStatusBar = type.GetMethod("InitStatusBar");
                    if (methodInfo__InitStatusBar != null)
                        methodInfo__InitStatusBar.Invoke(formMain, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sau khi sửa cấu hình phần mềm => đóng các tab đang mở, reload lại các dữ liệu liên quan
        /// </summary>
        public void RefeshDataConfigApplication()
        {
            try
            {
                ConfigApplicationWorker.ReloadAll();
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    Type type = formMain.GetType();
                    if (type != null)
                    {
                        MethodInfo methodInfo__ResetAllTabpageToDefault = type.GetMethod("ResetAllTabpageToDefault");
                        if (methodInfo__ResetAllTabpageToDefault != null)
                            methodInfo__ResetAllTabpageToDefault.Invoke(formMain, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void RefeshDataVoiceCommand()
        {
            try
            {
                VoiceCommandProcess.ReloadVoiceCommand();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xử lý khi mất token
        /// </summary>
        /// <param name="param"></param>
        public void HasExceptionApi(CommonParam param)
        {
            try
            {
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static void ShowModule(string moduleLink, long roomId, long roomTypeId, List<object> listArgs)
        {
            string name = "";
            try
            {
                if (moduleLink == null) throw new ArgumentNullException("moduleLink is null");
                if (listArgs == null) throw new ArgumentNullException("listArgs is null");

                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == moduleLink).FirstOrDefault();
                //moduleData.Visible = false;//TODO???? dòng này chạy sẽ làm mất chức năng trên menu nếu mở 1 module hiển thị trên menu
                if (moduleData == null) throw new ArgumentException("Not found module by ModuleLink = '" + moduleLink + "'");
                ShowModule(ButtonMenuProcessor.CreateModuleData(moduleData, roomId, roomTypeId), listArgs);
            }
            catch (ArgumentNullException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.NguoiDungNhapDuLieuKhongHopLe), name + " - " + moduleLink), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (ArgumentException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), name + " - " + moduleLink), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowModule(Inventec.Desktop.Common.Modules.Module moduleData, List<object> listArgs)
        {
            try
            {
                ShowModule(moduleData, 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowModule(Inventec.Desktop.Common.Modules.Module moduleData, long roomId, long roomTypeId, List<object> listArgs)
        {
            string name = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ShowModule.1");
                WaitingManager.Show();
                if (moduleData == null) throw new ArgumentNullException("moduleData is null");
                if (String.IsNullOrEmpty(moduleData.ModuleLink)) throw new ArgumentNullException("ModuleLink is null");
                if (listArgs == null) throw new ArgumentNullException("listArgs is null");

                name = (moduleData.text + " - " + moduleData.ModuleLink);
                //if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new ArgumentException("Module '" + moduleData.ModuleLink + "' is not plugins");

                Inventec.Common.Logging.LogSystem.Debug("ShowModule.2");
                Inventec.Desktop.Common.Modules.Module moduleShow = new Inventec.Desktop.Common.Modules.Module();
                if (roomId > 0 && roomTypeId > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ShowModule.3");
                    moduleShow = ButtonMenuProcessor.CreateModuleData(moduleData, roomId, roomTypeId);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ShowModule.4");
                    moduleShow = ButtonMenuProcessor.CreateModuleData(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
                }
                Inventec.Common.Logging.LogSystem.Debug("ShowModule.moduleData:ModuleLink=" + moduleData.ModuleLink + ",IsNotShowDialog=" + moduleData.IsNotShowDialog);
                var extenceInstance = PluginInstance.GetPluginInstance(moduleShow, listArgs);
                Inventec.Common.Logging.LogSystem.Debug("-----------------------------" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleShow), moduleShow));
                Inventec.Common.Logging.LogSystem.Debug("-----------------------------" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listArgs), listArgs));
                LogSystem.Info($"extenceInstance11111111111 {extenceInstance}, ");
                if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                moduleData.ExtensionInfo = moduleShow.ExtensionInfo;
                LogSystem.Info($"moduleData.ExtensionInfo {moduleData.ExtensionInfo}");
                moduleData.PluginInfo = moduleShow.PluginInfo;
                LogSystem.Info($"moduleData.PluginInfo {moduleData.PluginInfo}");
                moduleData.ModuleTypeId = moduleShow.ModuleTypeId;
                LogSystem.Info($"moduleData.ModuleTypeId {moduleData.ModuleTypeId}");

                Inventec.Common.Logging.LogSystem.Debug("ShowModule.5");
                WaitingManager.Hide();

                if (extenceInstance is System.Windows.Forms.Form)
                {
                    LogSystem.Info($"extenceInstance6666666666666666 {extenceInstance}");
                    if (moduleData.IsNotShowDialog.HasValue && moduleData.IsNotShowDialog.Value)
                        ((System.Windows.Forms.Form)extenceInstance).Show();
                    else
                    {
                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    }
                }
                else if (extenceInstance is System.Windows.Forms.UserControl)
                {
                    TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleShow.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleShow);
                }
                
                //switch (moduleShow.ModuleTypeId)
                //{
                //    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM:
                //        if (moduleData.IsNotShowDialog.HasValue && moduleData.IsNotShowDialog.Value)
                //            ((System.Windows.Forms.Form)extenceInstance).Show();
                //        else
                //        {
                //            ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                //        }
                //        break;
                //    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC:
                //        TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleShow.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleShow);
                //        break;
                //    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__COMBO:
                //        if (extenceInstance is System.Windows.Forms.UserControl)
                //        {
                //            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleShow.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleShow);
                //        }
                //        else if (extenceInstance is System.Windows.Forms.Form)
                //        {
                //            if (moduleData.IsNotShowDialog.HasValue && moduleData.IsNotShowDialog.Value)
                //                ((System.Windows.Forms.Form)extenceInstance).Show();
                //            else
                //            {
                //                ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                //            }
                //        }
                //        break;
                //    default:
                //        if (extenceInstance is System.Windows.Forms.UserControl)
                //        {
                //            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleShow.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleShow);
                //        }
                //        else if (extenceInstance is System.Windows.Forms.Form)
                //        {
                //            ((System.Windows.Forms.Form)extenceInstance).Show();
                //        }
                //        break;
                //}
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa), name), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomTypeId), roomTypeId));
            }
            catch (ArgumentNullException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), name), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomTypeId), roomTypeId));
            }
            catch (ArgumentException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), name), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomTypeId), roomTypeId));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomTypeId), roomTypeId));
            }
        }


    }
}
