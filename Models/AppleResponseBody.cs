using System.Text.Json.Serialization;

namespace ExtremeInsiders.Models
{
    public class AppleResponseBody
    {
        [JsonPropertyName("notification_type")]
        private string NotificationType { get; set; }
        
        
    }
}