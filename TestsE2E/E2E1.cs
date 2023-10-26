using System.Net;

namespace IntegrationTest2;

public class Tests
{
	private const string BaseUrl = "http://localhost:5000/";
	[Theory]
	[InlineData("api/stats/users/?date=13.10.2023 21:01","{\"usersOnline\":59}")]
	[InlineData("api/stats/users/?date=13.10.1999 21:01","null")]
	[InlineData("api/stats/users/?date=13.10.2023 21:01&userId=2fba2529-c166-8574-2da2-eac544d82634" ,"timeless")]
	[InlineData("api/predictions/users/?date=13.10.2025 23:33","timeless")]
	[InlineData("api/predictions/users/?date=13.10.2025 23:33&userId=2fba2529-c166-8574-2da2-eac544d82634&tolerance=0,85","timeless")]
	[InlineData("api/stats/user/total/?userId=2fba2529-c166-8574-2da2-eac544d82634","timeless")]
	[InlineData("api/stats/user/average/?userId=2fba2529-c166-8574-2da2-eac544d82634","timeless")]
	public async Task IntegrationTestStatus(string link, string expected)
	{
		using (HttpClient client = new HttpClient())
		{
			// Assuming 'BaseUrl' is a field or property that holds the base URL.
			string requestUrl = BaseUrl + link;
			HttpResponseMessage response = await client.GetAsync(requestUrl);
			var stringResponse = response.Content.ReadAsStringAsync().Result;
			// Assert the status code.
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			if (expected != "timeless")
			{
				Assert.Equal(expected, stringResponse);
			}
		}
	}

}
