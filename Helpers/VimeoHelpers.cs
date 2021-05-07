using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Helpers
{
    public static class VimeoHelpers
    {
        public static async Task<VimeoVideoConfig> GetVideoAsync(int? id)
        {
            if (id == null) return null;
            using var client = new HttpClient();
            
            var url = "https://player.vimeo.com/video/" + id + "/config";
            try
            {
                var result = await client.GetAsync(url);
                var videoConfig = JsonConvert.DeserializeObject<VimeoVideoConfig>(await result.Content.ReadAsStringAsync());
                return videoConfig;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<VimeoVideoConfig> GetVideoAsync(string url)
        {
            if (url == null || !url.Contains("vimeo")) return null;
            var id = int.TryParse(Regex.Match(url, @"\d+").Value, out var x) ? (int?)x : null;
            return await GetVideoAsync(id);
        }
    }
}