namespace Fundo.Core.Models.Common
{
    public class BaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
