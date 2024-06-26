using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ProcessorBase.Core
{
    /* IVT
     * @Project : hisnguonmo
     * @Author : INVENTEC
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
    public class ProcessorFactory
    {
        private const string PROCESSOR_NAMESPACE_PREFIX = "MPS.Processor.";
        private const string DLL_EXT = ".dll";
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
        public object GetProcessor(PrintData printData, CommonParam param)
        {
            if (PROCESSOR_DIC != null && PROCESSOR_DIC.ContainsKey(printData.printTypeCode.ToUpper()))
            {
                Type t = PROCESSOR_DIC[printData.printTypeCode.ToUpper()];

                return Activator.CreateInstance(t, param, printData);
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format("Khong tim thay dll {0}{1}", PROCESSOR_NAMESPACE_PREFIX, printData.printTypeCode));
            }
            return null;
        }

        bool Init()
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin MPS.ProcessorBase ProcessorFactory Init");
            bool result = false;
            if (ProcessorFactory.PROCESSOR_DIC == null)
            {
                try
                {
                    this.LoadDll(CurrentNumberDll, TotalMps);
                    IEnumerable<Type> parserTypes = this.GetAllTypesImplement(typeof(AbstractProcessor));
                    if (parserTypes != null)
                    {
                        foreach (Type parserType in parserTypes.ToList())
                        {
                            bool valid = true;
                            //if (!IsValidAbstractBaseClass(parserType))
                            //{
                            //    LogSystem.Warn(string.Format("{0} does not appear to be a valid abstract base class.", parserType.FullName));
                            //    valid = false;
                            //}
                            //if (!this.IsRealClass(parserType))
                            //{
                            //    LogSystem.Warn(string.Format("{0} does not appear to be a valid real class.", parserType.FullName));
                            //    valid = false;
                            //}

                            if (valid)
                                this.AddType(parserType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Co loi khi load dll");
                    LogSystem.Error(ex);
                }
            }
            Inventec.Common.Logging.LogSystem.Debug("End MPS.ProcessorBase ProcessorFactory Init");
            return result;
        }

        private void AddType(Type parserType)
        {
            try
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
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDll(Action<int> CurrentNumberDll,Action<int> TotalMps)
        {
            try
            {
                string dllFolderPath = ConfigurationManager.AppSettings["MPS.Processor.Instance.Dll.Folder"];
                string rootPath = InstallDirectory;
                string[] dllFiles = Directory.GetFiles(rootPath + dllFolderPath, "*.dll", SearchOption.TopDirectoryOnly);
                if (dllFiles != null && dllFiles.Length > 0)
                {
                    if (TotalMps != null)
                        TotalMps(dllFiles.Length);
                    int count = 1;
                    foreach (string s in dllFiles)
                    {
                        Assembly.LoadFrom(s);
                        if (CurrentNumberDll != null)
                            CurrentNumberDll(count++);
                        //Assembly assembly = Assembly.LoadFrom(s);
                        //AppDomain.CurrentDomain.Load(assembly.GetName());
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

        private static object _syncRoot = new Object();
        private static volatile string _installDirectory = null;
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

        private List<Type> GetAllTypesImplement(Type desiredType)
        {
            var ass = AppDomain
                .CurrentDomain
                .GetAssemblies().Where(o => o.FullName.Substring(0, 14).ToLower() == PROCESSOR_NAMESPACE_PREFIX.ToLower());
            if (ass != null)
            {
                return ass
                    .SelectMany(assembly => GetLoadableTypes(assembly))
                    //.SelectMany(assembly => assembly.GetTypes())
                   .Where(type => type != null && IsValidAbstractBaseClass(type)).ToList();
            }
            return null;
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

        private bool IsRealClass(Type testType)
        {
            return !testType.IsAbstract
                && testType.IsClass
                && !testType.IsGenericTypeDefinition
                && !testType.IsInterface;
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
