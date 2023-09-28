using System.Net;
using UserStatusLibrary;
using Moq;
using Moq.Protected;

namespace UserStatusTests;


public class UnitTestUserStatusSolution
{
	[Fact]
	public void testOnline()
	{
		// Arrange
		bool isOnline = true;
		string nickname = "John";
		DateTime currentTime = DateTime.Now;
		DateTime lastSeenTime = currentTime.AddSeconds(-10); // Online just now

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal(("is online now"), message.Item2);
	}
	[Fact]
	public void testOnlineJustNow()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "John";
		DateTime currentTime = DateTime.Now;
		DateTime lastSeenTime = currentTime.AddSeconds(-15); // Online just now

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((" was online just now"), message.Item2);
	}
	
	[Fact]
	public void testWasOnline1MinuteAgo()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "John";
		DateTime currentTime = DateTime.Now;
		DateTime lastSeenTime = currentTime.AddSeconds(-45); // " was online 1 minute ago"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((" was online 1 minute ago"), message.Item2);
	}
	
	[Fact]
	public void testWasOnlineACoupleMinutesAgo()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "John";
		DateTime currentTime = DateTime.Now;
		DateTime lastSeenTime = currentTime.AddSeconds(-1200); // online a couple minutes ago

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((" was online a couple minutes ago"), message.Item2);
	}

	[Fact]
	public void testWasOnline1HourAgo()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "Alice";
		DateTime currentTime = new DateTime(2019,05,09,9,15,0);
		DateTime lastSeenTime = currentTime.AddSeconds(-5000); // " was online 1 hour ago"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((nickname, " was online 1 hour ago"), message);
	}
	
	[Fact]
	public void testWasOnlineToday()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "Alice";
		DateTime currentTime = new DateTime(2019,05,09,9,15,0);
		DateTime lastSeenTime = currentTime.AddHours(-6); // " was online today"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((nickname, " was online today"), message);
	}
	
	[Fact]
	public void testWasOnlineYesterday()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "Alice";
		DateTime currentTime = new DateTime(2019,05,09,9,15,0);
		DateTime lastSeenTime = currentTime.AddHours(-12); // " was online yesterday"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((nickname, " was online yesterday"), message);
	}
	
	[Fact]
	public void testWasOnlineThisWeek()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "Alice";
		DateTime currentTime = new DateTime(2019,05,09,9,15,0);
		DateTime lastSeenTime = currentTime.AddHours(-55); // " was online this week"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((nickname, " was online this week"), message);
	}

	[Fact]
	public void testWasOnlineLongTimeAgo()
	{
		// Arrange
		bool isOnline = false;
		string nickname = "Alice";
		DateTime currentTime = new DateTime(2019, 05, 09, 9, 15, 0);
		DateTime lastSeenTime = currentTime.AddDays(-39); // " was online a long time ago"

		// Act
		var message = UserStatusStorage.SetMessage(isOnline, nickname, currentTime, lastSeenTime);

		// Assert
		Assert.Equal((nickname, " was online a long time ago"), message);
	}
}