using Frontend.Decorator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/* Biblioteca Útil */
using System.Security.Cryptography;
using System.Text;


namespace Frontend.Decorator
{
    internal class SignedScript : ScriptDecorator
    {
        /* Atributos */

        private string signature;
        private readonly string csvPath;

        /* Constructor */

        public SignedScript(IScript inner, string csvPath) : base(inner)
        {
            this.csvPath = csvPath;
            signature = LoadSignatureFromCsv() ?? string.Empty;
        }

        /* Auxiliares */

        private string LoadSignatureFromCsv()
        {
            if (!File.Exists(csvPath)) return null;

            string path = inner.GetPath();
            string line = File.ReadAllLines(csvPath)
                .Skip(1)
                .FirstOrDefault(l => l.StartsWith(path + ","));

            return line?.Split(',')[1];
        }

        private static string ComputeSha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash).ToLower();
            }
        }

        private void SaveSignatureToCsv(string path, string hash)
        {
            // Si ya existe y es oculto, quitarle el atributo antes de escribir
            if (File.Exists(csvPath))
                File.SetAttributes(csvPath, File.GetAttributes(csvPath) & ~FileAttributes.Hidden);

            var lines = File.Exists(csvPath)
                ? File.ReadAllLines(csvPath).ToList()
                : new List<string>();

            if (lines.Count == 0)
                lines.Add("path,sha256");

            int idx = lines.FindIndex(l => l.StartsWith(path + ","));
            string entry = $"{path},{hash}";

            if (idx > 0) lines[idx] = entry;
            else lines.Add(entry);

            File.WriteAllLines(csvPath, lines);

            // Volver a ocultar
            File.SetAttributes(csvPath, File.GetAttributes(csvPath) | FileAttributes.Hidden);
        }

        /* Métodos */

        public bool VerifySignature()
        {
            if (string.IsNullOrEmpty(signature))
                return false;

            string currentHash = ComputeSha256(inner.GetText());
            return currentHash == signature;
        }

        public void RegenerateSignature()
        {
            this.signature = ComputeSha256(inner.GetText());
            SaveSignatureToCsv(inner.GetPath(), signature);
        }

        public string GetSignature() => signature;
        public override string GetText() => inner.GetText();
        public override string GetPath() => inner.GetPath();
    }
}
