using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Models;
using ToDoAPI.Services;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _todoService;

        public ToDoController(IToDoService todoService)
        {
            _todoService = todoService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncomingTodos([FromQuery] string filter)
        {
            var todos = await _todoService.GetIncomingTodosAsync(filter);
            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] ToDoItem newTodo)
        {
            var createdTodo = await _todoService.CreateTodoAsync(newTodo);
            return CreatedAtAction(nameof(GetTodoById), new { id = createdTodo.Id }, createdTodo);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] ToDoItem updatedTodo)
        {
            updatedTodo.Id = id;

            var todo = await _todoService.UpdateTodoAsync(updatedTodo);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }


        [HttpPatch("{id}/percent-complete")]
        public async Task<IActionResult> SetPercentComplete(int id, [FromBody] double percentComplete)
        {
            await _todoService.SetTodoPercentCompleteAsync(id, percentComplete);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoById(int id)
        {
            await _todoService.DeleteTodoByIdAsync(id);
            return NoContent();
        }


        [HttpPatch("{id}/mark-done")]
        public async Task<IActionResult> MarkTodoAsDone(int id)
        {
            await _todoService.MarkTodoAsDoneAsync(id);
            return NoContent();
        }
    }
}