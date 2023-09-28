using UserStatusLibrary;

namespace UserStatusTests;
using Xunit;

public class UnitTestLocalisation
{
	[Theory]
	[InlineData(" was online just now", " був(ла) в мережі щойно")]
	[InlineData(" was online 1 minute ago", " був(ла) в мережі 1 хвилину тому")]
	[InlineData(" was online a couple minutes ago", " був(ла) в мережі кілька хвилин тому")]
	[InlineData(" was online 1 hour ago", " був(ла) в мережі 1 годину тому")]
	[InlineData(" was online today", " був(ла) в мережі сьогодні")]
	[InlineData(" was online yesterday", " був(ла) в мережі вчора")]
	[InlineData(" was online this week", " був(ла) в мережі цього тижня")]
	[InlineData("unknown message", " був(ла) в мережі давно")]
	public void TranslateToUkrainian_ReturnsExpectedTranslation(string engMessage, string expectedTranslation)
	{
		// Act
		string translatedMessage = UserStatusStorage.LocalisationUkr(engMessage);

		// Assert
		Assert.Equal(expectedTranslation, translatedMessage);
	}
}
