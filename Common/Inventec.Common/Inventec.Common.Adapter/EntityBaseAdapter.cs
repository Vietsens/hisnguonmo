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
using Inventec.Common.Logging;
using Inventec.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Adapter
{
    public abstract class EntityBaseAdapter
    {
        protected EntityBaseAdapter()
        {
            try
            {
                ClassName = this.GetType().Name;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        protected CommonParam param { get; set; }
        protected string errorFormat = "Call API \"{0}/{1}\":";
        protected string ClassName { get; set; }
        protected string MethodName { get; set; }
        public static string UserName { get; set; }
        protected string ErrorFormat { get; set; }
        protected string StatusCode { get; set; }
        protected string BugCodes { get; set; }
        protected string Input { get; set; }
        protected string Output { get; set; }
        protected int FrameIndex { get; set; }
        protected static Action actProcessSessionLogout;

        protected void LogInOut()
        {
            try
            {
                //MethodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                Logging(new StringBuilder().Append("InputData: ").Append(Input).Append(Environment.NewLine).Append("____OutputData: ").Append(Output).ToString(), LogType.Info);
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("EntityBase.LogInOut.Exception.", ex);
                }
                catch (Exception)
                {
                }
            }
        }

        protected void LogInOut(string output)
        {
            FrameIndex += 1;
            LogInOut(output, LogType.Info);
        }

        protected void LogInOut<T>(Inventec.Core.ApiResultObject<T> output)
        {
            try
            {
                FrameIndex += 1;
                BugCodes = (output != null && output.Param != null && output.Param.BugCodes != null && output.Param.BugCodes.Count > 0) ? String.Join("", output.Param.BugCodes) : "";
                if (output == null || !output.Success || output.Data == null || !String.IsNullOrEmpty(BugCodes))
                {
                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(output);
                    Logging(new StringBuilder().Append("InputData: ").Append(Input).Append(Environment.NewLine).Append("____OutputData: ").Append(data).ToString(), (!String.IsNullOrEmpty(BugCodes) ? LogType.Error : LogType.Warn));
                }
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("EntityBase.LogInOut.Exception.", ex);
                }
                catch (Exception)
                {
                }
            }
        }

        protected void LogInOut(string output, LogType logType)
        {
            try
            {
                FrameIndex += 1;
                BugCodes = "HIS00000";
                //MethodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                Logging(new StringBuilder().Append("InputData: ").Append(Input).Append(Environment.NewLine).Append("____OutputData: ").Append(output).ToString(), logType);
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("EntityBase.LogInOut.Exception.", ex);
                }
                catch (Exception)
                {
                }
            }
        }

        protected void Logging(string message, LogType en)
        {
            try
            {
                FrameIndex += 1;
                try
                {
                    //LogSystem.Info(String.Join("|||", new System.Diagnostics.StackTrace().GetFrames().Select(o => o.GetMethod().Name).ToList()));
                    //if (String.IsNullOrEmpty(MethodName))
                    //{
                    MethodName = string.Format("{0}", new System.Diagnostics.StackTrace().GetFrame(FrameIndex + 2).GetMethod().Name);
                    ClassName = string.Format("{0}", new System.Diagnostics.StackTrace().GetFrame(FrameIndex + 2).GetMethod().ReflectedType.FullName);
                    //}
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }

                message = new StringBuilder().Append(ErrorFormat).Append(String.IsNullOrEmpty(ErrorFormat) ? "" : Environment.NewLine).Append("____").Append(GetInfoProcess()).Append(Environment.NewLine).Append("____").Append("UserName: [").Append(UserName).Append("]").Append(Environment.NewLine).Append("____").Append("StatusCode: ").Append(StatusCode).Append(Environment.NewLine).Append("____").Append("BugCodes: ").Append(BugCodes).Append(Environment.NewLine).Append("____").Append(message).ToString();
                switch (en)
                {
                    case LogType.Debug:
                        LogSystem.Debug(message);
                        break;
                    case LogType.Info:
                        LogSystem.Info(message);
                        break;
                    case LogType.Warn:
                        LogSystem.Warn(message);
                        break;
                    case LogType.Error:
                        LogSystem.Error(message);
                        break;
                    case LogType.Fatal:
                        LogSystem.Fatal(message);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("EntityBase.Logging.Exception.", ex);
                }
                catch (Exception)
                {
                }
            }
        }

        protected void SetLogInputWithParam(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filterOrInputData, int userTimeout, Action action, params object[] listParam)
        {
            Input = (Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filterOrInputData), filterOrInputData) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listParam), listParam) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => userTimeout), userTimeout));
            ErrorFormat = String.Format(errorFormat, consumer.GetBaseUri(), requestUri);
            StatusCode = "200";
        }

        protected string GetInfoProcess()
        {
            try
            {
                FrameIndex += 1;
                return new StringBuilder().Append("TraceInfo: [").Append("Class: ").Append((String.IsNullOrWhiteSpace(GetClassName()) ? "" : GetClassName() + "; ")).Append("MethodName: ").Append((String.IsNullOrWhiteSpace(GetMethodName()) ? "" : GetMethodName() + "; ")).Append("LineNumber: ").Append((String.IsNullOrWhiteSpace(GetLineNumber().ToString()) ? "" : GetLineNumber().ToString())).Append("]").ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return "";
            }
        }

        protected string GetClassName()
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            return trace.GetFrame(FrameIndex + 2).GetMethod().ReflectedType.FullName;
        }

        protected string GetMethodName()
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            return trace.GetFrame(FrameIndex + 2).GetMethod().Name;
        }

        protected int GetLineNumber()
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            return trace.GetFrame(FrameIndex + 2).GetFileLineNumber();
        }

        protected static string GetThreadId()
        {
            try
            {
                return "ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId + "";
            }
            catch (Exception ex)
            {
                LogSystem.Error("EntityBase.GetThreadId.Exception.", ex);
                return "";
            }
        }

        /// <summary>
        /// True neu data != null.
        /// False neu nguoc lai.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected bool IsNotNull(Object data)
        {
            bool result = false;
            try
            {
                result = (data != null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// True neu string != NullOrEmpty.
        /// False neu nguoc lai.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected bool IsNotNullOrEmpty(string data)
        {
            bool result = false;
            try
            {
                result = (!String.IsNullOrEmpty(data));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Su dung de kiem tra list co du lieu.
        /// True neu listData != null && Count > 0.
        /// False neu nguoc lai.
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        protected bool IsNotNullOrEmpty(ICollection listData)
        {
            bool result = false;
            try
            {
                result = (listData != null && listData.Count > 0);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Su dung de kiem tra cac truong ID trong CSDL.
        /// True neu id > 0.
        /// False neu nguoc lai.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected bool IsGreaterThanZero(long id)
        {
            bool result = false;
            try
            {
                result = (id > 0);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        protected void SetCommonParam(CommonParam data)
        {
            if (data != null && param != null)
            {
                param.Messages.AddRange(data.Messages);
                param.BugCodes.AddRange(data.BugCodes);
                param.MessageCodes.AddRange(data.MessageCodes);
                param.Now = data.Now;
            }
        }

        protected enum LogType
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
        }
    }
}
