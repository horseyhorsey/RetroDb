using System.IO;

namespace RetroDb.Data.Hyperspin
{
    public static class HyperspinRootPaths
    {
        public const string Databases = "Databases";
        public const string Media = "Media";
        public const string Settings = "Settings";
        public const string Sound = "Sound";
        public const string Themes = "Themes";
        public const string Video = "Video";
    }

    public static class HyperspinImages
    {
        public const string Artwork1 = @"Images\Artwork1";
        public const string Artwork2 = @"Images\Artwork2";
        public const string Artwork3 = @"Images\Artwork3";
        public const string Artwork4 = @"Images\Artwork4";
        public const string Backgrounds = @"Images\Backgrounds";
        public const string GenreWheel = @"Images\Genre\Wheel";
        public const string GenreBackgrounds = @"Images\Genre\Backgrounds";
        public const string Letters = @"Images\Letters";
        public const string Pointer = @"Images\Other";
        public const string Special = @"Images\Special";
        public const string Wheels = @"Images\Wheel";
    }

    public static class HyperspinSound
    {
        public const string WheelSounds = @"Sound\Wheel Sounds";
        public const string BackgroundMusic = @"Sound\Background Music";
        public const string SystemStart = @"Sound\System Start";
        public const string SystemExit = @"Sound\System Exit";

    }

    public static class HyperspinHelper
    {
        /// <summary>
        /// Returns the full path for the given mediatype with the system set to this.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public static string GetMediaDirectory(string frontendPath, string systemName, HsMediaType mediaType)
        {
            switch (mediaType)
            {
                case HsMediaType.Artwork1:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Artwork1);
                case HsMediaType.Artwork2:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Artwork2);
                case HsMediaType.Artwork3:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Artwork3);
                case HsMediaType.Artwork4:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Artwork4);
                case HsMediaType.Backgrounds:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Backgrounds);
                case HsMediaType.Wheel:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinImages.Wheels);
                case HsMediaType.Video:
                    return Path.Combine(frontendPath, HyperspinRootPaths.Media, systemName, HyperspinRootPaths.Video);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the database xml path. Retrives main entry database if name isn't given.
        /// </summary>
        /// <param name="fePath"></param>
        /// <param name="systemName">System name</param>
        /// <param name="name">name of the database, can include the extension</param>
        /// <returns>the file location to a Hyperspin database</returns>
        public static string GetDataBaseFilePath(string fePath, string systemName, string name = null)
        {
            //Set to system name if name not given
            if (name == null)
                name = systemName;

            //Add xml extension if not existing
            if (Path.GetExtension(name) != ".xml")
                name = $"{name}.xml";
            
            return Path.Combine(fePath, HyperspinRootPaths.Databases, systemName, name);
        }
    }

    public enum HsMediaType
    {
        Artwork1,
        Artwork2,
        Artwork3,
        Artwork4,
        Backgrounds,
        Wheel,
        Video
    }
}
