

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

string filePath1 = "jsonUserDictionary.json";
string filePath2 = "jsonGlobalStats.json";
var programInstance = new UserStatusLibrary.HistDataCore(filePath1,filePath2);
Task.Run(programInstance.UpdateUsersDictionary);
Task.Run(programInstance.UpdateSave);
app.UseRouting();

app.MapGet("/api/stats/users", Stats);
app.MapGet("/api/predictions/users", Predictions);
app.MapPost("/api/user/forget", Forget);
app.MapGet("/api/stats/user/total", Total);
app.MapGet("/api/stats/user/average", Average);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


object? Stats(string? date, string? userId)
{
    if (userId != null && date != null)
    {
        var result = programInstance.WasUserOnline(date, userId);
        if (result != null) { return new {wasUserOnline = result.Value.Item1, nearestOnlineTime = result.Value.Item2};}
        
    }
    else if (date != null)
    {
        var result = programInstance.GetUsersOnlineByData(date);
        if (result != null) return new {usersOnline = result.Value};
    }

    return null;
}

object? Predictions(string? date, string? tolerance, string? userId)
{
    if (date == null)
    {
        return null;
    }

    if (tolerance != null && userId != null)
    {
        var result = programInstance!.predictOnlineForUser(date, tolerance, userId);
        return new {willBeOnline = result!.Value.Item1, onlineChance = result.Value.Item2};
    }
    else
    {
        var result = programInstance!.predictOnlineUsers(date);
        return new {onlineUsers = result};
    }
    return null;
}

object? Forget(string? userId)
{
    if (userId == null) { return null;}
    return new {userId = programInstance.RightToBeForgotten(userId)};
}

object? Total(string? userId)
{
    if (userId == null) { return null;}
    return new {totalTime = programInstance.TotalTime(userId)};
}

object? Average(string? userId)
{
    if (userId == null) { return null;}

    var result = programInstance.DailyWeeklyAverage(userId);
    return new {weeklyAverage = result.Value.Item1, dailyAverage = result.Value.Item2};
}


