#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class Expression
	{
		#region NullExpression

		class NullExpression : Expression
		{
			public NullExpression()
				: base(null) { }

			public override object Evaluate(object arg)
			{
				return null;
			}
		}

		public static readonly Expression Null = new NullExpression();

		#endregion

		private readonly string _text;

		protected Expression(string text)
		{
			// treat "" as null
			_text = string.IsNullOrEmpty(text) ? null : text;
		}

		public string Text
		{
			get { return _text; }
		}

		public abstract object Evaluate(object arg);

		public override bool Equals(object obj)
		{
			var other = obj as Expression;
			return other != null && other._text == this._text;
		}

		public override int GetHashCode()
		{
			return _text == null ? 0 : _text.GetHashCode();
		}
	}
}
