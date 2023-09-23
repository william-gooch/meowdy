using Godot;

public class LeaderboardData : Resource
{
	[Export]
	public string APIKey { get; set; }
	[Export]
	public bool DevelopmentMode { get; set; }
	[Export]
	public string LeaderboardKey { get; set; }

	public LeaderboardData () : this("", true, "") {}

	public LeaderboardData (string apiKey, bool developmentMode, string leaderboardKey)
	{
		APIKey = apiKey;
		DevelopmentMode = developmentMode;
		LeaderboardKey = leaderboardKey;
	}
}