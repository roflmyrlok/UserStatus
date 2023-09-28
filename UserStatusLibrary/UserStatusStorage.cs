using Newtonsoft.Json;

namespace UserStatusLibrary;

public class UserStatusStorage
{
    public static ResponseObject Users;
    public static string BaseUrl;

    static UserStatusStorage()
    {
        Users = new ResponseObject();
        BaseUrl = "https://sef.podkolzin.consulting/api/users/lastSeen";;
    }
    public static (string, string) SetMessage(bool isOnline, string nickname, DateTime currentTime, DateTime lastSeenTime)
    {
        var year2000 = new DateTime(2000, 1, 1);
        var timeDifferenceSpan = (currentTime - year2000) - (lastSeenTime - year2000);
        // Get the total number of seconds
        var secondsPassed = (long) timeDifferenceSpan.TotalSeconds;
        if (isOnline) { return (nickname, "is online now"); }
        switch (secondsPassed)
        {
            case < 30:
                return (nickname, " was online just now");
            case < 60:
                return (nickname, " was online 1 minute ago");
            case < 3540:
                return (nickname, " was online a couple minutes ago");
            case < 7140:
                return (nickname, " was online 1 hour ago");
            case < 86400 * 2:
                if (lastSeenTime.Date == currentTime.Date)
                { return (nickname, " was online today"); }
                else { return (nickname, " was online yesterday"); }
            case < 604800:
                return (nickname, " was online this week");
            default:
                return (nickname, " was online a long time ago");
        }
    }

    public static async Task<int> FillResponseObject()
    {
        using HttpClient client = new HttpClient();
        try
        {
            
            var offset = 0;
            while (true)
            {
                var url = BaseUrl + "?offset=" + offset;
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tempResponseObject = JsonConvert.DeserializeObject<ResponseObject>(json);
                    if (Users == null || tempResponseObject == null)
                    {
                        throw new Exception("Users is not initialized");
                    }

                    Users.data ??= new List<UserData>();  // Initialize Users.data if it's null
                    tempResponseObject.data ??= new List<UserData>(); 
                    Users.data.AddRange(tempResponseObject.data);
                    if (Users.data.Count % 20 != 0) { break; }
                    offset += 20;
                }
                else
                {
                    Console.WriteLine($"Request failed with status code {response.StatusCode}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return 0;
        }
        return 1;
    }

    public static async Task<Dictionary<string, string>> FillDictionary()
    {
        var responseDictionary = new Dictionary<string, string>();
        var tempTry = await Task.Run((FillResponseObject));
        Console.WriteLine(tempTry);
        foreach (var user in Users.data)
        {
            var lastSeen = DateTime.Now;
            var currentTime = DateTime.Now;
            if (user.lastSeenDate != null)
            {
                lastSeen = DateTime.Parse(user.lastSeenDate);
            }

            var message = SetMessage(Convert.ToBoolean(user.isOnline), user.nickname, currentTime, lastSeen);
            responseDictionary.Add(message.Item1, message.Item2);
        }
        return responseDictionary;
    }
}