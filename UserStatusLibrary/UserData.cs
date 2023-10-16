namespace UserStatusLibrary;

public class UserData
{
	public string userId { get; set; }
	public string nickname { get; set; }
	public string firstName { get; set; }
	public string lastName { get; set; }
	public string registrationDate { get; set; }
	public string lastSeenDate { get; set; }
	public string isOnline { get; set; }

	public List<TimeSegment> onlineStart { get; set; }
	public List<TimeSegment> onlineEnd { get; set; }
}