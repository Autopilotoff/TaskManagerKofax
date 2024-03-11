using System.Diagnostics;
using TaskManagerApi.Models;
using TaskManagerApi.Proxies;
using TaskManagerApi.Services.Processes;

namespace TaskManagerApi.Tests
{
    [TestFixture]
    public class SingletonProcessesStorageTests
    {
        [Test]
        public async Task GetChanges_Should_Return_Empty_If_No_Processes()
        {
            var storage = new SingletonProcessesStorage(new MockProcessesProxy());
            var result = storage.GetChanges();
            Assert.That(result.AddedProcesses, Is.Empty);
            Assert.That(result.UpdatedProcesses, Is.Empty);
            Assert.That(result.DeletedProcesses, Is.Empty);
        }

        [Test]
        public async Task GetInitialProcesses_Should_Return_Added_Processes()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            var mockProcessesProxy = new MockProcessesProxy();
            mockProcessesProxy.Processes = processes;
            var storage = new SingletonProcessesStorage(mockProcessesProxy);

            // Act
            var result = storage.GetInitialProcesses();

            // Assert
            Assert.That(result.AddedProcesses.Count(), Is.EqualTo(2));
            Assert.That(result.UpdatedProcesses, Is.Null);
            Assert.That(result.DeletedProcesses, Is.Null);
        }

        [Test]
        public async Task GetChanges_Should_Return_Deleted_Processes()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            var mockProcessesProxy = new MockProcessesProxy();
            mockProcessesProxy.Processes = processes;
            var storage = new SingletonProcessesStorage(mockProcessesProxy);

            storage.GetChanges();

            var existingProcesses = processes.Take(1);
            mockProcessesProxy.Processes = existingProcesses;

            // Act
            var result = storage.GetChanges();

            // Assert
            Assert.That(result.DeletedProcesses.Count(), Is.EqualTo(1));
            Assert.That(result.UpdatedProcesses, Is.Empty);
            Assert.That(result.AddedProcesses, Is.Empty);
        }

        [Test]
        public async Task GetChanges_Should_Return_Updated_Processes()
        {
            // Arrange
            var existingProcesses = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            var mockProcessesProxy = new MockProcessesProxy();
            mockProcessesProxy.Processes = existingProcesses;
            var storage = new SingletonProcessesStorage(mockProcessesProxy);

            storage.GetChanges();

            var updatedProcesses = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "UpdatedProcess1", NonpagedSystemMemorySize64 = 150 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };
            mockProcessesProxy.Processes = updatedProcesses;

            // Act
            var result = storage.GetChanges();

            // Assert
            Assert.That(result.UpdatedProcesses.Count(), Is.EqualTo(1));
            Assert.That(result.DeletedProcesses, Is.Empty);
            Assert.That(result.AddedProcesses, Is.Empty);
        }

        [Test]
        public async Task GetChanges_Should_Return_Empty_If_Processes_Are_Unchanged()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            var mockProcessesProxy = new MockProcessesProxy();
            mockProcessesProxy.Processes = processes;
            var storage = new SingletonProcessesStorage(mockProcessesProxy);

            storage.GetChanges();

            var existingProcesses = processes.ToList();
            mockProcessesProxy.Processes = existingProcesses;

            // Act
            var result = storage.GetChanges();

            // Assert
            Assert.That(result.AddedProcesses, Is.Empty);
            Assert.That(result.UpdatedProcesses, Is.Empty);
            Assert.That(result.DeletedProcesses, Is.Empty);
        }

        internal class MockProcessesProxy : IProcessesProxy
        {
            public IEnumerable<ProcessModel> Processes { get ; set; } = new List<ProcessModel>();
                
            public ProcessModel[] GetCurrentProcesses()
            {
                return Processes.ToArray();
            }
        }
    }
}
