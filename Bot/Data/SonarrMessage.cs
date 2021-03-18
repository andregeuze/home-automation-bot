using System;

namespace Bot.Data
{
    public class SonarrMessage : LiteDbEntity
    {
        public Series series { get; set; }
        public Episode[] episodes { get; set; }
        public Episodefile episodeFile { get; set; }
        public bool isUpgrade { get; set; }
        public string downloadClient { get; set; }
        public string downloadId { get; set; }
        public string eventType { get; set; }
    }

    public class Series
    {
        public int id { get; set; }
        public string title { get; set; }
        public string path { get; set; }
        public int tvdbId { get; set; }
        public int tvMazeId { get; set; }
        public string imdbId { get; set; }
        public string type { get; set; }
    }

    public class Episodefile
    {
        public int id { get; set; }
        public string relativePath { get; set; }
        public string path { get; set; }
        public string quality { get; set; }
        public int qualityVersion { get; set; }
        public string releaseGroup { get; set; }
        public string sceneName { get; set; }
        public int size { get; set; }
    }

    public class Episode
    {
        public int id { get; set; }
        public int episodeNumber { get; set; }
        public int seasonNumber { get; set; }
        public string title { get; set; }
        public string airDate { get; set; }
        public DateTime airDateUtc { get; set; }
    }
}
