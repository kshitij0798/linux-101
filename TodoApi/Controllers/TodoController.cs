using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _context;
        private readonly ILogger<TodoController> _logger;

        public TodoController(TodoDbContext context, ILogger<TodoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
        {
            try
            {
                return await _context.Todos.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all todos");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetById(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning("Todo with ID {Id} not found", id);
                    return NotFound($"Todo with ID {id} not found");
                }

                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting todo {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> Create(Todo todo)
        {
            try
            {
                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new todo with ID {Id}", todo.Id);
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating todo");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                _context.Entry(todo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated todo with ID {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Todos.AnyAsync(t => t.Id == id))
                {
                    _logger.LogWarning("Todo with ID {Id} not found during update", id);
                    return NotFound($"Todo with ID {id} not found");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating todo {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning("Todo with ID {Id} not found for deletion", id);
                    return NotFound($"Todo with ID {id} not found");
                }

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted todo with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting todo {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Todo>>> Search([FromQuery] string? term, [FromQuery] Priority? priority)
        {
            try
            {
                var query = _context.Todos.AsQueryable();

                if (!string.IsNullOrWhiteSpace(term))
                {
                    query = query.Where(t => t.Title.Contains(term) || t.Description!.Contains(term));
                }

                if (priority.HasValue)
                {
                    query = query.Where(t => t.Priority == priority.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching todos");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
} 