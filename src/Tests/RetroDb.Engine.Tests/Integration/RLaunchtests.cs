using RetroDb.Engine.Frontends;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace RetroDb.Engine.Tests.Integration
{    
    public class RLaunchtests
    {
        const string RL_PATH = "TestData\\Rocket";
        //const string RL_PATH = "I:\\Rocketlauncher";

        private RocketLauncher _rl;

        public RLaunchtests()
        {
            //Need to set full path. Some methods are converting relative paths and fail to combine correctly
            var testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _rl = new RocketLauncher(Path.Combine(testDir, RL_PATH));             
        }

        [Fact]
        public async Task SetCurrentSystem()
        {
            await _rl.SetCurrentSystemAsync("MFME");
            Assert.False(string.IsNullOrWhiteSpace(_rl.CurrentSystemSettings.DefaultEmulator));
        }

        [Theory]
        [InlineData("MFME", 2)]
        public async Task GetRocketlaunchGameStats(string systemName, int expectedCount)
        {
            var stats = await _rl.GetStatsAsync(systemName);
            Assert.True(stats?.Count() == expectedCount);
        }

        [Fact(Skip = "Quick test to launch RL")]
        public void LaunchGame_Rl()
        {
            Assert.True(Directory.Exists(_rl.InstallPath));
            _rl.Launch("3D Boxing (Europe)", "Amstrad CPC");
        }

        [Fact(Skip = "Works, but only run when RL is opened")]
        public async Task ExitRocketLaunchThroughMessage()
        {
            var result = await _rl.Quit();
            Assert.True(result > 0);
        }
    }
}
