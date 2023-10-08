using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
// api/stats/users?date=08-10-2023-22:16:36&userId=2fba2529-c166-8574-2da2-eac544d82634
// api/stats/users?date=08-10-2023-22:16:36
app.MapGet("/api/stats/users", async context =>
{
	var dateParam = context.Request.Query["date"];
	var userId = context.Request.Query["userId"];
	var filePath = "/Users/atrybushnyi/workspace/tempproj/UserStatus/UserStatusTests/bin/Debug/net7.0/user_status_log.txt";

	if (!string.IsNullOrEmpty(dateParam) && !string.IsNullOrEmpty(userId))
	{

		if (!File.Exists(filePath))
		{
			context.Response.StatusCode = 404; 
			await context.Response.WriteAsync("File not found.");
			return;
		}

		try
		{
			string dateArg = dateParam.First().ToString();
			string format = "dd-MM-yyyy-HH:mm:ss";

			DateTime.TryParseExact(dateArg, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate);

			var (wasUserOnline, nearestOnlineTime) = GetUserOnlineStatusAndNearestTime(filePath, userId, targetDate);
            
			await context.Response.WriteAsJsonAsync(new
			{
				wasUserOnline,
				nearestOnlineTime
			});
		}
		catch (Exception ex)
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync($"An error occurred: {ex.Message}");
		}
	}
	else if (!string.IsNullOrEmpty(dateParam))
	{

		if (!File.Exists(filePath))
		{
			context.Response.StatusCode = 404;
			await context.Response.WriteAsync("File not found.");
			return;
		}

		try
		{
			string dateArg = dateParam.First().ToString();
			string format = "dd-MM-yyyy-HH:mm:ss";

			DateTime.TryParseExact(dateArg, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate);

            
			var usersOnline = CountUsersWithIsOnlineAndLastSeenDate(filePath, targetDate);

            
			await context.Response.WriteAsJsonAsync(new
			{
				usersOnline
			});
		}
		catch (Exception ex)
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync($"An error occurred: {ex.Message}");
		}
	}
	else
	{
		context.Response.StatusCode = 400;
		await context.Response.WriteAsync("Either 'date' or both 'date' and 'userId' query parameters are required.");
	}
});

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();

static (bool, DateTime?) GetUserOnlineStatusAndNearestTime(string filePath, string userId, DateTime targetDate)
{
	string[] lines = File.ReadAllLines(filePath);

	bool wasUserOnline = false;
	DateTime? nearestOnlineTime = null;
	DateTime? lastOnlineTime = null;

	foreach (string line in lines)
	{
		string[] parts = line.Split(',');

		if (parts.Length == 4 && parts[0] == userId)
		{
			DateTime recordDate = DateTime.Parse(parts[1]);
			bool isOnline = bool.Parse(parts[2]);

			if (recordDate <= targetDate)
			{
				if (isOnline)
				{
					wasUserOnline = true;
					lastOnlineTime = recordDate;
				}
				else if (!wasUserOnline)
				{
					nearestOnlineTime = recordDate;
				}
			}
		}
	}

	return (wasUserOnline, nearestOnlineTime);
}

static int CountUsersWithIsOnlineAndLastSeenDate(string filePath, DateTime targetDate)
{
	string[] lines = File.ReadAllLines(filePath);

	int count = 0;

	foreach (string line in lines)
	{
		string[] parts = line.Split(',');

		if (parts.Length == 4)
		{
			DateTime recordDate = DateTime.Parse(parts[1]);
			bool isOnline = bool.Parse(parts[2]);

			if (recordDate <= targetDate)
			{
				if (isOnline)
				{
					count++;
				}
			}
		}
	}

	return count;
}