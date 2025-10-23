using Microsoft.AspNetCore.Mvc;
using demo_api_csharp.Models;

namespace demo_api_csharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        // In-memory storage (thay thế bằng database trong thực tế)
        private static List<Todo> _todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "Học ASP.NET Core", Description = "Tìm hiểu về Web API", IsCompleted = false, CreatedAt = DateTime.Now.AddDays(-2) },
            new Todo { Id = 2, Title = "Tạo Todo API", Description = "Xây dựng API quản lý công việc", IsCompleted = true, CreatedAt = DateTime.Now.AddDays(-1), CompletedAt = DateTime.Now.AddHours(-2) },
            new Todo { Id = 3, Title = "Viết Documentation", Description = "Tạo README và hướng dẫn sử dụng", IsCompleted = false, CreatedAt = DateTime.Now.AddHours(-1) }
        };
        private static int _nextId = 4;

        /// <summary>
        /// Lấy danh sách tất cả công việc
        /// </summary>
        /// <returns>Danh sách công việc</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Todo>> GetTodos()
        {
            return Ok(_todos);
        }

        /// <summary>
        /// Lấy thông tin một công việc theo ID
        /// </summary>
        /// <param name="id">ID của công việc</param>
        /// <returns>Thông tin công việc</returns>
        [HttpGet("{id}")]
        public ActionResult<Todo> GetTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Không tìm thấy công việc với ID {id}");
            }
            return Ok(todo);
        }

        /// <summary>
        /// Thêm công việc mới
        /// </summary>
        /// <param name="todo">Thông tin công việc mới</param>
        /// <returns>Công việc đã được tạo</returns>
        [HttpPost]
        public ActionResult<Todo> CreateTodo([FromBody] CreateTodoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Tiêu đề không được để trống");
            }

            var newTodo = new Todo
            {
                Id = _nextId++,
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            _todos.Add(newTodo);
            return CreatedAtAction(nameof(GetTodo), new { id = newTodo.Id }, newTodo);
        }

        /// <summary>
        /// Cập nhật trạng thái hoàn thành của công việc
        /// </summary>
        /// <param name="id">ID của công việc</param>
        /// <returns>Công việc đã được cập nhật</returns>
        [HttpPut("{id}")]
        public ActionResult<Todo> UpdateTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Không tìm thấy công việc với ID {id}");
            }

            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;

            return Ok(todo);
        }

        /// <summary>
        /// Xóa công việc
        /// </summary>
        /// <param name="id">ID của công việc cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Không tìm thấy công việc với ID {id}");
            }

            _todos.Remove(todo);
            return Ok(new { message = $"Đã xóa công việc '{todo.Title}' thành công" });
        }

        /// <summary>
        /// Lấy thống kê công việc
        /// </summary>
        /// <returns>Thống kê tổng quan</returns>
        [HttpGet("stats")]
        public ActionResult<object> GetStats()
        {
            var total = _todos.Count;
            var completed = _todos.Count(t => t.IsCompleted);
            var pending = total - completed;

            return Ok(new
            {
                Total = total,
                Completed = completed,
                Pending = pending,
                CompletionRate = total > 0 ? Math.Round((double)completed / total * 100, 2) : 0
            });
        }
    }

    // DTO cho request tạo mới
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
