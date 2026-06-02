using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Decorator
{
    internal class Script : IScript
    {
        private readonly string text;
        private readonly string path;

        public Script(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el script en: {path}");
            this.path = path;
            this.text = File.ReadAllText(path);
        }

        public Script(string path, string text)
        {
            this.path = path;
            this.text = text;
        }

        public string GetText() => text;
        public string GetPath() => path;
    }
}
