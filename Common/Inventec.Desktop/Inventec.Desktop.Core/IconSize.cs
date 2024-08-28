#region License

// Created by phuongdt

#endregion

using System;
using System.ComponentModel;
using System.Globalization;

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// Enumeration for different standard icon sizes.
	/// </summary>
	[TypeConverter(typeof (IconSizeConverter))]
	public enum IconSize
	{
		/// <summary>
		/// Small icon.
		/// </summary>
		Small,
		/// <summary>
		/// Medium icon.
		/// </summary>
		Medium,
		/// <summary>
		/// Large icon.
		/// </summary>
		Large
	}

	/// <summary>
	/// Provides a <see cref="TypeConverter"/> to convert <see cref="IconSize"/> values to and from localized and invariant <see cref="string"/> representations.
	/// </summary>
	public sealed class IconSizeConverter : EnumConverter
	{
		/// <summary>
		/// Initializes a new instance of <see cref="IconSizeConverter"/>.
		/// </summary>
		public IconSizeConverter()
			: base(typeof (IconSize)) {}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			// the culture parameter is that of the system culture, not the thread UI culture
			// if the intent is to convert for localized UI display, use the thread UI culture directly and ignore the parameter
			// if the intent is to convert for regional formatting (i.e. decimal number formatting), use the parameter culture

			if (destinationType == typeof (string))
			{
                //switch ((IconSize) value)
                //{
                //    case IconSize.Small:
                //        return SR.LabelSmall;
                //    case IconSize.Medium:
                //        return SR.LabelMedium;
                //    case IconSize.Large:
                //        return SR.LabelLarge;
                //}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var sValue = value.ToString();
                //if (string.Equals(sValue, SR.LabelSmall))
                //    return IconSize.Small;
                //else if (string.Equals(sValue, SR.LabelMedium))
                //    return IconSize.Medium;
                //else if (string.Equals(sValue, SR.LabelLarge))
                //    return IconSize.Large;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}