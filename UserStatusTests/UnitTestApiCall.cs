using System.Net;
using UserStatusLibrary;
using Moq;
using Moq.Protected;

public class UnitTestApiCall
{
    [Fact]
    public static async Task UnitTestFillDIct()
    {
        //arrange
        string link = "https://sef.podkolzin.consulting/api/users/lastSeen?offset=";
        // Act
        var result = await UserStatusStorage.FillUserStatusDictionary(link); // Await the async method

        // Assert
        Assert.IsType<Dictionary<string,string>>(result);
    }
    
	[Fact]
	public static async Task UnitTestApiCall1()
	{
        //arrange
        string link = "https://sef.podkolzin.consulting/api/users/lastSeen?offset=";
		// Act
        var result = await UserStatusStorage.ApiCall1(link,"0"); // Await the async method

		// Assert
		Assert.Equal(1,result.Item2);
	}

    [Fact]
    public async Task UnitTestParseData()
    {
        // Arrange
        string fakeJsonResponse = @"{
    ""total"": 217,
    ""data"": [
        {
            ""userId"": ""2fba2529-c166-8574-2da2-eac544d82634"",
            ""nickname"": ""Doug93"",
            ""firstName"": ""Doug"",
            ""lastName"": ""Rogahn"",
            ""registrationDate"": ""2023-06-04T03:53:45.4490942+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:30:48.0799983+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""8b0b5db6-19d6-d777-575e-915c2a77959a"",
            ""nickname"": ""Nathaniel6"",
            ""firstName"": ""Nathaniel"",
            ""lastName"": ""Murphy"",
            ""registrationDate"": ""2023-09-19T08:48:06.5731641+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:16:47.9768436+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""e13412b2-fe46-7149-6593-e47043f39c91"",
            ""nickname"": ""Terry_Weber"",
            ""firstName"": ""Terry"",
            ""lastName"": ""Weber"",
            ""registrationDate"": ""2022-10-24T17:46:53.1388008+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:29:48.0739523+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""cbf0d80b-8532-070b-0df6-a0279e65d0b2"",
            ""nickname"": ""Willard66"",
            ""firstName"": ""Willard"",
            ""lastName"": ""Treutel"",
            ""registrationDate"": ""2023-05-04T23:29:34.6069562+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:20:08.0088552+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""de5b8815-1689-7c78-44e1-33375e7e2931"",
            ""nickname"": ""Nick37"",
            ""firstName"": ""Nick"",
            ""lastName"": ""Boyle"",
            ""registrationDate"": ""2023-08-08T07:18:54.3367009+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:31:48.0883541+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""e31e41f4-4992-cd5d-def4-4c79f86bb0e1"",
            ""nickname"": ""Jerry_Adams6"",
            ""firstName"": ""Jerry"",
            ""lastName"": ""Adams"",
            ""registrationDate"": ""2022-09-24T15:33:18.0113307+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:13:47.9596402+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""908dcb71-beeb-57c4-72f6-50451a6c3d12"",
            ""nickname"": ""Leticia.Pagac"",
            ""firstName"": ""Leticia"",
            ""lastName"": ""Pagac"",
            ""registrationDate"": ""2022-09-29T23:41:42.1597638+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""5ed4eae5-d93c-6b18-be47-93a787c73bcb"",
            ""nickname"": ""Myrtle27"",
            ""firstName"": ""Myrtle"",
            ""lastName"": ""Ernser"",
            ""registrationDate"": ""2022-11-19T06:54:20.5472617+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""3f9747d7-d084-7db4-5226-220085e07b54"",
            ""nickname"": ""Erik_Abshire"",
            ""firstName"": ""Erik"",
            ""lastName"": ""Abshire"",
            ""registrationDate"": ""2023-03-20T00:31:49.2624028+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""05227367-07f0-b3a5-8345-2513e0c45cca"",
            ""nickname"": ""Robin.Herman70"",
            ""firstName"": ""Robin"",
            ""lastName"": ""Herman"",
            ""registrationDate"": ""2023-02-04T07:08:53.4751734+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""4a198b3f-6696-e319-2cbf-c50fa09f4ecc"",
            ""nickname"": ""Candice_Farrell39"",
            ""firstName"": ""Candice"",
            ""lastName"": ""Farrell"",
            ""registrationDate"": ""2023-04-05T08:31:56.245789+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:20:48.0101677+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""bb367131-ec06-3d69-a861-eeca3f9cc88d"",
            ""nickname"": ""Eva35"",
            ""firstName"": ""Eva"",
            ""lastName"": ""Turner"",
            ""registrationDate"": ""2022-11-18T00:26:01.3160615+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:31:48.0883569+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""02d4563d-5727-c811-b3b7-57a10f6be25a"",
            ""nickname"": ""Marlene_Witting"",
            ""firstName"": ""Marlene"",
            ""lastName"": ""Witting"",
            ""registrationDate"": ""2023-01-24T14:19:08.2954867+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:26:48.0522441+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""a807e6f7-ec9c-f8a6-a6e4-43b8f36c78cc"",
            ""nickname"": ""Hector.Ankunding"",
            ""firstName"": ""Hector"",
            ""lastName"": ""Ankunding"",
            ""registrationDate"": ""2023-01-04T08:34:37.798541+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""e9de6dd1-84e5-9833-59de-8c51008de6a0"",
            ""nickname"": ""Emmett82"",
            ""firstName"": ""Emmett"",
            ""lastName"": ""Block"",
            ""registrationDate"": ""2023-06-08T21:50:41.1053254+00:00"",
            ""lastSeenDate"": ""2023-09-28T15:56:07.8199725+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""d59e1aae-c9cc-5c17-4458-4f02bdc494d2"",
            ""nickname"": ""Catherine.McCullough"",
            ""firstName"": ""Catherine"",
            ""lastName"": ""McCullough"",
            ""registrationDate"": ""2023-03-29T09:03:35.9109743+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:25:28.0379397+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""acb1c13f-a700-ae85-97fa-daa56573f7bd"",
            ""nickname"": ""Dominic.Daugherty55"",
            ""firstName"": ""Dominic"",
            ""lastName"": ""Daugherty"",
            ""registrationDate"": ""2022-10-08T05:25:36.4221263+00:00"",
            ""lastSeenDate"": null,
            ""isOnline"": true
        },
        {
            ""userId"": ""2e164e3c-3abd-a835-8e00-a3fa4b1d636e"",
            ""nickname"": ""Enrique_Vandervort17"",
            ""firstName"": ""Enrique"",
            ""lastName"": ""Vandervort"",
            ""registrationDate"": ""2023-03-20T19:42:18.732595+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:09:07.9211658+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""88885096-1825-640b-1dff-281b668b24e5"",
            ""nickname"": ""Ted32"",
            ""firstName"": ""Ted"",
            ""lastName"": ""Toy"",
            ""registrationDate"": ""2023-07-07T17:33:57.5700768+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:22:08.0185702+00:00"",
            ""isOnline"": false
        },
        {
            ""userId"": ""99f065e9-2796-0dc7-97c4-8ea28db20bf7"",
            ""nickname"": ""Eileen_Schneider30"",
            ""firstName"": ""Eileen"",
            ""lastName"": ""Schneider"",
            ""registrationDate"": ""2022-10-25T05:37:31.5933049+00:00"",
            ""lastSeenDate"": ""2023-09-28T16:21:08.0160319+00:00"",
            ""isOnline"": false
        }
    ]
}";
        var result = UserStatusStorage.ParseData(fakeJsonResponse);
        
        Assert.Equal(20, result.data.Count);
    }
}