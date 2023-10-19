using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace UserStatusLibrary;

public class UserStatusStorage
{
	public string format = "dd.MM.yyyy HH:mm";
	public ResponseObject users;
	public UserStatusStorage()
	{
		users = new ResponseObject();
	}

	public (string, string) SetMessage(bool isOnline, string nickname, DateTime currentTime,
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

	public string LocalisationUkr(string engMessage)
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

	public string ApiCall1(string url)
	{
		using var client = new HttpClient();
		using var result = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
		using var reader = new StreamReader(result.Content.ReadAsStream());
		var stringContent = reader.ReadToEnd();
		return stringContent;
	}

	public ResponseObject ParseData(string json)
	{
		var tempResponseObject = JsonConvert.DeserializeObject<ResponseObject>(json);
		if (users == null || tempResponseObject == null)
		{
			throw new Exception("users is not initialized");
		}

		users.data ??= new List<UserData>(); // Initialize users.data if it's null
		tempResponseObject.data ??= new List<UserData>();
		users.data.AddRange(tempResponseObject.data);
		return tempResponseObject;
	}

	public Dictionary<string, string> FillUserStatusDictionary(string link)
	{
		int offset = 0;
		var responseDictionary = new Dictionary<string, string>();
		var tempTry =  ApiCall1(link + offset.ToString());
		var tempResponseObject = ParseData(tempTry);
		while (tempResponseObject.data.Count != 0)
		{
			foreach (var user in tempResponseObject.data)
			{
				var lastSeen = DateTime.Now;
				var currentTime = DateTime.Now;
				if (user.lastSeenDate != null)
				{
					lastSeen = DateTime.ParseExact(user.lastSeenDate,format,CultureInfo.InvariantCulture);
				}
				var message = SetMessage(Convert.ToBoolean(user.isOnline), user.nickname, currentTime, lastSeen);
				responseDictionary.Add(message.Item1, message.Item2);
			}

			offset += tempResponseObject.data.Count;
			tempTry = ApiCall1(link + offset.ToString());
			tempResponseObject = ParseData(tempTry);
		}
		return responseDictionary;
	}

	public Dictionary<string, UserData> ObserveUsers(string link, Dictionary<string, UserData> dictionaryReference, List<string> forbiddenUsers)
	{
		var temp = dictionaryReference;
		int offset = 0;
		var currTime = DateTime.Now;
		var tempTry = ApiCall1(link + offset.ToString());
		var tempResponseObject = ParseData(tempTry);
		while (tempResponseObject.data.Count != 0)
		{
			foreach (var user in tempResponseObject.data)
			{
				if (forbiddenUsers != null)
				{
					if (forbiddenUsers.Contains(user.userId))
					{
						continue;
					}
				}
				
				if (!temp.ContainsKey(user.userId))
				{
					temp.Add(user.userId, user);
				
					if (user.isOnline == "true")
					{
						user.onlineStart = new List<TimeSegment>();
						user.onlineStart.Add(new TimeSegment(){end = null, start = currTime});
					}
					else
					{
						user.onlineStart = new List<TimeSegment>();
						user.onlineStart.Add(new TimeSegment(){end = currTime, start = DateTime.Parse(user.lastSeenDate)});
					}
				}
				var tempUser = temp[user.userId];
				tempUser.isOnline = user.isOnline;
				tempUser.lastSeenDate = user.lastSeenDate;
				
				
				var lastSegment = tempUser.onlineStart[^1];
				if (tempUser.isOnline == "true")
				{
					if (lastSegment.end == null)
					{
						continue;
					}

					tempUser.onlineStart.Add(new TimeSegment(){start = currTime, end = null});
				}
				else
				{
					lastSegment.end ??= currTime;
				}
			}

			offset += tempResponseObject.data.Count;
			tempTry = ApiCall1(link + offset.ToString());
			tempResponseObject = ParseData(tempTry);
		}
		return temp;
	}
}