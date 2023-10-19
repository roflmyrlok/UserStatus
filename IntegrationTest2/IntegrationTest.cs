using System.Net;

namespace IntegrationTest2;

public class Tests
{
	private const string BaseUrl = "http://localhost:5000/";

	[Theory]
	[InlineData("api/stats/users/?date=13.10.2023 21:01:33")]
	[InlineData("api/stats/users/?date=13.10.2025 21:01:33")]
	[InlineData("api/stats/users/?date=13.10.2025 21:01:33&userId=2fba2529-c166-8574-2da2-eac544d82634" )]
	[InlineData("api/predictions/users/?date=13.10.2025")]
	[InlineData("api/predictions/users/?date=13.10.2025&id=2fba2529-c166-8574-2da2-eac544d82634&tolerance=0.85")]
	[InlineData("api/stats/user/total/?userId=2fba2529-c166-8574-2da2-eac544d82634")]
	[InlineData("api/stats/user/average/?userId=2fba2529-c166-8574-2da2-eac544d82634")]
	public void IntegrationTestStatus(string link)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl + link);
		request.Method = "GET";

		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}