using System.Collections.Generic;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;

namespace ExtremeInsiders.Models
{
    public class DefaultNotification : NotificationBase
    {
        public EntityBase Entity { get; set; }

        public override Dictionary<string, string> Data => new Dictionary<string, string>
        {
            { "open_id", Entity?.Id.ToString() },
            {
                "open_id_type", Entity?.EntityType
            }
        };

        public static DefaultNotification NewVideo(Video entity)
            => new DefaultNotification
            {
                Entity = entity,
                Translations = new TranslationCollection<NotificationBase, NotificationBaseTranslation>
                {
                    new NotificationBaseTranslation { Title = "Новое видео уже доступно!", CultureId = Culture.Russian.Id, Body = "{0}" },
                    new NotificationBaseTranslation { Title = "New video is out now!", CultureId = Culture.English.Id, Body = "{0}" }
                },
                BodyArgs = new List<object> { entity.Content?.Name ?? "Error" }
            };
    }
}