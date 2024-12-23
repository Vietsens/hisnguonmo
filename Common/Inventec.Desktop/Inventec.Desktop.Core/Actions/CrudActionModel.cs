#region License

// Created by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// A convenience class for creating an action model with standard Add, Edit, and Delete actions.
    /// </summary>
    /// <remarks>
    /// An instance of this class can be configured to have any or all of Add, Edit, and Delete actions.
    /// Standard labels and icons will be used for these actions, however you can freely modify these values.
    /// You may also add additional custom actions to an instance of this class.
    /// </remarks>
    public class CrudActionModel : SimpleActionModel
    {
        /// <summary>
        /// Resource key for the "Add" icon.
        /// </summary>
        public const string IconAddResource = "Icons.AddToolSmall.png";

        /// <summary>
        /// Resource key for the "Edit" icon.
        /// </summary>
        public const string IconEditResource = "Icons.EditToolSmall.png";

        /// <summary>
        /// Resource key for the "Delete" icon.
        /// </summary>
        public const string IconDeleteResource = "Icons.DeleteToolSmall.png";

        private static readonly object AddKey = new object();
        private static readonly object EditKey = new object();
        private static readonly object DeleteKey = new object();


        /// <summary>
        /// Constructor that creates an instance with Add, Edit and Delete actions.
        /// </summary>
        public CrudActionModel()
            :this(true, true, true)
        {
        }

        /// <summary>
        /// Constructor that allows specifying which of Add, Edit, and Delete actions should appear.
        /// </summary>
        /// <param name="add"></param>
        /// <param name="edit"></param>
        /// <param name="delete"></param>
        public CrudActionModel(bool add, bool edit, bool delete)
            :this(add, edit, delete, new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly))
        {
        }

		/// <summary>
		/// Constructor that allows specifying which of Add, Edit, and Delete actions should appear.
		/// </summary>
		/// <param name="add"></param>
		/// <param name="edit"></param>
		/// <param name="delete"></param>
		/// <param name="fallBackResolver"></param>
		public CrudActionModel(bool add, bool edit, bool delete, IResourceResolver fallBackResolver)
			: base(new ApplicationThemeResourceResolver(typeof(CrudActionModel).Assembly, fallBackResolver))
		{
			if (add)
			{
				this.AddAction(AddKey, SR.TitleAdd, IconAddResource);
			}
			if (edit)
			{
				this.AddAction(EditKey, SR.TitleEdit, IconEditResource);
			}
			if (delete)
			{
				this.AddAction(DeleteKey, SR.TitleDelete, IconDeleteResource);
			}
		}

        /// <summary>
        /// Gets the Add action.
        /// </summary>
        public ClickAction Add
        {
			get { return (ClickAction)this[AddKey]; }
        }

        /// <summary>
        /// Gets the Edit action.
        /// </summary>
        public ClickAction Edit
        {
			get { return (ClickAction)this[EditKey]; }
        }

        /// <summary>
        /// Gets the Delete action.
        /// </summary>
        public ClickAction Delete
        {
			get { return (ClickAction)this[DeleteKey]; }
        }
    }
}
