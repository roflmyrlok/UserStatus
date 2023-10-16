using UserStatusLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

string filePath1 = "jsonUserDictionary.json";
string filePath2 = "jsonGlobalStats.json";
var programInstance = new UserStatusLibrary.HistDataCore(filePath1,filePath2);
Task.Run(programInstance.UpdateUsersDictionary);
Task.Run(programInstance.UpdateSave);
app.UseRouting();

app.MapGet("/api/stats/users/1/", (string date) => new { usersOnline = programInstance.GetUsersOnlineByData(date) });
app.MapGet("/api/stats/users/2/", (string date, string id) => new { wasUserOnline = programInstance.WasUserOnline(date, id)});
app.MapGet("/api/predictions/users/1/", (string date) => new {onlineUsers = programInstance.predictOnlineUsers(date)});
app.MapGet("/api/predictions/users/2/", (string date, string tolerance, string userId) => new {result = programInstance.predictOnlineForUser(date, tolerance, userId)});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

