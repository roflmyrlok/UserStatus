using UserStatusLibrary;

namespace UserStatusTests;

public class UnitTestHistData
{
	

	[Theory]
	[InlineData("13.10.2023 20:33", 53)]
	[InlineData("13.10.2023 20:34", 58)]
	[InlineData("13.10.2023 21:01", 59)]
	[InlineData("13.10.2023", null)]
	public void TestGetUsersOnlineByData(string date, int? expected)
	{
		string filePath1 = "jsonUserDictionary.json";
		string filePath2 = "jsonGlobalStats.json";
	    HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.GetUsersOnlineByData(date);


		if (expected != null) Assert.Equal(result, expected.Value);
		
	}
	[Theory]
	[InlineData("13.10.2023 21:01","0,85", "2fba2529-c166-8574-2da2-eac544d82634","willBeOnline: true, onlineChance: 1")]
	[InlineData("13.10.2023 21:01","1,1", "2fba2529-c166-8574-2da2-eac544d82634","willBeOnline: true, onlineChance: 1")]
	[InlineData("13.10.2023 21:01","0,85", "8574-2da2-eac544d82634",null)]
	[InlineData("13.10.2022 21:01","0,85", "2fba2529-c166-8574-2da2-eac544d82634",null)]
	public void TestPredictOnlineForUser(string date, string tolerance, string id, string? expected)
	{
		string filePath1 = "jsonUserDictionary.json";
		string filePath2 = "jsonGlobalStats.json";
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.predictOnlineForUser(date,tolerance,id);
		
		if (expected != null) Assert.Equal(result, expected);
	}

	[Theory]
	[InlineData("13.10.2023 21:01", "51")]
	public void TestPredictOnlineUsers(string date, string expected )
	{
		string filePath1 = "jsonUserDictionary.json";
		string filePath2 = "jsonGlobalStats.json";
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.predictOnlineUsers(date);
		
		if (expected != null) Assert.Equal(result, expected);
	}

	[Theory]
	[InlineData("13.10.2023 21:01", "2fba2529-c166-8574-2da2-eac544d82634", null)]
	[InlineData("13.10.1999 21:01", "2fba2529-c166-8574-eac544d82634", null)]
	[InlineData("13.10.2023 21:01", "2fba2529-c166-8574-2da2-eac544d82634", "user was online since 13.10.2023 21:01")]
	public void TestWasUserOnline(string date, string id, string? expected)
	{
		string filePath1 = "jsonUserDictionary.json";
		string filePath2 = "jsonGlobalStats.json";
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.WasUserOnline(date,id);
		
		if (expected != null) Assert.Equal(result, expected);
		
	}
	
}