namespace Bot.Data
{
    public class RadarrMessage : LiteDbEntity
    {
        public Movie movie { get; set; }
        public Remotemovie remoteMovie { get; set; }
        public Moviefile movieFile { get; set; }
        public bool isUpgrade { get; set; }
        public string downloadId { get; set; }
        public Deletedfile[] deletedFiles { get; set; }
        public string eventType { get; set; }
    }

    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string releaseDate { get; set; }
        public string folderPath { get; set; }
        public int tmdbId { get; set; }
        public string imdbId { get; set; }
    }

    public class Remotemovie
    {
        public int tmdbId { get; set; }
        public string imdbId { get; set; }
        public string title { get; set; }
        public int year { get; set; }
    }

    public class Moviefile
    {
        public int id { get; set; }
        public string relativePath { get; set; }
        public string path { get; set; }
        public string quality { get; set; }
        public int qualityVersion { get; set; }
        public string releaseGroup { get; set; }
        public string sceneName { get; set; }
        public long size { get; set; }
    }

    public class Deletedfile
    {
        public int id { get; set; }
        public string relativePath { get; set; }
        public string path { get; set; }
        public string quality { get; set; }
        public int qualityVersion { get; set; }
        public string releaseGroup { get; set; }
        public string sceneName { get; set; }
        public long size { get; set; }
    }

}
