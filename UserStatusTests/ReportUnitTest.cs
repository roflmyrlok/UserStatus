using UserStatusLibrary;

namespace UserStatusTests;


public class ReportUnitTest
{
	[Fact]
	public void testOnline()
	{
		// Arrange
		var path = "/Users/atrybushnyi/workspace/sdb/UserStatus/UserStatusTests/ReportTestData.json";
		string filePath1 = "/users/atrybushnyi/workspace/sdb/UserStatus/UserStatusTests/TestJsonUserDictionary.json";
		string filePath2 = "/users/atrybushnyi/workspace/sdb/UserStatus/UserStatusTests/TestJsonGlobalStats.json";
		HistDataCore ss = new HistDataCore(filePath1, filePath2);

		// Act
		string[] jsonLines = File.ReadAllLines(path);
		string jsonString = string.Join("", jsonLines);
		var result = ss.Report("testReport", jsonString);
		var i = ss.ReturnReport("testReport");

		// Assert
		Assert.Equal("{}", result);
		Assert.True(i != null);
	}
}