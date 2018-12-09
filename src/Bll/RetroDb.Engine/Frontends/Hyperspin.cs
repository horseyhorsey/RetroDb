using RetroDb.Data;
using RetroDb.Data.Hyperspin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RetroDb.Engine.Import
{
    internal class Hyperspin : FrontEnd
    {
        public Hyperspin(string feDirectory) : base(feDirectory)
        {

        }

        /// <summary>
        /// Gets the favorites from the system favorites text. Hyperspin only
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public override Task<IEnumerable<string>> GetFavoritesAsync(string systemName)
        {
            return Task.Run<IEnumerable<string>>(() =>
            {
                var favoritesList = new List<string>();
                var favoriteTextFile = Path.Combine(FePath, HyperspinRootPaths.Databases, systemName, "favorites.txt");
                if (!File.Exists(favoriteTextFile))
                    throw new FileNotFoundException($"Favorite file not found: {favoriteTextFile}");

                try
                {
                    using (StreamReader reader = new StreamReader(favoriteTextFile))
                    {
                        var line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                        {
                            favoritesList.Add(line);
                        }

                        return favoritesList;
                    }
                }
                catch (Exception) { throw; }
            });
        }

        /// <summary>
        /// Gets games from a Hyperspin xml
        /// </summary>
        /// <param name="dbFilePath"></param>
        /// <param name="mainMenuDb"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<Game>> GetGamesAsync(string systemName, string name = null, bool mainMenuDb = false)
        {
            IEnumerable<Game> games = null;

            return await Task.Run<IEnumerable<Game>>(() =>
            {
                var dbPath = HyperspinHelper.GetDataBaseFilePath(FePath, systemName, name);

                if (!File.Exists(dbPath))
                    throw new FileNotFoundException($"Couldn't find a database file: {dbPath}");

                //Create one system to add to all games
                var system = new GameSystem { Name = systemName };

                //Setup
                var tempGamesList = new List<Game>();
                XmlDocument xdoc = new XmlDocument();

                //Load the xml
                xdoc.Load(dbPath);

                string romName = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                    manu = string.Empty, genre = string.Empty, rating = string.Empty;
                int enabled = 0;
                int year = 0;
                string index = string.Empty;
                var i = 0;
                var lastRom = string.Empty;

                try
                {
                    //Build the games list
                    foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
                    {
                        romName = node.SelectSingleNode("@name").InnerText;

                        char s = romName[0];
                        char t;

                        if (lastRom != string.Empty)
                        {
                            t = lastRom[0];
                            if (char.ToLower(s) == char.ToLower(t))
                            {
                                index = string.Empty;
                                image = string.Empty;
                            }
                            else
                            {
                                index = "true";
                                image = char.ToLower(s).ToString();
                            }
                        }

                        if (node.SelectSingleNode("@enabled") != null)
                        {
                            if (node.SelectSingleNode("@enabled").InnerText != null)
                            {
                                enabled = Convert.ToInt32(node.SelectSingleNode("@enabled").InnerText);
                            }
                        }
                        else
                            enabled = 1;

                        if (!mainMenuDb)
                        {
                            desc = node.SelectSingleNode("description").InnerText;

                            if (node.SelectSingleNode("cloneof") != null)
                                cloneof = node.SelectSingleNode("cloneof").InnerText;
                            if (node.SelectSingleNode("crc") != null)
                                crc = node.SelectSingleNode("crc").InnerText;
                            if (node.SelectSingleNode("manufacturer") != null)
                                manu = node.SelectSingleNode("manufacturer").InnerText;
                            if (node.SelectSingleNode("year") != null)
                                if (!string.IsNullOrEmpty(node.SelectSingleNode("year").InnerText))
                                    Int32.TryParse(node.SelectSingleNode("year").InnerText, out year);

                            if (node.SelectSingleNode("genre") != null)
                            {
                                var genreName = node.SelectSingleNode("genre").InnerText;

                                genreName = genreName.Replace(@"\", "-");
                                genreName = genreName.Replace(@"/", "-");

                                genre = genreName;
                            }
                            if (node.SelectSingleNode("rating") != null)
                                rating = node.SelectSingleNode("rating").InnerText;
                        }

                        tempGamesList.Add(new Game
                        {
                            FileName = romName,
                            ShortDescription = desc,
                            Enabled = Convert.ToBoolean(enabled),
                            Genre = new Genre { Name = genre },
                            Title = desc,
                            Year = year,
                            Rating = rating,
                            Manufacturer = new Manufacturer { Name = manu },
                            System = system
                        });

                        //(name, index, image, desc, cloneof, crc, manu, year, genre, rating, enabled, _systemName));

                        lastRom = romName;
                        i++;
                    }

                    //Only sort by romname if it isn't main menu
                    if (!mainMenuDb)
                        tempGamesList.Sort((x, y) => x.FileName.CompareTo(y.FileName));

                    return tempGamesList;
                }
                catch (Exception)
                {
                    throw;
                }
            });

        }

        /// <summary>
        /// Gets GameSystem from a Hyperspin Main Menu xml
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<GameSystem>> GetSystemsAsync(string dbName = null)
        {
            return await Task.Run(() =>
             {
                 //Set system (under Main Menu), with dbname
                 if (dbName == null)
                     dbName = "Main Menu";                 
                 var xmlPath = HyperspinHelper.GetDataBaseFilePath(FePath, "Main Menu", dbName);

                 IList<GameSystem> systems = null;

                 if (!File.Exists(xmlPath))
                     throw new FileNotFoundException($"Menu Db not found. {xmlPath}");

                 using (XmlTextReader reader = new XmlTextReader(xmlPath))
                 {
                     var menuName = Path.GetFileNameWithoutExtension(xmlPath);
                     XmlDocument xdoc = new XmlDocument();
                     xdoc.Load(xmlPath);

                     int sysCount = xdoc.SelectNodes("menu/game").Count + 1;
                     systems = new List<GameSystem>();

                     while (reader.Read())
                     {
                         if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                             if (reader.HasAttributes)
                             {
                                 var meh = reader.GetAttribute("enabled");
                                 int enabled;
                                 int.TryParse(reader.GetAttribute("enabled"), out enabled);
                                 systems.Add(new GameSystem
                                 {
                                     Name = reader.GetAttribute("name"),
                                     Enabled = Convert.ToBoolean(enabled)
                                 });
                             }
                     }
                 }

                 return systems.AsEnumerable();

             });

        }
    }
}
