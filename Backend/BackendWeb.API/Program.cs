// ---------
// Libraries
// ---------


// ---------
// Builder
// ---------
var builder = WebApplication.CreateBuilder(args);

// ---------
// Dependency Injections
// ---------


// ---------
// App
// ---------
var app = builder.Build();

// ---------
// APIs
// ---------
app.MapGet("/api", () => "Hello World!");

// ---------
// Execution 
// ---------
app.Run();
