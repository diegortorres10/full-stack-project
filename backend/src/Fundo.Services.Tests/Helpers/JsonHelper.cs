using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fundo.Services.Tests.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// JsonSerializerOptions configured to match the API settings.
        /// Includes JsonStringEnumConverter to handle enums as strings.
        /// </summary>
        public static JsonSerializerOptions GetApiJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
