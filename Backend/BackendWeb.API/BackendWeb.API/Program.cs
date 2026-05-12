using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Between these two lines builder--app
builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());
// We could customize our app builder

// Where singleton speaks about life cycle of the dependency that we're resolving
// One service that'll exist during the lifetime of the app

var app = builder.Build();

// Middlewares
// They are run IN ORDER!!!

// ASP.NET MiddleWares -> Rewrite routes
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// Custom Middlewares -> Support Logs
// param:
/*  context > HTTPCONTEXT
    type current http request/response that's been processed

    next > request delegate 
    I run my function now you run. Its literally the next 
    middleware that should be run in the pipeline. 
*/
app.Use(async (context, next) =>
{
    Console.WriteLine($" [{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started,");
    await next(context);
    Console.WriteLine($" [{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished,");
});

// Task Manager EXAMPLE

//GET
var todos = new List<Todo>();
app.MapGet("/todos/", (ITaskService service) => service.GetTodos());
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskService service) =>
{
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
    ? TypedResults.NotFound()
    : TypedResults.Ok(targetTodo);
});

//POST
app.MapPost("/todos", (Todo task, ITaskService service) =>
{
    service.AddTodo(task);
    return TypedResults.Created("/todos/{id}", task);
})
.AddEndpointFilter(async (context, next) => // ENDPOINT FILTER
{
    
    var taskArgument = context.GetArgument<Todo>(0); // Obtiene el primer argumento y lo convierte
    var errors = new Dictionary<string, string[]>(); // Diccionario de Errores por campo del
                                                     // objeto todo.
    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Cannot have due date in the past."]);
    }

    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed todo."]);
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    return await next(context);
});

//DELETE
app.MapDelete("/todos/{id}", (int id, ITaskService service) =>
{
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

//Execution
app.Run();

//Record
public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);

interface ITaskService
{
    Todo? GetTodoById(int id);
    List<Todo> GetTodos();
    void DeleteTodoById(int id);
    Todo AddTodo(Todo task);
}

class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];
    
    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }

    public void DeleteTodoById(int id)
    {
        _todos.RemoveAll(task => id == task. Id);
    }

    public Todo? GetTodoById(int id)
    {
        return _todos.SingleOrDefault(task => id == task.Id);
    }

    public List<Todo> GetTodos()
    {
        return _todos;
    }
} 