using System.Net;

namespace IntegrationTest2;

public class Tests
{
	private const string BaseUrl = "http://localhost:5000/";
	[Theory]
	[InlineData("api/users?date=31.10.2023%2015:45","{\"usersOnline\":55}")]
	[InlineData("api/users?date=13.10.1999%2021:01","null")]
	[InlineData("api/users?date=13.10.2023%2021:01&userid=2fba2529-c166-8574-2da2-eac544d82634" ,"{\"wasUserOnline\":\"true\",\"nearestOnlineTime\":null}")]
	[InlineData("api/predictions?date=13.10.2025 23:33","timeless")]
	[InlineData("api/predictions?date=13.10.2025 23:33&userId=2fba2529-c166-8574-2da2-eac544d82634&tolerance=0,85","{\"willBeOnline\":\"false\",\"onlineChance\":\"0\"}")]
	[InlineData("api/total?userID=2fba2529-c166-8574-2da2-eac544d82634","timeless")]//"{\"totalTime\":482047}"
	[InlineData("api/average?userId=2fba2529-c166-8574-2da2-eac544d82634","timeless")]//"{\"weeklyAverage\":86261,\"dailyAverage\":603827}"
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
