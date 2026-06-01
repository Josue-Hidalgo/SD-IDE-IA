using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Decorator
{
    internal class FormattedScript : ScriptDecorator
    {
        private readonly string theme;

        public FormattedScript(IScript inner, string theme = "dark") : base(inner)
        {
            this.theme = theme;
        }
        
        public override string GetText() => inner.GetText();
        public override string GetPath() => inner.GetPath();

        public string GetTheme() => theme;
    
        /* ALGO MÁS ... */
    }
}
