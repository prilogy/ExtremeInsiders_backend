using ExtremeInsiders.Models;

namespace ExtremeInsiders.Interfaces
{
    public interface IWithUrlAndDuration : ITranslationWithUrl
    {
        string Duration { get; set; }
    }
}