using TaskManagerApi.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Tests
{
    [TestFixture]
    public class ProcessStorageTests
    {
        private ProcessesStorage _processesStorage;

        [SetUp]
        public void SetUp()
        {
            _processesStorage = new ProcessesStorage();
        }


        [Test]
        public async Task GetChangesAsync_Should_Return_Empty_If_No_Processes()
        {
            var result = await _processesStorage.GetChangesAsync(new List<ProcessModel>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetChangesAsync_Should_Return_Added_Processes()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            // Act
            var result = await _processesStorage.GetChangesAsync(processes);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(p => p.Action == ProcessActionModel.ActionEnum.Add), Is.True);
        }

        [Test]
        public async Task GetChangesAsync_Should_Return_Deleted_Processes()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            await _processesStorage.GetChangesAsync(processes);

            var existingProcesses = processes.Take(1);

            // Act
            var result = await _processesStorage.GetChangesAsync(existingProcesses);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.All(p => p.Action == ProcessActionModel.ActionEnum.Delete), Is.True);
        }

        [Test]
        public async Task GetChangesAsync_Should_Return_Updated_Processes()
        {
            // Arrange
            var existingProcesses = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            await _processesStorage.GetChangesAsync(existingProcesses);

            var updatedProcesses = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "UpdatedProcess1", NonpagedSystemMemorySize64 = 150 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            // Act
            var result = await _processesStorage.GetChangesAsync(updatedProcesses);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.All(p => p.Action == ProcessActionModel.ActionEnum.Update), Is.True);
        }

        [Test]
        public async Task GetChangesAsync_Should_Return_Empty_If_Processes_Are_Unchanged()
        {
            // Arrange
            var processes = new List<ProcessModel>
            {
                new ProcessModel { Id = 1, ProcessName = "Process1", NonpagedSystemMemorySize64 = 100 },
                new ProcessModel { Id = 2, ProcessName = "Process2", NonpagedSystemMemorySize64 = 200 }
            };

            await _processesStorage.GetChangesAsync(processes);

            var existingProcesses = processes.ToList();

            // Act
            var result = await _processesStorage.GetChangesAsync(existingProcesses);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
