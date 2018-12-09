using RetroDb.Engine.Import;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetroDb.Repo.Tests.Integration
{
    public class ImportFrontendTests
    {
        private readonly FrontEndBuilder _fe;

        public ImportFrontendTests()
        {
            _fe = new FrontEndBuilder();
        }

        [Fact(Skip = "Integration")]
        public async Task ImportHyperspinSystemsToDb__InsertOrUpdate()
        {
            var hsFe = _fe.GetFrontEnd("hyperspin", "TestData\\Hyperspin");
            var systems = await hsFe.GetSystemsAsync();
            Assert.True(systems?.Count() > 0);
            
            //Insert systems
            using (var uow = new UnitOfWork(Constants.CONN_STRING))
            {
                uow.EnsureCreated();

                foreach (var system in systems)
                {
                    var existingSystem = uow.GamingSystemRepository.Get(x => x.Name == system.Name).FirstOrDefault();
                    if (existingSystem != null)
                    {
                        existingSystem.Enabled = system.Enabled;
                        uow.GamingSystemRepository.Update(existingSystem);
                    }
                    else
                    {
                        uow.GamingSystemRepository.Insert(system);
                    }
                }

                await uow.SaveAsync();
            }
        }

        [Fact(Skip = "Integration")]
        public async Task DoBulkHyperspinImport()
        {
            IBulkImport bulkImport = new BulkImport(Constants.CONN_STRING, "hyperspin", @"I:\Hyperspin", @"I:\Rocketlauncher");

            Assert.True(bulkImport?.FrontEnd?.FePath == @"I:\Hyperspin");

            bulkImport.ImportProgressChanged += BulkImport_ImportProgressChanged; ;
            await bulkImport.ImportAsync();
        }

        private void BulkImport_ImportProgressChanged(string msg)
        {
            Debug.WriteLine(msg);
        }
    }
}
