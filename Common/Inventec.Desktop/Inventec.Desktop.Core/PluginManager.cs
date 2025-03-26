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

// Created by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Loads plugin assemblies dynamically from disk and exposes meta-data about the set of installed
    /// plugins, extension points, and extensions to the application.
    /// </summary>
    public sealed class PluginManager
    {
        #region BackgroundAssemblyLoader

        private class BackgroundAssemblyLoader
        {
            private readonly PluginManager _owner;
            private bool _running;
            private volatile bool _cancelRequested;

            public BackgroundAssemblyLoader(PluginManager owner)
            {
                _owner = owner;
            }

            public void Run()
            {
                if (_running)
                    return;

                // if all plugin assemblies are loaded, nothing to do
                if (_owner.Plugins.All(p => p.Assembly.IsResolved))
                    return;

                Platform.Log(LogLevel.Debug, "PluginManager: Starting background assembly loading.");
                ThreadPool.QueueUserWorkItem(state => LoadAssemblies());
                _running = true;
            }

            public void Cancel()
            {
                if (!_running || _cancelRequested)
                    return;

                _cancelRequested = true;
                _running = false;

                Platform.Log(LogLevel.Debug, "PluginManager: Suspending background assembly loading.");
            }

            private void LoadAssemblies()
            {
                foreach (var plugin in _owner.Plugins.Where(p => !p.Assembly.IsResolved))
                {
                    if (_cancelRequested)
                        return;

                    try
                    {
                        plugin.Assembly.Resolve();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e);
                    }
                }
            }
        }

        #endregion

        private readonly List<PluginInfo> _plugins = new List<PluginInfo>();
        private readonly List<ExtensionInfo> _extensions = new List<ExtensionInfo>();
        private readonly List<ExtensionPointInfo> _extensionPoints = new List<ExtensionPointInfo>();
        private readonly string _pluginDir;

        private readonly object _syncLock = new object();
        private readonly PluginLoader _loader;
        private volatile bool _pluginsLoaded;

        private readonly BackgroundAssemblyLoader _backgroundAssemblyLoader;

        internal PluginManager(string pluginDir)
        {
            _pluginDir = pluginDir;

            string[] alternateCacheLocations;
            _loader = new PluginLoader(pluginDir, GetMetadataCacheFilePath(out alternateCacheLocations), alternateCacheLocations);
            _backgroundAssemblyLoader = new BackgroundAssemblyLoader(this);
        }

        /// <summary>
        /// Gets the plugin manager instance in use by the framework.
        /// </summary>
        public static PluginManager Instance
        {
            get { return Platform.PluginManager; }
        }

        #region Public API

        /// <summary>
        /// Gets information about the set of all installed plugins.
        /// </summary>
        /// <remarks>
        /// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
        /// </remarks>
        public IList<PluginInfo> Plugins
        {
            get
            {
                EnsurePluginInfoLoaded();
                return _plugins.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets information about the set of extensions defined across all installed plugins,
        /// including disabled and unlicensed extensions.
        /// </summary>
        /// <remarks>
        /// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
        /// </remarks>
        public IList<ExtensionInfo> Extensions
        {
            get
            {
                EnsurePluginInfoLoaded();
                return _extensions.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets information about the set of extension points defined across all installed plugins.  
        /// </summary>
        /// <remarks>
        /// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
        /// </remarks>
        public IList<ExtensionPointInfo> ExtensionPoints
        {
            get
            {
                EnsurePluginInfoLoaded();
                return _extensionPoints.AsReadOnly();
            }
        }

        /// <summary>
        /// Occurs when a plugin is loaded.
        /// </summary>
        public event EventHandler<PluginLoadedEventArgs> PluginLoaded
        {
            add
            {
                lock (_syncLock)
                {
                    _loader.PluginLoaded += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _loader.PluginLoaded -= value;
                }
            }
        }

        /// <summary>
        /// Enables or disables loading of any outstanding plugin assemblies on a background thread.
        /// </summary>
        public void EnableBackgroundAssemblyLoading(bool enable)
        {
            lock (_syncLock)
            {
                if (enable)
                    _backgroundAssemblyLoader.Run();
                else
                    _backgroundAssemblyLoader.Cancel();
            }
        }

        #endregion

        #region Helpers

        private void EnsurePluginInfoLoaded()
        {
            if (!_pluginsLoaded)
            {
                lock (_syncLock)
                {
                    if (!_pluginsLoaded)
                    {
                        LoadPluginInfo();
                    }
                }
            }
        }

        internal PluginResultInfo LoadSinglePluginInfo(string moduleLink)
        {
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo.Begin");
            if (String.IsNullOrEmpty(moduleLink))
                throw new PluginException(SR.ExceptionPluginCouldNotBeFound);

            PluginResultInfo pluginResultInfo = null;


            if (Platform.PluginHasAuthenticates == null)
                Platform.PluginHasAuthenticates = new Dictionary<string, string>();
            if (!Platform.PluginHasAuthenticates.ContainsKey(moduleLink.ToLower()))
            {
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo.1");
                Platform.PluginHasAuthenticates.Add(moduleLink.ToLower(), moduleLink);

                var lugInAdds = _loader.LoadSinglePluginInfo(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo lugInAdds.count=" + (lugInAdds != null ? lugInAdds.Count : 0));
                _plugins.AddRange(lugInAdds);
                if (lugInAdds.Count == 0)
                {
                    // If there are no plugins, there's nothing left to do ... but they were still loaded.
                    _pluginsLoaded = true;
                    return null;
                }

                pluginResultInfo = new PluginResultInfo();

                //// compile lists of all extension points and extensions
                var extensions = new List<ExtensionInfo>(lugInAdds.SelectMany(p => p.Extensions));
                var points = new List<ExtensionPointInfo>(lugInAdds.SelectMany(p => p.ExtensionPoints));
                //// hack: add points and extensions from Inventec.Desktop.Core, which isn't technically a plugin
                PluginInfo.DiscoverExtensionPointsAndExtensions(GetType().Assembly, points, extensions);
                //// #742: order the extensions according to the XML configuration
                var ordered = ExtensionSettings.Default.OrderExtensions(extensions);
                //SetExtensionTool(ref ordered);

                //// create global extension list
                _extensions.AddRange(ordered);

                //// points do not need to be ordered
                _extensionPoints.AddRange(points);

                _pluginsLoaded = true;

                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo ordered.count=" + (ordered != null ? ordered.Count : 0));
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo points.count=" + (points != null ? points.Count : 0));

                pluginResultInfo.PluginInfo = lugInAdds.Where(k => k.Name != null && k.Name.ToLower() == moduleLink.ToLower()).FirstOrDefault();
                pluginResultInfo.ExtensionInfo = ordered.Where(k => k.Code != null && k.Code.ToLower() == moduleLink.ToLower()).FirstOrDefault();
                pluginResultInfo.ExtensionPointInfo = points.Where(k => k.FormalName != null && k.FormalName.ToLower() == moduleLink.ToLower()).FirstOrDefault();

                if (pluginResultInfo.PluginInfo.AssemblyResolve == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("PluginManager.LoadSinglePluginInfo pluginResultInfo.PluginInfo.AssemblyResolve == null");
                }
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo.2");
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo _plugins.count=" + (_plugins != null ? _plugins.Count : 0));
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo _extensions.count=" + (_extensions != null ? _extensions.Count : 0));
                Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo _extensionPoints.count=" + (_extensionPoints != null ? _extensionPoints.Count : 0));
                pluginResultInfo = new PluginResultInfo();
                pluginResultInfo.PluginInfo = _plugins.Where(k => k.Name != null && k.Name.ToLower() == moduleLink.ToLower()).FirstOrDefault();
                pluginResultInfo.ExtensionInfo = _extensions.Where(k => k.Code != null && k.Code.ToLower() == moduleLink.ToLower()).FirstOrDefault();
                pluginResultInfo.ExtensionPointInfo = _extensionPoints.Where(k => k.FormalName != null && k.FormalName.ToLower() == moduleLink.ToLower()).FirstOrDefault();
            }
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadSinglePluginInfo.End");

            return pluginResultInfo;
        }

        private void LoadPluginInfo()
        {
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.Begin");
            if (!Directory.Exists(_pluginDir))
                throw new PluginException(SR.ExceptionPluginDirectoryNotFound);
            var pListAdds = _loader.LoadPluginInfo();
            if (pListAdds != null && pListAdds.Count > 0)
                _plugins.AddRange(pListAdds);
            if (_plugins.Count == 0)
            {
                // If there are no plugins, there's nothing left to do ... but they were still loaded.
                _pluginsLoaded = true;
                return;
            }

            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.1");
            //// compile lists of all extension points and extensions
            var extensions = new List<ExtensionInfo>(_plugins.SelectMany(p => p.Extensions));
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.2");
            var points = new List<ExtensionPointInfo>(_plugins.SelectMany(p => p.ExtensionPoints));
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.3");
            //// hack: add points and extensions from Inventec.Desktop.Core, which isn't technically a plugin
            PluginInfo.DiscoverExtensionPointsAndExtensions(GetType().Assembly, points, extensions);
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.4");
            //// #742: order the extensions according to the XML configuration
            var ordered = ExtensionSettings.Default.OrderExtensions(extensions);
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.5");
            //SetExtensionTool(ref ordered);

            //// create global extension list
            _extensions.AddRange(ordered);

            //// points do not need to be ordered
            _extensionPoints.AddRange(points);

            _pluginsLoaded = true;
            Inventec.Common.Logging.LogSystem.Debug("PluginManager.LoadPluginInfo.End");
        }

        private void SetExtensionTool(ref IList<ExtensionInfo> _extensionsForUpdates)
        {
            //ToolSetActionComponent ivComponent = new ToolSetActionComponent(itemExt);
            //md.ExtensionInfo.ToolSet = ivComponent.ToolSet;
            try
            {
                foreach (var ext in _extensionsForUpdates)
                {
                    ext.ToolSet = new ToolSetActionComponent(ext).ToolSet;
                }
            }
            catch { }
        }

        private static string GetMetadataCacheFilePath(out string[] alternates)
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            using (var sha = new SHA256CryptoServiceProvider2())
            {
                // since this is used to generate a file path, we must limit the length of the generated name so it doesn't exceed max path length
                // we don't simply use MD5 because it throws an exception if the OS has strict cryptographic policies in place (e.g. FIPS)
                // note: truncation of SHA256 seems to be an accepted method of producing a shorter hash - see notes in HashUtilities
                var hash = StringUtilities.ToHexString(sha.ComputeHash(Encoding.Unicode.GetBytes(exePath)), 0, 16);

                // alternate locations are treated as read-only pre-generated cache files (e.g. for Portable workstation)
                alternates = new[] { System.IO.Path.Combine(Platform.PluginDirectory, "pxpx", hash) };
                if (!Directory.Exists(System.IO.Path.Combine(Platform.ApplicationDataDirectory, "pxpx")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(Platform.ApplicationDataDirectory, "pxpx"));
                }
                // return the main location, which must be writable
                return System.IO.Path.Combine(Platform.ApplicationDataDirectory, "pxpx", hash);
            }
        }

        #endregion
    }
}
