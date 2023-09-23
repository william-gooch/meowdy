using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LeaderboardDisplay : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	[Export]
	public PackedScene LeaderboardItem { get; set; }
	[Export]
	public NodePath Container { get; set; }

	private List<LeaderboardManager.LeaderboardItem> _leaderboardItems;

    public override void _Ready()
    {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		leaderboard.Connect(nameof(LeaderboardManager.AuthenticationSuccess), this, nameof(SetupLeaderboard));
    }

    public async Task SetupLeaderboard() {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		Error error;
		string message;

		GD.Print("Getting leaderboard...");
		(error, _leaderboardItems, message) = await leaderboard.GetLeaderboard();
		if (error == Error.Ok)
		{
			GD.Print("Got leaderboard!");
			CallDeferred("CreateLeaderboard");
		}
		else
		{
			GD.PrintErr($"{error}: {message}");
		}
	}

	private void CreateLeaderboard() {
		Node container = GetNode(Container);
		foreach (var item in _leaderboardItems) {
			var newItem = LeaderboardItem.Instance<LeaderboardItem>();
			newItem.PlayerName = item.PlayerName;
			newItem.Score = item.Score;
			container.AddChild(newItem);
		}
	}
}
