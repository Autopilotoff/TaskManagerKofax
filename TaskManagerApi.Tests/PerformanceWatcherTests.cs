using TaskManagerApi.Facades;
using TaskManagerApi.Services.Notifications;

namespace TaskManagerApi.Tests
{
    [TestFixture]
    public class PerformanceWatcherTests
    {
        [Test]
        public async Task StartWatchingAsync_AlertWhenLimitExceeded()
        {
            // Arrange
            int limit = 100;
            string category = "Processor";
            string name = "% Processor Time";
            string instanceName = "_Total";
            int countMillisecondsTimeout = 10;
            int pauseAfterNotifyMilliseconds = 10;
            int exceedLimit = 150;
            bool alertReceived = false;

            var counter = new PerformanceWatcher(
                new PerformanceCounterFacade(category, name, instanceName) { GetCounterNextValues = () => exceedLimit },
                limit,
                countMillisecondsTimeout,
                pauseAfterNotifyMilliseconds);

            Action<string> notify = alert => alertReceived = true;

            // Act
            counter.StartWatching(notify);
            Thread.Sleep(500);
            counter.StopWatching();

            // Assert
            Assert.That(alertReceived, Is.True);
        }

        [Test]
        public async Task StopWatchingAsync_StopsWatching()
        {
            // Arrange
            PerformanceWatcher counter = new PerformanceWatcher(
                new PerformanceCounterFacade("Processor", "% Processor Time", "_Total"),
                100,
                10,
                10);

            // Act
            counter.StartWatching(alert => { });
            Thread.Sleep(100);
            counter.StopWatching();

            // Assert
            Assert.That(counter.IsCancellationRequested, Is.True);
        }
    }
}
