#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Inventec.Desktop.Core.Scripting;

namespace Inventec.Desktop.Core.Specifications
{
	[ExtensionOf(typeof(ExpressionFactoryExtensionPoint))]
	[LanguageSupport("jscript")]
	public class JScriptExpressionFactory : IExpressionFactory
	{
		#region IExpressionFactory Members

		public Expression CreateExpression(string text)
		{
			return new JScriptExpression(text);
		}

		#endregion
	}

	internal class JScriptExpression : Expression
	{
		private const string AutomaticVariableToken = "$";

		[ThreadStatic]
		private static IScriptEngine _scriptEngine;
		private IExecutableScript _script;

		internal JScriptExpression(string text)
			: base(text)
		{
		}

		public override object Evaluate(object arg)
		{
			if (string.IsNullOrEmpty(this.Text))
				return null;

			if (this.Text == AutomaticVariableToken)
				return arg;

			try
			{
				// create the script if not yet created
				if (_script == null)
					_script = CreateScript(this.Text);

				// evaluate the test expression
				var context = new Dictionary<string, object> {{AutomaticVariableToken, arg}};
				return _script.Run(context);
			}
			catch (Exception e)
			{
				throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, this.Text, e.Message), e);
			}
		}

		private static IExecutableScript CreateScript(string expression)
		{
			return ScriptEngine.CreateScript("return " + expression, new [] { AutomaticVariableToken });
		}

		private static IScriptEngine ScriptEngine
		{
			get { return _scriptEngine ?? (_scriptEngine = ScriptEngineFactory.GetEngine("jscript")); }
		}
	}
}
