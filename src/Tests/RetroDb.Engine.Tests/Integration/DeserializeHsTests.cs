using RetroDb.Data;
using RetroDb.Engine.Import;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetroDb.Engine.Tests.Integration
{
    /// <summary>
    /// Import game entries from frontend files
    /// </summary>
    public class DeserializeHsTests
    {
        private FrontEndBuilder _builder;
        private IFrontEnd _hypserspin;

        public DeserializeHsTests()
        {
            _builder = new FrontEndBuilder();
            _hypserspin = _builder.GetFrontEnd("hyperspin", "TestData\\Hyperspin");
        }

        [Fact]
        public async Task ImportHyperspin_CpcGamesCount_EqualsTo_5()
        {            
            var games = await _hypserspin.GetGamesAsync("Amstrad CPC");
            Assert.True(games.Count() == 5);
        }

        [Fact]
        public async Task ImportHyperspin_MenusCount_EqualsTo_3()
        {            
            var games = await _hypserspin.GetSystemsAsync();
            Assert.True(games.Count() == 3);
            Assert.True(games.ElementAt(0).Enabled);
        }

        [Theory]
        [InlineData("Amstrad CPC", 5)]
        public async Task GetHyperspinFavorites(string systemName, int expectedCount)
        {
            var faves = await _hypserspin.GetFavoritesAsync(systemName);

            Assert.True(faves.Count() == expectedCount);
        }
    }
}
