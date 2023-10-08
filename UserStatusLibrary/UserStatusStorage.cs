using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace UserStatusLibrary;

public class UserStatusStorage
{
	public static ResponseObject Users;

	static UserStatusStorage()
	{
		Users = new ResponseObject();
        
	}

	public static (string, string) SetMessage(bool isOnline, string nickname, DateTime currentTime,
		DateTime lastSeenTime)
	{
		var year2000 = new DateTime(2000, 1, 1);
		var timeDifferenceSpan = (currentTime - year2000) - (lastSeenTime - year2000);
		// Get the total number of seconds
		var secondsPassed = (long) timeDifferenceSpan.TotalSeconds;
		if (isOnline)
		{
			return (nickname, "is online now");
		}

		switch (secondsPassed)
		{
			case < 30:
				return (nickname, " was online just now");
			case < 60:
				return (nickname, " was online 1 minute ago");
			case < 3540:
				return (nickname, " was online a couple minutes ago");
			case < 7140:
				return (nickname, " was online 1 hour ago");
			case < 86400 * 2:
				if (lastSeenTime.Date == currentTime.Date)
				{
					return (nickname, " was online today");
				}
				else
				{
					return (nickname, " was online yesterday");
				}
			case < 604800:
				return (nickname, " was online this week");
			default:
				return (nickname, " was online a long time ago");
		}
	}

	public static string LocalisationUkr(string engMessage)
	{
		switch (engMessage)
		{
			case " was online just now":
				return " був(ла) в мережі щойно";
			case " was online 1 minute ago":
				return " був(ла) в мережі 1 хвилину тому";
			case " was online a couple minutes ago":
				return " був(ла) в мережі кілька хвилин тому";
			case " was online 1 hour ago":
				return " був(ла) в мережі 1 годину тому";
			case " was online today":
				return " був(ла) в мережі сьогодні";
			case " was online yesterday":
				return " був(ла) в мережі вчора";
			case " was online this week":
				return " був(ла) в мережі цього тижня";
			default:
				return " був(ла) в мережі давно";
		}
	}

	public static async Task<(string,int)> ApiCall1(string link, string offset)
	{
		using HttpClient client = new HttpClient();
		try
		{
			var url = link + offset;
			var response = await client.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				var json = await response.Content.ReadAsStringAsync();
				return (json,1);
			}
			else
			{
				return ($"Request failed with status code {response.StatusCode}",0);
			}
		}
		catch(Exception e)
		{
			return (e.Message,0);
		}
	}

	public static ResponseObject ParseData(string json)
	{
		var tempResponseObject = JsonConvert.DeserializeObject<ResponseObject>(json);
		if (Users == null || tempResponseObject == null)
		{
			throw new Exception("Users is not initialized");
		}

		Users.data ??= new List<UserData>(); // Initialize Users.data if it's null
		tempResponseObject.data ??= new List<UserData>();
		Users.data.AddRange(tempResponseObject.data);
		return tempResponseObject;
	}

	public static async Task<Dictionary<string, string>> FillUserStatusDictionary(string link)
	{
		int offset = 0;
		var responseDictionary = new Dictionary<string, string>();
		var tempTry = await ApiCall1(link,offset.ToString());
		if (tempTry.Item2 == 0) { return null;}
		var tempResponseObject = ParseData(tempTry.Item1);
		while (tempResponseObject.data.Count != 0)
		{
			foreach (var user in tempResponseObject.data)
			{
				var lastSeen = DateTime.Now;
				var currentTime = DateTime.Now;
				if (user.lastSeenDate != null)
				{
					lastSeen = DateTime.Parse(user.lastSeenDate);
				}
				var message = SetMessage(Convert.ToBoolean(user.isOnline), user.nickname, currentTime, lastSeen);
				responseDictionary.Add(message.Item1, message.Item2);
			}

			offset += tempResponseObject.data.Count;
			tempTry = await ApiCall1(link, offset.ToString());
			tempResponseObject = ParseData(tempTry.Item1);
		}
		return responseDictionary;
	}
}