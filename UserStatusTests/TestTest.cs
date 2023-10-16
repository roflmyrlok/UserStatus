using Microsoft.AspNetCore.Hosting;

namespace UserStatusTests;

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class IntegrationTest : IClassFixture<WebApplicationFactory<StartupBase>>
{
	private readonly WebApplicationFactory<StartupBase> _factory;

	public IntegrationTest(WebApplicationFactory<StartupBase> factory)
	{
		_factory = factory;
	}

	[Theory]
	[InlineData("2023-10-13-12:00:00")]
	[InlineData("2023-10-13-15:30:00")]
	public async void GetUsers_ReturnsExpectedResult(string date)
	{
		// Arrange
		var client = _factory.CreateClient();
		var requestUrl = $"/api/stats/users/1/?date={date}";

		// Act
		var response = await client.GetAsync(requestUrl);
		var content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.True(response.IsSuccessStatusCode);
		Assert.Contains("usersOnline:", content);
	}
}

