using System.Net;

namespace IntegrationTest2;

public class Tests
{
	private const string BaseUrl = "http://localhost:5000/";
	[Theory]
	[InlineData("api/users?date=31.10.2023-15:45","timeless")]
	[InlineData("api/users?date=13.10.1999-21:01","null")]
	[InlineData("api/users?date=13.10.2023-21:01&8b0b5db6-19d6-d777-575e-915c2a77959a" ,"timeless")]
	[InlineData("api/predictions?date=13.10.2025-23:33","timeless")]
	[InlineData("api/predictions?date=13.10.2025-23:33&8b0b5db6-19d6-d777-575e-915c2a77959a&tolerance=0,85","timeless")]
	[InlineData("api/total?userID=8b0b5db6-19d6-d777-575e-915c2a77959a","timeless")]//"{\"totalTime\":482047}"
	[InlineData("api/average?userId=8b0b5db6-19d6-d777-575e-915c2a77959a","timeless")]//"{\"weeklyAverage\":86261,\"dailyAverage\":603827}"
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
