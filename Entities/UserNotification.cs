using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
    public class UserNotification : IWithId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime? DateCreated { get; set; }
        
        public int? OpenId { get; set; }
        public UserNotificationOpenTypes? OpenType { get; set; }
        
        public string Title { get; set; }
        public string Body { get; set; }
        
        public int? CultureId { get; set; }
        public virtual Culture Culture { get; set; }

        public NotificationBase ToNotificationBase()
            => new NotificationBase
            {
                Title = Title,
                Body = Body,
                Data = OpenId != null && OpenType != null ? new Dictionary<string, string>
                {
                    { "open_id", OpenId.Value.ToString() },
                    {
                        "open_id_type", OpenType switch
                        {
                            UserNotificationOpenTypes.Sport => "sport",
                            UserNotificationOpenTypes.Playlist => "playlist",
                            UserNotificationOpenTypes.Movie => "movie",
                            UserNotificationOpenTypes.Video => "video",
                            _ => "Error"
                        }
                    }
                } : null
            };
    }

    public enum UserNotificationOpenTypes
    {
        Sport = 0,
        Playlist = 1,
        Movie = 2,
        Video = 3
    }
}