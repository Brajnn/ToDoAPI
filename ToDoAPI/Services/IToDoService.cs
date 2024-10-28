using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Models;

namespace ToDoAPI.Services
{
    public interface IToDoService
    {
        // Retrieve all tasks
        Task<IEnumerable<ToDoItem>> GetAllTodosAsync();

        // Retrieve a task by its Id
        Task<ToDoItem?> GetTodoByIdAsync(int id);

        // Retrieve tasks that fall within the specified period from 'fromDate' to 'toDate'
        Task<IEnumerable<ToDoItem>> GetIncomingTodosAsync(string filter);

        // Create a new task
        Task<ToDoItem> CreateTodoAsync(ToDoItem newTodo);

        // Update an existing task
        Task<ToDoItem?> UpdateTodoAsync(ToDoItem updatedTodo);

        // Set the completion percentage of a task
        Task SetTodoPercentCompleteAsync(int id, double percentComplete);

        // Delete a task by its Id
        Task DeleteTodoByIdAsync(int id);

        // Mark a task as completed
        Task MarkTodoAsDoneAsync(int id);
    }
}
