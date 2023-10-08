using UserStatusLibrary;
namespace UserStatusTests;

public class UnitTestHistoricalData
{
	[Fact]
	public static async Task UnitTestHistData()
	{
		//arrange
		
		// Act
		var f = await UserHistoricalDataStorage.Main();

		// Assert
		Assert.Equal(1, f);
	}
}