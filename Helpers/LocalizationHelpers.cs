using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Helpers
{
    public static class LocalizeHelpers
    {
        public static string ReplaceWithArgs(string v, List<object> args, string nullValue = "-")
        {
            if (args == null || args.Count == 0 || v == null) return v;
            foreach (var (arg, index) in args.WithIndex())
                v = v.Replace("{" + index + "}", (arg ?? nullValue).ToString());
            return v;
        }

        private static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }

    public interface ILocalizable<out T>
    {
        T Localize(Culture culture);
    }

    public interface ILocalizedEntity<T, TR> : ILocalizable<T>
        where T : class, new()
        where TR : Translation<T, TR>, new()
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        TranslationCollection<T, TR> Translations { get; set; }
        TR Content { get; set; }
    }

    public abstract class LocalizedEntityBase<T, TR> : EntityBase, ILocalizedEntity<T, TR>
        where T : class, new()
        where TR : Translation<T, TR>, new()
    {
        [ForeignKey("BaseEntityId")]
        [JsonIgnore]
        public virtual TranslationCollection<T, TR> Translations { get; set; }

        [NotMapped] public virtual TR Content { get; set; }

        public virtual T Localize(Culture culture)
        {
            if (!Translations.Any() || culture == null) return this as T;
            Content = Translations[culture];
            return this as T;
        }
    }

    public abstract class Translation<T, TR>
        where T : class, new()
        where TR : Translation<T, TR>, new()
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BaseEntityId { get; set; }
        [JsonIgnore] public virtual T BaseEntity { get; set; }

        public int CultureId { get; set; }
        [JsonIgnore] public virtual Culture Culture { get; set; }
    }

    public class TranslationCollection<T, TR> : Collection<TR>
        where T : class, new()
        where TR : Translation<T, TR>, new()
    {
        public TR this[Culture culture]
        {
            get => this.FirstOrDefault(x => x.CultureId == culture.Id)
                   ?? this.FirstOrDefault();
            set
            {
                var translation = this.FirstOrDefault(x => (x.Culture?.Id ?? x.CultureId) == culture.Id);
                if (translation != null)
                {
                    Remove(translation);
                }

                value.CultureId = culture.Id;
                Add(value);
            }
        }


        public bool HasCulture(Culture culture)
        {
            return this.Any(x => (x.Culture?.Id ?? x.CultureId) == culture.Id);
        }
    }
}