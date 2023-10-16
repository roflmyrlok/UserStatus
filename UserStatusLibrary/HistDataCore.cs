using System.Globalization;
using Newtonsoft.Json;

namespace UserStatusLibrary;

using System;



public class HistDataCore
{
	private bool saveRdy = false;
	Dictionary<string,UserData> userDictionary;
	public Dictionary<string, int> globalStats;
	public UserStatusStorage UserStatusStorage;
	private string format = "dd.MM.yyyy HH:mm";

	public HistDataCore(string filePath1, string filePath2)
	{
		this.userDictionary = new Dictionary<string, UserData>();
		this.globalStats = new Dictionary<string, int>();
		UserStatusStorage = new UserStatusStorage();
		if (File.Exists(filePath1) && File.Exists(filePath2))
		{
			string json1 = File.ReadAllText(filePath1);
			string json2 = File.ReadAllText(filePath2);

			userDictionary = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(json1);
			globalStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(json2);
		}
	}

	public int GetOnlineUsers()
	{
		var counter = 0;
		foreach (var user in userDictionary.Values)
		{
			if (user.isOnline == "true")
			{
				counter++;
			}
		}

		var rn = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
		if (!globalStats.ContainsKey(rn))
		{
			globalStats.Add(DateTime.Now.ToString("dd.MM.yyyy HH:mm"),counter);
		}
		return counter;
	}

	public void UpdateSave()
	{
		while (true)
		{
			SaveUserDictionary();
			Thread.Sleep(1000);
		}
	}
	
	public void UpdateUsersDictionary()
	{
		while (true)
		{
			userDictionary =  UserStatusStorage.ObserveUsers("https://sef.podkolzin.consulting/api/users/lastSeen?offset=", userDictionary);
			GetOnlineUsers();
			if (!saveRdy && userDictionary.Count != 0 && globalStats.Count != 0)
			{
				SaveUserDictionary();
				saveRdy = true;
			}
		}
	}

	public void SaveUserDictionary()
	{ 
		string jsonUserDictionary = JsonConvert.SerializeObject(userDictionary, Formatting.Indented);
		string jsonGlobalStats = JsonConvert.SerializeObject(globalStats, Formatting.Indented);
			
		string filePath1 = "jsonUserDictionary.json";
		string filePath2 = "jsonGlobalStats.json";
		if (File.Exists(filePath1))
		{
			File.Delete(filePath1);
		}
		if (File.Exists(filePath2))
		{
			File.Delete(filePath2);
		}

		// Write the JSON data to the file.
		File.WriteAllText(filePath1, jsonUserDictionary);
		File.WriteAllText(filePath2, jsonGlobalStats);
			
	}

	public int? GetUsersOnlineByData(string date)
	{
		var notFormattedDate = DateTime.Parse(date);
		if (globalStats.ContainsKey(notFormattedDate.ToString(format)))
		{
			return globalStats[notFormattedDate.ToString(format)];
		}
		else return null;
	}
	
	public string? WasUserOnline(string date, string id)
	{
		var notFormattedDate = DateTime.Parse(date);
		var formattedDate = notFormattedDate.ToString(format);
		if (!userDictionary.ContainsKey(id) || !globalStats.ContainsKey(formattedDate))
		{
			return null;
		}
		var user = userDictionary[id];
		/*if (user.onlineStart.Count == 1)
		{
			if (user.onlineStart[0].end == null)
			{
				return null;
			}
		}*/

		DateTime? lastSeen = null;
		foreach (var segment in user.onlineStart)
		{
			if (segment.start >= notFormattedDate || segment.start == null)
			{
				break;
			}

			lastSeen = segment.end;

			if (segment.end == null)
			{
				return "user was online since " + formattedDate;
			}
			if (segment.end.HasValue && segment.end <= notFormattedDate)
			{
				return "user was online at " + formattedDate;
			}
		}

		if (lastSeen.HasValue)
		{
			return "user was offline, last seen: " + lastSeen.Value.ToString(format);
		}

		return null;

	}

	public string predictOnlineUsers(string data)
	{
		var obs = 0;
		var sum = 0;
		var notFormattedDate = DateTime.Parse(data);
		var day = notFormattedDate.DayOfWeek;
		var hour = notFormattedDate.Hour;
		foreach (var entry in globalStats.Keys)
		{
			var notFormattedEntry = DateTime.Parse(data);
			var dayE = notFormattedEntry.DayOfWeek;
			var hourE = notFormattedEntry.Hour;
			if (day == dayE && hourE == hour)
			{
				obs++;
				sum += globalStats[entry];
			}
		}
		return (sum/obs).ToString();
	}

	public string? predictOnlineForUser(string data, string tolerance, string id)
	{
		if (!userDictionary.ContainsKey(id))
		{
			return null;
		}
		var notFormattedDate = DateTime.Parse(data);
		var requestDate = notFormattedDate.DayOfWeek;
		
		if (!userDictionary[id].onlineStart[0].start.HasValue)
		{
			return null;
		}
		var startDate = userDictionary[id].onlineStart[0].start.Value.Date;
		while (startDate.DayOfWeek != requestDate)
		{
			startDate = startDate.AddDays(1);
		}
		var currentDate = DateTime.Today;
		var total = 0;
		var passed = 0;
		while (startDate < currentDate)
		{
			total++;
			foreach (var segment in userDictionary[id].onlineStart)
			{
				if (segment.start.Value.Date == startDate)
				{
					passed++;
					break;
				}
			}
			startDate = startDate.AddDays(7);
		}

		if (total == 0)
		{
			return null;
		}
		var chance = passed / total;
		if (chance >= Convert.ToDouble(tolerance))
		{
			return "willBeOnline: true, onlineChance:" + chance;
		}
		else
		{
			return "willBeOnline: false, onlineChance:" + chance;
		}
	}
}
