#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Used by framework to provide a consistent interface for browsable meta-data objects.
    /// </summary>
    public interface IBrowsable
    {
        /// <summary>
        /// Formal name of this object, typically the type name or assembly name.  Cannot be null.
        /// </summary>
        string FormalName
        {
            get;
        }

        /// <summary>
        /// Friendly name of the object, if one exists, otherwise null.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// A friendly description of this object, if one exists, otherwise null.
        /// </summary>
        string Description
        {
            get;
        }
    }
}
