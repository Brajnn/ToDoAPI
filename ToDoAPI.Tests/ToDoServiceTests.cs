using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Data;
using ToDoAPI.Models;
using ToDoAPI.Services;

namespace ToDoAPI.Tests
{
    public class ToDoServiceTests
    {
        private readonly ToDoDbContext _context;
        private readonly ToDoService _service;

        public ToDoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: "ToDoDatabase")
                .Options;

            _context = new ToDoDbContext(options);
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated();


            _service = new ToDoService(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {

            _context.ToDoItems.RemoveRange(_context.ToDoItems);
            _context.SaveChanges();


            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); 
            var midWeek = startOfWeek.AddDays(3);
            var endOfWeek = startOfWeek.AddDays(6); 
            _context.ToDoItems.AddRange(new List<ToDoItem>
        {
            new ToDoItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                ExpiryDate = today,
                PercentComplete = 50,
                IsDone = false
            },
            new ToDoItem
            {
                Id = 2,
                Title = "Task 2",
                Description = "Description 2",
                ExpiryDate = today.AddDays(1),
                PercentComplete = 65,
                IsDone = false
            },
            new ToDoItem
            {
                Id = 3,
                Title = "Task 3",
                Description = "Description 3",
                ExpiryDate = midWeek,
                PercentComplete = 0,
                IsDone = false
            }
        });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllTodosAsync_ReturnsAllTodos()
        {
            // Act
            var result = await _service.GetAllTodosAsync();

            // Assert
            var todosList = result.ToList();
            Assert.Equal(3, todosList.Count); 
            Assert.Equal("Task 1", todosList[0].Title);
            Assert.Equal("Task 2", todosList[1].Title);
            Assert.Equal("Task 3", todosList[2].Title);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithValidId_ReturnsTodo()
        {
            // Act
            var result = await _service.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Task 1", result?.Title);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _service.GetTodoByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetIncomingTodosAsync_TodayFilter_ReturnsTodayTasks()
        {
            // Act
            var result = await _service.GetIncomingTodosAsync("today");

            // Assert
            Assert.Single(result);
            Assert.Equal(DateTime.Today, result.First().ExpiryDate.Date);
        }

        [Fact]
        public async Task GetIncomingTodosAsync_TomorrowFilter_ReturnsSingleTomorrowTask()
        {
            // Act
            var result = await _service.GetIncomingTodosAsync("tomorrow");

            // Assert
            Assert.Single(result);
            var task = result.First();
            Assert.Equal("Task 2", task.Title);
            Assert.Equal(DateTime.Today.AddDays(1), task.ExpiryDate.Date);
        }
        [Fact]
        public async Task GetIncomingTodosAsync_WeekFilter_ReturnsThisWeekTasks()
        {
            // Act
            var result = await _service.GetIncomingTodosAsync("week");

            // Assert
            Assert.Equal(3, result.Count()); 
            Assert.All(result, task =>
            {
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                var endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
                Assert.InRange(task.ExpiryDate, startOfWeek, endOfWeek);
            });
        }

        [Fact]
        public async Task GetIncomingTodosAsync_InvalidFilter_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetIncomingTodosAsync("invalid_filter"));
        }

        [Fact]
        public async Task CreateTodoAsync_AddsNewTodo()
        {
            // Arrange
            var newTodo = new ToDoItem { Title = "New Task", Description = "New Description", ExpiryDate = DateTime.Today.AddDays(3) };

            // Act
            var createdTodo = await _service.CreateTodoAsync(newTodo);

            // Assert
            Assert.NotNull(createdTodo);
            Assert.Equal("New Task", createdTodo.Title);
            Assert.Equal(4, _context.ToDoItems.Count());
        }

        [Fact]
        public async Task UpdateTodoAsync_WithExistingTodo_UpdatesTodo()
        {
            // Arrange
            var id = 1;
            var updatedTodo = new ToDoItem
            {
                Id = id,
                Title = "Updated Task",
                Description = "Updated Description",
                ExpiryDate = DateTime.Today.AddDays(5),
                PercentComplete = 75,
                IsDone = false
            };

            // Act
            var result = await _service.UpdateTodoAsync(updatedTodo);
            var retrievedTodo = await _service.GetTodoByIdAsync(id);

            // Assert
            Assert.NotNull(result); 
            Assert.NotNull(retrievedTodo); 
            Assert.Equal("Updated Task", retrievedTodo!.Title);
            Assert.Equal("Updated Description", retrievedTodo.Description);
            Assert.Equal(DateTime.Today.AddDays(5), retrievedTodo.ExpiryDate);
            Assert.Equal(75, retrievedTodo.PercentComplete);
            Assert.False(retrievedTodo.IsDone);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithNonExistingTodo_ReturnsNull()
        {
            // Arrange
            var updatedTodo = new ToDoItem { Id = 99, Title = "Non-Existing", Description = "Non-Existing", ExpiryDate = DateTime.Today };

            // Act
            var result = await _service.UpdateTodoAsync(updatedTodo);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetTodoPercentCompleteAsync_SetsPercentCompleteAndMarksAsDone()
        {
            // Arrange
            var id = 2;
            var percentComplete = 100;

            // Act
            await _service.SetTodoPercentCompleteAsync(id, percentComplete);
            var updatedTodo = await _service.GetTodoByIdAsync(id);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal(100, updatedTodo.PercentComplete);
            Assert.True(updatedTodo.IsDone);
        }

        [Fact]
        public async Task DeleteTodoByIdAsync_RemovesTodo()
        {
            // Arrange
            var id = 1;

            // Act
            await _service.DeleteTodoByIdAsync(id);
            var deletedTodo = await _service.GetTodoByIdAsync(id); 
            var remainingTodos = await _service.GetAllTodosAsync();

            // Assert
            Assert.Null(deletedTodo);
            Assert.Equal(2, remainingTodos.Count());
        }


        [Fact]
        public async Task MarkTodoAsDoneAsync_MarksTodoAsDone()
        {
            // Arrange
            var id = 1;

            // Act
            await _service.MarkTodoAsDoneAsync(id);
            var updatedTodo = await _service.GetTodoByIdAsync(id);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.True(updatedTodo.IsDone);
            Assert.Equal(100, updatedTodo.PercentComplete);
        }
    }
}
