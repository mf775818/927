using ShoeMoldControl.Core;

namespace ShoeMoldControl.Application
{
    public class DefaultBarcodeParser : IBarcodeParser
    {
        public string GenerateScriptCommand(string decodedText)
        {
            if (string.IsNullOrWhiteSpace(decodedText)) return string.Empty;

            string[] parts = decodedText.Split('-');
            if (parts.Length < 2) return string.Empty;

            string scriptName = $"blockly_{parts[0]}-{parts[1]}";
            return $"RunScript(\"{scriptName}\")";
        }
    }
}
