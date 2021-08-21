using System.Collections.Generic;
using ExtremeInsiders.Entities;
using FirebaseAdmin.Messaging;

namespace ExtremeInsiders.Helpers
{
    public class NotificationBase : LocalizedEntityBase<NotificationBase, NotificationBaseTranslation>
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public virtual Dictionary<string, string> Data { get; set; }

        public List<object> TitleArgs { get; set; }
        public List<object> BodyArgs { get; set; }

        public override NotificationBaseTranslation Content { get; set; }

        public override NotificationBase Localize(Culture culture)
        {
            base.Localize(culture);
            if (Content == null) return this;
            
            Title = LocalizeHelpers.ReplaceWithArgs(Content.Title, TitleArgs);
            Body = LocalizeHelpers.ReplaceWithArgs(Content.Body, BodyArgs);
            return this;
        }
    }


    public class NotificationBaseTranslation : Translation<NotificationBase, NotificationBaseTranslation>
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public static class NotificationBaseExtensions
    {
        public static Notification ToNotification(this NotificationBase taskNotification)
            => new Notification
            {
                Title = taskNotification.Title,
                Body = taskNotification.Body
            };
    }
}