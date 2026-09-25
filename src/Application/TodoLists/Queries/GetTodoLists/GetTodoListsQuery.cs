using MediatR;

namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// CQRS Query: Отримання всіх списків завдань разом з дочірніми елементами.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (CQRS - Query):
/// Відповідає за читання даних.
/// Запит повертає ViewModel (TodoListsVm) і гарантовано НЕ змінює стан бази даних.
/// </summary>
public record GetTodoListsQuery : IRequest<TodoListsVm>;
