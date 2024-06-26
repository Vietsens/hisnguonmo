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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ProcessorBase.Core
{
    public class ProcessorFactory
    {
        private const string PROCESSOR_NAMESPACE_PREFIX = "MPS.Processor.";
        private const string DLL_EXT = ".dll";
        private const int MaxFileNameLength = 27;

        private static volatile string _installDirectory = null;
        private static object _syncRoot = new Object();

        private static List<string> dllFileNames;
        private static Dictionary<string, Type> PROCESSOR_DIC = null;

        private Action<int> CurrentNumberDll;
        private Action<int> TotalMps;

        public ProcessorFactory()
        {
            Init();
        }

        public ProcessorFactory(Action<int> CurrentNumberDll, Action<int> TotalMps)
        {
            this.CurrentNumberDll = CurrentNumberDll;
            this.TotalMps = TotalMps;
            Init();
        }

        public static string InstallDirectory
        {
            get
            {
                if (_installDirectory == null)
                {
                    lock (_syncRoot)
                    {
                        if (_installDirectory == null)
                            _installDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }

                return _installDirectory;
            }
        }

        public object GetProcessor(PrintData printData, CommonParam param)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin GetProcessor");
            if (PROCESSOR_DIC != null && PROCESSOR_DIC.ContainsKey(printData.printTypeCode.ToUpper()))
            {
                Type t = PROCESSOR_DIC[printData.printTypeCode.ToUpper()];
                if (t != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetProcessor From PROCESSOR_DIC");
                    return Activator.CreateInstance(t, param, printData);
                }
            }
            else if (dllFileNames != null && dllFileNames.Exists(o => o.ToUpper().Contains(printData.printTypeCode.ToUpper())))
            {
                List<string> fileNames = dllFileNames.Where(o => o.ToUpper().Contains(printData.printTypeCode.ToUpper())).ToList();
                foreach (var fileName in fileNames)
                {
                    try
                    {
                        Type t = LoadDllFromFileName(fileName);
                        if (t != null)
                        {
                            AddType(t);
                            Inventec.Common.Logging.LogSystem.Debug("GetProcessor From dllFileNames");
                            return Activator.CreateInstance(t, param, printData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    finally
                    {
                        dllFileNames.Remove(fileName);
                    }
                }
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format("Khong tim thay dll {0}{1}", PROCESSOR_NAMESPACE_PREFIX, printData.printTypeCode));
            }
            Inventec.Common.Logging.LogSystem.Debug("GetProcessor From null");
            return null;
        }

        private Type LoadDllFromFileName(string fileName)
        {
            Assembly dll = Assembly.LoadFrom(fileName);

            if (dll.FullName.Substring(0, 14).ToLower() == PROCESSOR_NAMESPACE_PREFIX.ToLower())
            {
                var TargetFramework = (TargetFrameworkAttribute)dll.GetCustomAttributes(typeof(TargetFrameworkAttribute)).FirstOrDefault();
                if (TargetFramework != null)
                {
                    if (!TargetFramework.FrameworkName.Contains("4.5") && !TargetFramework.FrameworkName.Contains("4.0"))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Framework: " + System.IO.Path.GetFileName(fileName) + "_______" + TargetFramework.FrameworkName);
                    }
                    else if (TargetFramework.FrameworkName.Contains("4.5.") || TargetFramework.FrameworkName.Contains("4.0."))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Framework: " + System.IO.Path.GetFileName(fileName) + "_______" + TargetFramework.FrameworkName);
                    }
                }

                List<Type> types = GetLoadableTypes(dll).Where(type => type != null && IsValidAbstractBaseClass(type)).ToList();
                if (types != null && types.Count > 0)
                {
                    return types.FirstOrDefault();
                }
            }

            return null;
        }

        private bool Init()
        {
            bool result = false;
            try
            {
                if (dllFileNames == null || dllFileNames.Count <= 0)
                {
                    this.LoadFileNameDll(CurrentNumberDll, TotalMps);
                }
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co loi khi load dll");
                LogSystem.Error(ex);
            }
            return result;
        }

        private void AddType(Type parserType)
        {
            if (parserType != null)
            {
                //Xu ly de lay ma bao cao tu namespace cua thu vien dll
                string nameSpace = parserType.Namespace;
                int index = !string.IsNullOrWhiteSpace(nameSpace) ? nameSpace.IndexOf(PROCESSOR_NAMESPACE_PREFIX) : -1;
                if (index >= 0)
                {
                    string printTypeCode = nameSpace.Substring(index + PROCESSOR_NAMESPACE_PREFIX.Length).ToUpper();
                    if (ProcessorFactory.PROCESSOR_DIC != null && ProcessorFactory.PROCESSOR_DIC.ContainsKey(printTypeCode))
                    {
                        throw new Exception(string.Format("{0} co thong tin PrintTypeCode trung voi Processor khac", parserType.FullName));
                    }
                    if (ProcessorFactory.PROCESSOR_DIC == null)
                    {
                        ProcessorFactory.PROCESSOR_DIC = new Dictionary<string, Type>();
                    }
                    ProcessorFactory.PROCESSOR_DIC.Add(printTypeCode, parserType);
                }
            }
        }

        private void LoadFileNameDll(Action<int> CurrentNumberDll, Action<int> TotalMps)
        {
            try
            {
                string dllFolderPath = ConfigurationManager.AppSettings["MPS.Processor.Instance.Dll.Folder"];
                string rootPath = InstallDirectory;
                string[] dllFiles = Directory.GetFiles(rootPath + dllFolderPath, "*.dll", SearchOption.TopDirectoryOnly);
                if (dllFiles != null && dllFiles.Length > 0)
                {
                    dllFileNames = new List<string>();

                    if (TotalMps != null)
                        TotalMps(dllFiles.Length);
                    int count = 1;
                    foreach (string s in dllFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(s) && s.Length > MaxFileNameLength)
                        {
                            //kiểm tra tên file có đúng cấu trúc hay không
                            if (s.Substring(s.Length - MaxFileNameLength, 14).ToLower() == PROCESSOR_NAMESPACE_PREFIX.ToLower())
                            {
                                dllFileNames.Add(s);
                            }
                            if (CurrentNumberDll != null)
                                CurrentNumberDll(count++);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Inventec.Common.Logging.LogSystem.Error(errorMessage);
                //Display or log the error based on your application.
            }
        }

        private Type[] GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            }
        }

        private bool IsValidAbstractBaseClass(Type testType)
        {
            bool valid = false;
            try
            {
                var baseType = testType.BaseType;
                return baseType != null && baseType.IsAbstract && (baseType == typeof(AbstractProcessor));
            }
            catch (Exception ex)
            {
                valid = false;
                LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
