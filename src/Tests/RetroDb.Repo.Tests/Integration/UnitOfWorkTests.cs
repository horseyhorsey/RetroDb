using Xunit;

namespace RetroDb.Repo.Tests.Integration
{
    public class UnitOfWorkTests
    {
        private bool _created;

        public UnitOfWorkTests()
        {
            if (!_created)
            {
                using (var uow = new UnitOfWork(Constants.CONN_STRING))
                {                    
                    _created = uow.EnsureCreated();
                }                
            }
        }

        [Fact(Skip = "First test")]
        public void InsertNintendoSystem()
        {
            using (var uow = new UnitOfWork(Constants.CONN_STRING))
            {
                try
                {
                    uow.GamingSystemRepository.Insert(new Data.GameSystem { Name = "Nintendo" });
                    uow.Save();
                }
                catch (System.Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
