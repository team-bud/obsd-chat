using ServerCore;


// builder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<ChatServer>();


// app
var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();


// start
app.Run();
