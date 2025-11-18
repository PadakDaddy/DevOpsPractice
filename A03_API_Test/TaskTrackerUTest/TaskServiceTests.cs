using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskTrackerAPITest
{
    [TestFixture]
    public class TaskApiTests
    {
        private HttpClient _client;
        private const string BaseUrl = "https://localhost:7191/api";

        [SetUp]
        public void Setup()
        {
            // Create HttpClientHandler that ignores SSL certificate validation for local testing
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        // ==================== CREATE TASK TESTS (3 tests) ====================

        /// <summary>
        /// Test 1: Create a task with valid data
        /// Acceptance Test - Verifies task is successfully created via API
        /// Expected: 201 Created
        /// </summary>
        [Test]
        public async Task CreateTask_Valid_201()
        {
            // Arrange
            var newTask = new
            {
                id = 1,
                title = "Test Task",
                assignee = "John",
                priority = 1
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(newTask),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/tasks", jsonContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(201),
                "Valid task creation should return 201 Created");
        }

        /// <summary>
        /// Test 2: Attempt to create a task with empty title
        /// Programmer Test - Verifies title validation
        /// Expected: 400 Bad Request
        /// </summary>
        [Test]
        public async Task CreateTask_EmptyTitle_400()
        {
            // Arrange
            var invalidTask = new
            {
                id = 2,
                title = "",
                assignee = "John",
                priority = 1
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(invalidTask),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/tasks", jsonContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Empty title should return 400 Bad Request");
        }

        /// <summary>
        /// Test 3: Attempt to create a task with null title
        /// Programmer Test - Verifies null title validation
        /// Expected: 400 Bad Request
        /// </summary>
        [Test]
        public async Task CreateTask_NullTitle_400()
        {
            // Arrange
            var invalidTask = new
            {
                id = 3,
                title = (string?)null,
                assignee = "John",
                priority = 1
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(invalidTask),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/tasks", jsonContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Null title should return 400 Bad Request");
        }

        // ==================== GET ALL TASKS TEST (1 test) ====================

        /// <summary>
        /// Test 4: Retrieve all tasks
        /// Acceptance Test - Verifies API returns all tasks successfully
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task GetAllTasks_Success_200()
        {
            // Arrange
            var query = $"{BaseUrl}/tasks";

            // Act
            var response = await _client.GetAsync(query);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Get all tasks should return 200 OK");
        }

        // ==================== GET TASK BY ID TEST (1 test) ====================

        /// <summary>
        /// Test 5: Retrieve a specific task by ID
        /// Acceptance Test - Verifies correct task retrieval by ID
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task GetTaskById_Valid_200()
        {
            // Arrange - First create a task to retrieve
            var newTask = new { id = 100, title = "Retrieve Test", assignee = "Alice", priority = 2 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(newTask),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Act - Then retrieve it by ID
            var response = await _client.GetAsync($"{BaseUrl}/tasks?id=100");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Get task by valid ID should return 200 OK");
        }

        // ==================== SEARCH BY ASSIGNEE TEST (1 test) ====================

        /// <summary>
        /// Test 6: Search tasks by assignee name
        /// Acceptance Test - Verifies tasks can be filtered by assignee
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task SearchByAssignee_Valid_200()
        {
            // Arrange
            var query = $"{BaseUrl}/tasks?assignee=John";

            // Act
            var response = await _client.GetAsync(query);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Search by assignee should return 200 OK");
        }

        // ==================== UPDATE TASK TESTS (3 tests) ====================

        /// <summary>
        /// Test 7: Update a task with valid data
        /// Acceptance Test - Verifies task can be successfully updated
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task UpdateTask_Valid_200()
        {
            // Arrange - Create a task first
            var originalTask = new { id = 10, title = "Original", assignee = "Kim", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(originalTask),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Now update it
            var updateData = new { title = "Updated" };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync($"{BaseUrl}/tasks/10", updateContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Valid task update should return 200 OK");
        }

        /// <summary>
        /// Test 8: Attempt to update a task with empty title
        /// Programmer Test - Verifies update validation for empty title
        /// Expected: 400 Bad Request
        /// </summary>
        [Test]
        public async Task UpdateTask_EmptyTitle_400()
        {
            // Arrange - Create a task first
            var originalTask = new { id = 11, title = "Test", assignee = "Kim", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(originalTask),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Now attempt update with empty title
            var updateData = new { title = "" };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync($"{BaseUrl}/tasks/11", updateContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Update with empty title should return 400 Bad Request");
        }

        /// <summary>
        /// Test 9: Attempt to update a non-existent task
        /// Programmer Test - Verifies error handling for missing task
        /// Expected: 404 Not Found
        /// </summary>
        [Test]
        public async Task UpdateTask_NonExistent_404()
        {
            // Arrange
            var updateData = new { title = "Updated Title" };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateData),
                Encoding.UTF8,
                "application/json"
            );

            // Act - Try to update non-existent task
            var response = await _client.PatchAsync($"{BaseUrl}/tasks/99999", updateContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(404),
                "Update non-existent task should return 404 Not Found");
        }

        // ==================== DELETE TASK TESTS (3 tests) ====================

        /// <summary>
        /// Test 10: Delete an existing task
        /// Acceptance Test - Verifies task can be successfully deleted
        /// Expected: 204 No Content
        /// </summary>
        [Test]
        public async Task DeleteTask_Valid_204()
        {
            // Arrange - Create a task first
            var taskToDelete = new { id = 20, title = "Delete Me", assignee = "Lee", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(taskToDelete),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/tasks/20");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(204),
                "Delete existing task should return 204 No Content");
        }

        /// <summary>
        /// Test 11: Attempt to delete a non-existent task
        /// Programmer Test - Verifies error handling for missing task
        /// Expected: 404 Not Found
        /// </summary>
        [Test]
        public async Task DeleteTask_NonExistent_404()
        {
            // Arrange
            int nonExistentId = 999;

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/tasks/{nonExistentId}");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(404),
                "Delete non-existent task should return 404 Not Found");
        }

        /// <summary>
        /// Test 12: Attempt to delete the same task twice
        /// Programmer Test - Verifies task cannot be deleted twice
        /// Expected: 404 Not Found on second deletion
        /// </summary>
        [Test]
        public async Task DeleteTask_Twice_404()
        {
            // Arrange - Create and delete a task
            var taskToDelete = new { id = 21, title = "Delete Twice", assignee = "Park", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(taskToDelete),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);
            await _client.DeleteAsync($"{BaseUrl}/tasks/21");

            // Act - Try to delete again
            var response = await _client.DeleteAsync($"{BaseUrl}/tasks/21");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(404),
                "Deleting already deleted task should return 404 Not Found");
        }

        // ==================== ASSIGNEE MANAGEMENT TESTS (3 tests) ====================

        /// <summary>
        /// Test 13: Add an assignee to a task
        /// Acceptance Test - Verifies assignee can be successfully added
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task AddAssignee_Valid_200()
        {
            // Arrange - Create a task first
            var newTask = new { id = 30, title = "Assignee Task", assignee = "", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(newTask),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Now add assignee
            var assigneeData = new { name = "Alice" };
            var assigneeContent = new StringContent(
                JsonSerializer.Serialize(assigneeData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/tasks/30/assignee", assigneeContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Add valid assignee should return 200 OK");
        }

        /// <summary>
        /// Test 14: Attempt to add empty assignee name
        /// Programmer Test - Verifies assignee name validation
        /// Expected: 400 Bad Request
        /// </summary>
        [Test]
        public async Task AddAssignee_EmptyName_400()
        {
            // Arrange
            var assigneeData = new { name = "" };
            var assigneeContent = new StringContent(
                JsonSerializer.Serialize(assigneeData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/tasks/31/assignee", assigneeContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Empty assignee name should return 400 Bad Request");
        }

        /// <summary>
        /// Test 15: Remove assignee from a task
        /// Acceptance Test - Verifies assignee can be successfully removed
        /// Expected: 204 No Content
        /// </summary>
        [Test]
        public async Task RemoveAssignee_Valid_204()
        {
            // Arrange - Create task with assignee
            var taskWithAssignee = new { id = 32, title = "Assigned Task", assignee = "Bob", priority = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(taskWithAssignee),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/tasks/32/assignee");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(204),
                "Remove assignee should return 204 No Content");
        }

        // ==================== PRIORITY MANAGEMENT TESTS (3 tests) ====================

        /// <summary>
        /// Test 16: Set priority to a valid value (0-4)
        /// Acceptance Test - Verifies priority can be successfully set
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task SetPriority_Valid_200()
        {
            // Arrange - Create a task first
            var newTask = new { id = 40, title = "Priority Task", assignee = "Charlie", priority = 0 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(newTask),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Now set priority
            var priorityContent = new StringContent(
                JsonSerializer.Serialize(3),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/tasks/40/priority", priorityContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Set valid priority should return 200 OK");
        }

        /// <summary>
        /// Test 17: Attempt to set priority outside valid range
        /// Programmer Test - Verifies priority range validation
        /// Expected: 400 Bad Request
        /// </summary>
        [Test]
        public async Task SetPriority_OutOfRange_400()
        {
            // Arrange
            var priorityContent = new StringContent(
                JsonSerializer.Serialize(10),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/tasks/41/priority", priorityContent);

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Out of range priority should return 400 Bad Request");
        }

        /// <summary>
        /// Test 18: Remove priority from a task
        /// Acceptance Test - Verifies priority can be successfully removed
        /// Expected: 200 OK
        /// </summary>
        [Test]
        public async Task RemovePriority_Valid_200()
        {
            // Arrange - Create task with priority
            var taskWithPriority = new { id = 42, title = "High Priority Task", assignee = "Dana", priority = 4 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(taskWithPriority),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync($"{BaseUrl}/tasks", createContent);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/tasks/42/priority");

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Remove priority should return 200 OK");
        }
    }
}