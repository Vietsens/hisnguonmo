#region License

// Creater by phuongdt

#endregion

using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Attribute used to mark a class as being an extension of the specified extension point class.
    /// </summary>
    /// <remarks>
    /// Use this attribute to mark a class as being an extension of the specified extension point,
    /// specified by the <see cref="Type" /> of the extension point class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExtensionOfAttribute : Attribute
    {
        private readonly Type _extensionPointClass;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="extensionPointClass">The type of the extension point class which the target class extends.</param>
        public ExtensionOfAttribute(Type extensionPointClass)
        {
            Enabled = true;
            _extensionPointClass = extensionPointClass;
        }

        public ExtensionOfAttribute(
          Type extensionPointClass,
          string moduleCode,
          string moduleName,
          string description,
          int imageindex,
          string groupName,
          long moduleType,
          bool enable,
          bool visible
          )
            : this(extensionPointClass,
            moduleCode,
            moduleName,
            description,
            imageindex,
            null,
            groupName,
            moduleType,
            enable,
            visible,
             null)
        { }

        public ExtensionOfAttribute(
           Type extensionPointClass,
           string moduleCode,
           string moduleName,
           string description,
           int imageindex,
           string icon,
           string groupName,
           long moduleType,
           bool enable,
           bool visible
           )
            : this(extensionPointClass,
            moduleCode,
            moduleName,
            description,
            imageindex,
            icon,
            groupName,
            moduleType,
            enable,
            visible,
             null)
        { }

        public ExtensionOfAttribute(
            Type extensionPointClass,
            string moduleCode,
            string moduleName,
            string description,
            int imageindex,
            string icon,
            string groupName,
            long moduleType,
            bool enable,
            bool visible,
            ToolSet toolSet
            )
        {
            _extensionPointClass = extensionPointClass;
            Code = moduleCode;
            Name = moduleName;
            Description = description;
            Imageindex = imageindex;
            Icon = icon;
            GroupName = groupName;
            ModuleType = moduleType;
            Enabled = enable;
            ToolSet = toolSet;
        }

        /// <summary>
        /// The class that defines the extension point which this extension extends.
        /// </summary>
        public Type ExtensionPointClass
        {
            get { return _extensionPointClass; }
        }

        /// <summary>
        /// A friendly code for the extension.
        /// </summary>
        /// <remarks>
        /// This is optional and may be supplied as a code parameter.
        /// </remarks>
        public string Code { get; set; }

        /// <summary>
        /// A friendly name for the extension.
        /// </summary>
        /// <remarks>
        /// This is optional and may be supplied as a named parameter.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// A friendly description for the extension.
        /// </summary>
        /// <remarks>
        /// This is optional and may be supplied as a named parameter.
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// The name of an groupName resource to associate with the plugin.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The name of an ModuleType resource to associate with the plugin.
        /// </summary>
        public long ModuleType { get; set; }

        /// <summary>
        /// The name of an Imageindex resource to associate with the plugin.
        /// </summary>
        public int Imageindex { get; set; }

        /// <summary>
        /// The name of an Icon resource to associate with the plugin.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The default enablement of the extension.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Feature identification token to be checked against application licensing.
        /// </summary>
        public string FeatureToken { get; set; }

        public ToolSet ToolSet { get; set; }
    }
}