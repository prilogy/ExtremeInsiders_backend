using System;

namespace ExtremeInsiders.Models
{
    public class VimeoVideoConfig
    {
        public VimeoVideo Video { get; set; }
    }

    public class VimeoVideo
    {
        public int? Duration { get; set; }
        public TimeSpan? DurationAsTimeSpan =>
            Duration != null ? (TimeSpan?) TimeSpan.FromSeconds(Duration.Value) : null;

        public string DurationFmt => DurationAsTimeSpan?.ToString(@"hh\:mm\:ss");
    }
}