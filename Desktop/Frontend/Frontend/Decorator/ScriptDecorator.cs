using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Decorator
{
    internal abstract class ScriptDecorator : IScript
    {
        protected readonly IScript inner;

        protected ScriptDecorator(IScript inner)
        {
            this.inner = inner;
        }

        public virtual string GetText() => inner.GetText();
        public virtual string GetPath() => inner.GetPath();
    }
}
