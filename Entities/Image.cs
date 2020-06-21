using System.Text.Json.Serialization;

namespace ExtremeInsiders.Entities
{
  public class Image
  {
    [JsonIgnore]
    public int Id { get; set; }
    public string Path { get; set; }
  }
}