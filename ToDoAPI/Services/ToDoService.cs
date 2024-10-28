using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.Models;

namespace ToDoAPI.Services
{
    public class ToDoService : IToDoService
    {
        private readonly ToDoDbContext _context;

        public ToDoService(ToDoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ToDoItem>> GetAllTodosAsync()
        {
            return await _context.ToDoItems.AsNoTracking().ToListAsync();
        }

        public async Task<ToDoItem?> GetTodoByIdAsync(int id)
        {
            return await _context.ToDoItems.AsNoTracking().FirstOrDefaultAsync(todo => todo.Id == id);
        }


        public async Task<IEnumerable<ToDoItem>> GetIncomingTodosAsync(string filter)
        {
            var today = DateTime.Today;


            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

            IQueryable<ToDoItem> query = _context.ToDoItems.Where(todo => !todo.IsDone);

            if (filter == "today")
            {
                query = query.Where(todo => todo.ExpiryDate.Date == today);
            }
            else if (filter == "tomorrow")
            {
                query = query.Where(todo => todo.ExpiryDate.Date == today.AddDays(1));
            }
            else if (filter == "week")
            {
                query = query.Where(todo => todo.ExpiryDate >= startOfWeek && todo.ExpiryDate <= endOfWeek);
            }
            else
            {
                throw new ArgumentException("Invalid filter. Available options are: 'today', 'tomorrow', 'week'.");
            }

            return await query.ToListAsync();
        }

        public async Task<ToDoItem> CreateTodoAsync(ToDoItem newTodo)
        {
            _context.ToDoItems.Add(newTodo);
            await _context.SaveChangesAsync();
            return newTodo;
        }

        public async Task<ToDoItem?> UpdateTodoAsync(ToDoItem updatedTodo)
        {
            var existingTodo = await _context.ToDoItems.FindAsync(updatedTodo.Id);
            if (existingTodo == null) return null;

            existingTodo.Title = updatedTodo.Title;
            existingTodo.Description = updatedTodo.Description;
            existingTodo.ExpiryDate = updatedTodo.ExpiryDate;
            existingTodo.PercentComplete = updatedTodo.PercentComplete;

            if (updatedTodo.PercentComplete == 100)
            {
                existingTodo.IsDone = true;
            }

            await _context.SaveChangesAsync();
            return existingTodo;
        }

        public async Task SetTodoPercentCompleteAsync(int id, double percentComplete)
        {
            var todo = await _context.ToDoItems.FindAsync(id);
            if (todo != null)
            {
                todo.PercentComplete = percentComplete;
                if (percentComplete == 100)
                {
                    todo.IsDone = true;
                }

                await _context.SaveChangesAsync();

            }
        }

        public async Task DeleteTodoByIdAsync(int id)
        {
            var todo = await _context.ToDoItems.FindAsync(id);
            if (todo != null)
            {
                _context.ToDoItems.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkTodoAsDoneAsync(int id)
        {
            var todo = await _context.ToDoItems.FindAsync(id);
            if (todo != null)
            {
                todo.IsDone = true;
                todo.PercentComplete = 100;
                await _context.SaveChangesAsync();
            }
        }
    }
}
