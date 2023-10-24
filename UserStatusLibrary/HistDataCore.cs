using System.Globalization;
using Newtonsoft.Json;

namespace UserStatusLibrary;

using System;



public class HistDataCore
{
	private List<string> _forbiddenUsers;
	private bool _saveRdy = false;
	private Dictionary<string, UserData> _userDictionary;
	private Dictionary<string, int> _globalStats;
	private UserStatusStorage _userStatusStorage;
	private readonly string _format = "dd.MM.yyyy HH:mm";
	private readonly string _filePath1;
	private readonly string _filePath2;
	private readonly string _filePath3 = "forbidden.json";

	public HistDataCore(string filePath1, string filePath2)
	{
		_filePath1 = filePath1;
		_filePath2 = filePath2;
		this._forbiddenUsers = new List<string> { };
		this._userDictionary = new Dictionary<string, UserData>();
		this._globalStats = new Dictionary<string, int>();
		_userStatusStorage = new UserStatusStorage();
		if (!File.Exists(_filePath3))
		{
			File.Create(_filePath3);
		}

		if (File.Exists(filePath1) && File.Exists(filePath2))
		{
			string json1 = File.ReadAllText(_filePath1);
			string json2 = File.ReadAllText(_filePath2);
			string json3 = File.ReadAllText(_filePath3);

			_userDictionary = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(json1);
			_globalStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(json2);
			_forbiddenUsers = JsonConvert.DeserializeObject<List<string>>(json3);
		}
	}

	public int GetOnlineUsers()
	{
		var counter = 0;
		foreach (var user in _userDictionary.Values)
		{
			if (user.isOnline == "true")
			{
				counter++;
			}
		}

		var rn = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
		if (!_globalStats.ContainsKey(rn))
		{
			_globalStats.Add(DateTime.Now.ToString("dd.MM.yyyy HH:mm"), counter);
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
			_userDictionary = _userStatusStorage.ObserveUsers(
				"https://sef.podkolzin.consulting/api/users/lastSeen?offset=", _userDictionary, _forbiddenUsers);
			GetOnlineUsers();
			if (!_saveRdy && _userDictionary.Count != 0 && _globalStats.Count != 0)
			{
				SaveUserDictionary();
				_saveRdy = true;
			}
		}
	}

	public void SaveUserDictionary()
	{
		string jsonUserDictionary = JsonConvert.SerializeObject(_userDictionary, Formatting.Indented);
		string jsonGlobalStats = JsonConvert.SerializeObject(_globalStats, Formatting.Indented);
		string jsonForbidden = JsonConvert.SerializeObject(_forbiddenUsers, Formatting.Indented);

		if (File.Exists(_filePath1))
		{
			File.Delete(_filePath1);
		}

		if (File.Exists(_filePath2))
		{
			File.Delete(_filePath2);
		}

		// Write the JSON data to the file.
		File.WriteAllText(_filePath1, jsonUserDictionary);
		File.WriteAllText(_filePath2, jsonGlobalStats);
		File.WriteAllText(_filePath3, jsonForbidden);

	}

	public int? GetUsersOnlineByData(string date)
	{
		var notFormattedDate = DateTime.ParseExact(date,_format,CultureInfo.InvariantCulture);
		if (_globalStats.ContainsKey(notFormattedDate.ToString(_format)))
		{
			return _globalStats[notFormattedDate.ToString(_format)];
		}
		else return null;
	}

	public (string?, string?)? WasUserOnline(string date, string id)
	{
		var notFormattedDate = DateTime.ParseExact(date,_format,CultureInfo.InvariantCulture);
		var formattedDate = notFormattedDate.ToString(_format);
		if (!_userDictionary.ContainsKey(id) || !_globalStats.ContainsKey(formattedDate))
		{
			return null;
		}

		var user = _userDictionary[id];

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
				return ("true", null);
			}

			if (segment.end.HasValue && segment.end <= notFormattedDate)
			{
				return ("true", null);
			}
		}

		if (lastSeen.HasValue)
		{
			return ("false", lastSeen.Value.ToString(_format));
		}

		return null;

	}

	public string predictOnlineUsers(string date)
	{
		var obs = 0;
		var sum = 0;
		var notFormattedDate = DateTime.ParseExact(date,_format,CultureInfo.InvariantCulture);
		var day = notFormattedDate.DayOfWeek;
		var hour = notFormattedDate.Hour;
		foreach (var entry in _globalStats.Keys)
		{
			var notFormattedEntry = DateTime.ParseExact(entry,_format,CultureInfo.InvariantCulture);
			var dayE = notFormattedEntry.DayOfWeek;
			var hourE = notFormattedEntry.Hour;
			if (day == dayE && hourE == hour)
			{
				obs++;
				sum += _globalStats[entry];
			}
		}

		return (sum / obs).ToString();
	}

	public (string?, string?)? predictOnlineForUser(string date, string tolerance, string id)
	{
		if (!_userDictionary.ContainsKey(id))
		{
			return null;
		}

		var notFormattedDate = DateTime.ParseExact(date,_format,CultureInfo.InvariantCulture);
		var requestDate = notFormattedDate.DayOfWeek;

		if (!_userDictionary[id].onlineStart[0].start.HasValue)
		{
			return null;
		}

		var startDate = _userDictionary[id].onlineStart[0].start.Value.Date;
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
			foreach (var segment in _userDictionary[id].onlineStart)
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
			return ("true", chance.ToString());
		}
		else
		{
			return ("false", chance.ToString());
		}
	}

	public int? TotalTime(string id)
	{
		if (!_userDictionary.ContainsKey(id))
		{
			return null;
		}

		var user = _userDictionary[id];
		var seconds = 0;
		foreach (var segment in user.onlineStart)
		{
			if (!segment.start.HasValue)
			{
				continue;
			}

			if (!segment.end.HasValue)
			{
				seconds += (int) (DateTime.Now - segment.start.Value).TotalSeconds;
				continue;
			}

			seconds += (int) (segment.end.Value - segment.start.Value).TotalSeconds;
		}

		return seconds;
	}

	public (string, string)? DailyWeeklyAverage(string id)
	{
		if (!_userDictionary.ContainsKey(id))
		{
			return null;
		}

		var user = _userDictionary[id];
		var dateTime = user.onlineStart[0].start;
		if (dateTime == null)
		{
			return null;
		}

		var startingSegment = dateTime.Value;
		var total = (int) (DateTime.Now - startingSegment).TotalSeconds;
		var result = TotalTime(id);
		if (result == null)
		{
			return null;
		}

		decimal dailyPercentage = (decimal) result.Value / (decimal) total;
		Int128 daily = (Int128) (dailyPercentage * 60 * 60 * 24);
		return (daily.ToString(CultureInfo.CurrentCulture), (daily * 7).ToString(CultureInfo.CurrentCulture));
	}

	public string? RightToBeForgotten(string id, bool saveDb = true)
	{
		if (!_userDictionary.ContainsKey(id))
		{
			return null;
		}

		_userDictionary.Remove(id);
		_forbiddenUsers.Add(id);
		if (saveDb)
		{
			UpdateSave();
		}

		if (_userDictionary.ContainsKey(id))
		{
			return null;
		}

		return id;
	}

	public string Report(string reportName, string reportData)
	{
		var path = "" + reportName;
		var tmp = ReportParse(reportData);
		var result = new Dictionary<string, List<string>>();
		result.Add("metrics", tmp.Metrics);
		foreach (var user in tmp.Users )
		{
			if (!_userDictionary.ContainsKey(user))
			{
				continue;
			}
			result.Add(user, new List<string>());
			if (tmp.Metrics.Contains("dailyAverage"))
			{
				result[user].Add(DailyWeeklyAverage(user).Value.Item1);
			}
			if (tmp.Metrics.Contains("weeklyAverage"))
			{
				result[user].Add(DailyWeeklyAverage(user).Value.Item2);
			}
			if (tmp.Metrics.Contains("total"))
			{
				result[user].Add(TotalTime(user).Value.ToString());
			}
		}
		string resultSerializeObject = JsonConvert.SerializeObject(result, Formatting.Indented);

		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.WriteAllText(path, resultSerializeObject);
		return "{}";
	}

	class ReportData
	{
		public List<string> Metrics { get; set; }
		public List<string> Users { get; set; }
	}

	ReportData ReportParse(string json)
	{
		ReportData data = JsonConvert.DeserializeObject<ReportData>(json);
		return data;
	}

	public string? ReturnReport(string reportName)
	{
		if (!File.Exists(reportName))
		{
			return null;
		}
		string[] jsonLines = File.ReadAllLines(reportName);
		string jsonString = string.Join("", jsonLines);
		return jsonString;
	}
}
