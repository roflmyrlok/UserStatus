using UserStatusLibrary;
using Xunit;

namespace UserStatusTests;

public class UnitTestHistData
{
	static string tmp =  Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName;
	string filePath1 = tmp +"/TestJsonUserDictionary.json";
	string filePath2 = tmp +"/TestJsonGlobalStats.json";
	
	

	[Theory]
	[InlineData("13.10.2023-20:33", 53)]
	[InlineData("13.10.2023-20:34", 58)]
	[InlineData("13.10.2023-21:01", 59)]
	public void TestGetUsersOnlineByData(string date, int? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.GetUsersOnlineByData(date);


		if (expected != null) Assert.Equal(expected.Value,result.Value);
		
	}
	
	[Theory]
	[InlineData("13.10.2023-21:01","0,85", "2fba2529-c166-8574-2da2-eac544d82634",("false" + "0"))]
	[InlineData("13.10.2023-21:01","1,1", "2fba2529-c166-8574-2da2-eac544d82634", ("false" + "0"))]
	[InlineData("13.10.2023-21:01","0,85", "8574-2da2-eac544d82634",null)]
	public void TestPredictOnlineForUser(string date, string tolerance, string id, string? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.predictOnlineForUser(date,tolerance,id);
		if (expected == null)
		{
			Assert.True(!result.HasValue);
		} else {
			Assert.Equal(expected, result.Value.Item1+result.Value.Item2);
		}
	}

	[Theory]
	[InlineData("13.10.2023-21:01", "54")]
	public void TestPredictOnlineUsers(string date, string expected )
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.predictOnlineUsers(date);
		
		if (expected != null) Assert.Equal(result, expected);
	}

	[Theory]
	[InlineData("13.10.2023-21:01", "2fba2529-c166-8574-2da2-eac544d82634", null)]
	[InlineData("13.10.1999-21:01", "2fba2529-c166-8574-eac544d82634", null)]
	[InlineData("13.10.2023-21:01", "2fba2529-c166-8574-2da2-eac544d82634", "true")]
	public void TestWasUserOnline(string date, string id, string? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.WasUserOnline(date,id);
		
		if (expected != null) Assert.Equal(result.Value.Item1, expected);
		
	}
	
	[Theory]
	[InlineData("2fba2529-c166-8574-2da2-eac544d82634", "2fba2529-c166-8574-2da2-eac544d82634")]
	[InlineData("2fba2529-c166-8574-eac544d82634", null)]
	public void TestRightToBeForgotten(string id, string? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.RightToBeForgotten(id, false);
		
		if (expected != null) Assert.Equal(expected, result);
		
	}
	
	[Theory]
	[InlineData("2fba2529-c166-8574-2da2-eac544d82634", "sting")]
	[InlineData("2fba2529-c166-8574-eac544d82634", null)]
	public void TestDailyWeeklyAverage(string id, string? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.DailyWeeklyAverage(id);
		
		if (expected != null) Assert.True(result.HasValue);
		
	}
	
	[Theory]
	[InlineData("2fba2529-c166-8574-2da2-eac544d82634", "System.Int32")]
	[InlineData("2fba2529-c166-8574-eac544d82634", null)]
	public void TestTotalTime(string id, string? expected)
	{
		HistDataCore core = new HistDataCore(filePath1,filePath2);
		
		var result = core.TotalTime(id);
		if (expected == null)
		{
			Assert.False(result.HasValue);
		}
		else
		{
			Assert.Equal(expected, result.Value.GetType().ToString());
		}
		 
	}
	
}