#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Inventec.Desktop.Core
{


    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    // these settings must be stored in a config file, not in the configuration store
    //[SettingsProvider(typeof(ApplicationCriticalSettingsProvider))]
    //[SharedSettingsMigrationDisabled]
    internal sealed partial class ExtensionSettings
    {

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }

        /// <summary>
        /// Orders the extensions according to the order specified by the XML document.
        /// </summary>
        public IList<ExtensionInfo> OrderExtensions(IEnumerable<ExtensionInfo> extensions)
        {
            var ordered = new List<ExtensionInfo>();
            var remainder = new List<ExtensionInfo>(extensions);

            // order the items based on the order in the XML
            foreach (XmlElement element in ListExtensionNodes())
            {
                var className = element.GetAttribute("class");

                // ignore entries that don't specify the "class" attribute
                // (these shouldn't exist in a well-formed doc)
                if (string.IsNullOrEmpty(className))
                    continue;

                // find the extensions corresponding to this class
                // (yes, it is possible that that are multiple extensions implemented by the same class)
                var typeRef = TypeRef.Get(className);
                var items = remainder.Where(x => Equals(typeRef, x.ExtensionClass)).ToList();

                // add these to the ordered list and remove them from the remainder
                foreach (var ext in items)
                {
                    ordered.Add(ext);
                    remainder.Remove(ext);
                }

                // if all known extensions are now ordered, we are done
                if (remainder.Count == 0)
                    break;
            }

            // return the unified, ordered list, starting with those whose order was explicitly specified in the settings, followed by the rest in order of discovery
            return ordered.Concat(remainder).ToList();
        }

        /// <summary>
        /// Determines whether the specified extension class is enabled.
        /// </summary>
        /// <param name="extensionClass"></param>
        /// <param name="defaultEnablement"></param>
        /// <returns></returns>
        public bool IsEnabled(Type extensionClass, bool defaultEnablement)
        {
            // find an XML entry for this class
            XmlElement extensionNode = (XmlElement)CollectionUtils.SelectFirst(ListExtensionNodes(),
                                                                                delegate(object node)
                                                                                {
                                                                                    string className = ((XmlElement)node).GetAttribute("class");
                                                                                    return !string.IsNullOrEmpty(className) && Type.GetType(className) == extensionClass;
                                                                                });

            // if an entry exists, check if it specifies an enablement override
            if (extensionNode != null)
            {
                string enabledString = extensionNode.GetAttribute("enabled");
                if (!string.IsNullOrEmpty(enabledString))
                    return Convert.ToBoolean(enabledString);
            }

            // return default
            return defaultEnablement;
        }

        /// <summary>
        /// List the stored extensions in the XML doc
        /// </summary>
        /// <returns>A list of "extension" element</returns>
        private XmlNodeList ListExtensionNodes()
        {
            return this.ExtensionConfigurationXml.SelectNodes(String.Format("/extensions/extension"));
        }
    }
}
