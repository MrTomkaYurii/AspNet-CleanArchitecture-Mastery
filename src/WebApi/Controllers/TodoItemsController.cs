using Application.Common.Models;
using Application.TodoItems.Commands.CreateTodoItem;
using Application.TodoItems.Commands.DeleteTodoItem;
using Application.TodoItems.Commands.UpdateTodoItem;
using Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Контролер для роботи з окремими елементами завдань (TodoItems).
/// </summary>
public class TodoItemsController : ApiControllerBase
{
    /// <summary>
    /// Отримати завдання конкретного списку з розбиттям на сторінки (пагінація).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TodoItemBriefDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination(
        [FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Створити нове завдання у списку.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateTodoItemCommand command)
    {
        var id = await Mediator.Send(command);
        return Created(string.Empty, id);
    }

    /// <summary>
    /// Оновити завдання (назву, опис, пріоритет чи позначити виконаним).
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id в URL не збігається з Id в тілі запиту.");
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Видалити завдання за Id.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoItemCommand(id));
        return NoContent();
    }
}
