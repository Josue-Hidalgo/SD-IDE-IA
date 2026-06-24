using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Decorator
{
    internal class FormattedScript : ScriptDecorator
    {
        public FormattedScript(IScript inner, Action onFormat) : base(inner)
        {
            onFormat?.Invoke();
        }

        public override string GetText() => inner.GetText();
        public override string GetPath() => inner.GetPath();
    }
}
