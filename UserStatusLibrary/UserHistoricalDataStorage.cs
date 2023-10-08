namespace UserStatusLibrary;

using System;
using System.IO;
using System.Timers;

public class UserHistoricalDataStorage
{
    public static async Task<int> Main()
    {
        try
        {
            var currTime = DateTime.Now;
            // Call a method to gather user data (e.g., GetUserStatusData).
            var userData = await GetUserStatusData(currTime.ToString());

            // Write the user data to a text file.
            WriteUserDataToFile(userData);

            Console.WriteLine("User data logged successfully.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0;
        }
    }

    private static async Task<Dictionary<string,(string,string,string)>> GetUserStatusData(string currTime)
    {
        int offset = 0;
        var link = "https://sef.podkolzin.consulting/api/users/lastSeen?offset=";
        var responseDictionary = new Dictionary<string, (string, string,string)>();
        var tempTry = await UserStatusStorage.ApiCall1(link,offset.ToString());
        if (tempTry.Item2 == 0) { return responseDictionary;}
        var tempResponseObject = UserStatusStorage.ParseData(tempTry.Item1);
        while (tempResponseObject.data.Count != 0)
        {
            foreach (var user in tempResponseObject.data)
            {
                responseDictionary.Add(user.userId,
                    user.isOnline == "True"
                        ? (currTime, user.isOnline, currTime)
                        : (currTime, user.isOnline, user.lastSeenDate));
            }

            offset += tempResponseObject.data.Count;
            tempTry = await UserStatusStorage.ApiCall1(link, offset.ToString());
            tempResponseObject = UserStatusStorage.ParseData(tempTry.Item1);
        }

        return responseDictionary;
    }

    private static int WriteUserDataToFile(Dictionary<string, (string, string, string)> userData)
    {
        string filePath = "user_status_log.txt";

        try
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                foreach (var kvp in userData)
                {
                    string userId = kvp.Key;
                    var userDataTuple = kvp.Value;
                    string currentTime = userDataTuple.Item1;
                    string isOnline = userDataTuple.Item2;
                    string lastSeenDate = userDataTuple.Item3;

                    // "UserID, CurrentTime, IsUserOnline, LastSeenTime".
                    string userDataLine = $"{userId}, {currentTime}, {isOnline}, {lastSeenDate}";

                    writer.WriteLine(userDataLine);
                }
            }

            Console.WriteLine("User data logged successfully.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0;
        }
    }
}
