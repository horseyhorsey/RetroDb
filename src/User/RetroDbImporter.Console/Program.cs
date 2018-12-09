using RetroDb.Repo;
using System;
using System.Threading.Tasks;

namespace RetroDbImporter.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length <= 0)
                    throw new ArgumentNullException("Give the path to Hyperspin.' RetroDbImporter C:\\Hyperspin'");

                if (args.Length <= 1)
                    throw new ArgumentNullException("Give the path to Rocketlauncher.' RetroDbImporter C:\\Hyperspin I:\\Rocketlauncher'");

                IBulkImport bulkImport = new BulkImport(@"Data Source=RetroDb.db", "hyperspin", args[0], args[1]);

                bulkImport.ImportProgressChanged += BulkImport_ImportProgressChanged; ;

                await bulkImport.ImportAsync();
                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                System.Console.Read();
            }
        }

        private static void BulkImport_ImportProgressChanged(string msg)
        {
            System.Console.WriteLine(msg);
        }
    }
}
