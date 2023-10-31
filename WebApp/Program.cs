

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

string filePath1 = "jsonUserDictionary.json";
string filePath2 = "jsonGlobalStats.json";
var programInstance = new UserStatusLibrary.HistDataCore(filePath1,filePath2);
Task.Run(programInstance.UpdateUsersDictionary);
Task.Run(programInstance.UpdateSave);
app.UseRouting();

app.MapGet("/api/users", Stats);
app.MapGet("/api/total", Total);
app.MapGet("/api/average", Average);
app.MapGet("/api/predictions", Predictions);
app.MapPost("/api/forget", Forget);
app.MapPost("/api/report/{REPORT_NAME}", ReportHandlerPost);
app.MapGet("/api/report/{REPORT_NAME}", ReportHandlerGet);
app.MapGet("/", Base);


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

object? Base()
{
    return new
    {
        usersOnline = "/api/users?date=x",
        wasUserOnline = "/api/users?date=x&userid=y",
        userTotalOnlineTime = "/api/total?userid=x",
        userAvgTime = "/api/average?userid=x",
        predictUserOnline = "/api/predictions?date=x&userId=y&tolerance=z",
        predictTotalUsersOnline = "/api/predictions?date=x",
        forgetUser = "/api/forget?userid=x"
    };
}
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

object? ReportHandlerPost(string? reportName, string? reportData)
{
    if (reportData == null && reportName == null)
    {
        var b = "bad";
        return new {b};
    }
    var result = programInstance.Report(reportName, reportData);
    return new {result};
}

object? ReportHandlerGet(string? reportName)
{
    var result = programInstance.ReturnReport(reportName);
    return result;
}


