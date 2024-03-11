using TaskManagerApi.Facades;
using TaskManagerApi.Services.Notifications;

namespace TaskManagerApi.Tests
{
    [TestFixture]
    public class PerformanceMonitorTests
    {
        [Test]
        public async Task StartMonitoring_AlertWhenLimitExceeded()
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

            var counter = new PerformanceMonitor(
                new PerformanceCounterFacade(category, name, instanceName) { GetCounterNextValues = () => exceedLimit },
                limit,
                countMillisecondsTimeout,
                pauseAfterNotifyMilliseconds);

            Action<string> notify = alert => alertReceived = true;

            // Act
            counter.StartMonitoring(notify);
            Thread.Sleep(500);
            counter.StopMonitoring();

            // Assert
            Assert.That(alertReceived, Is.True);
        }

        [Test]
        public async Task StopMonitoring_Stopped()
        {
            // Arrange
            PerformanceMonitor counter = new PerformanceMonitor(
                new PerformanceCounterFacade("Processor", "% Processor Time", "_Total"),
                100,
                10,
                10);

            // Act
            counter.StartMonitoring(alert => { });
            Thread.Sleep(100);
            counter.StopMonitoring();

            // Assert
            Assert.That(counter.IsCancellationRequested, Is.True);
        }
    }
}
