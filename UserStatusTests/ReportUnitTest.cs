using Microsoft.Extensions.Configuration;
using UserStatusLibrary;
using System.IO;

namespace UserStatusTests;


public class ReportUnitTest
{
	[Fact]
	public void testReport()
	{
		// Arrange
		var tmp =  Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName;
		var path = tmp + "/ReportTestData.json";
		var filePath1 = tmp +"/TestJsonUserDictionary.json";
		var filePath2 = tmp +"/TestJsonGlobalStats.json";
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