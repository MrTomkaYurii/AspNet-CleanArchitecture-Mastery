using Application.TodoLists.Commands.CreateTodoList;
using Application.TodoLists.Commands.DeleteTodoList;
using Application.TodoLists.Commands.UpdateTodoList;
using Application.TodoLists.Queries.GetTodoLists;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Контролер для керування списками завдань (TodoLists).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (CQRS через REST API):
/// Кожен метод відповідає за один конкретний Use Case:
/// - GET -> GetTodoListsQuery (читання)
/// - POST -> CreateTodoListCommand (створення)
/// - PUT -> UpdateTodoListCommand (модифікація)
/// - DELETE -> DeleteTodoListCommand (видалення)
/// </summary>
public class TodoListsController : ApiControllerBase
{
    /// <summary>
    /// Отримати всі списки завдань разом з їхніми елементами.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TodoListsVm), StatusCodes.Status200OK)]
    public async Task<ActionResult<TodoListsVm>> Get()
    {
        return await Mediator.Send(new GetTodoListsQuery());
    }

    /// <summary>
    /// Створити новий список завдань.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateTodoListCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    /// <summary>
    /// Оновити існуючий список завдань.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id в URL не збігається з Id в тілі запиту.");
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Видалити список завдань за його ідентифікатором.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoListCommand(id));
        return NoContent();
    }
}
