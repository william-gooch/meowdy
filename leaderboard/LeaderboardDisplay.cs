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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Task.Run(() => SetupLeaderboard());
	}

    public async Task SetupLeaderboard() {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");

		GD.Print("Authenticating...");
		var (error, message) = await leaderboard.Authenticate();

		if (error == Error.Ok) {
			GD.Print("Authentication succeeded!");
		}
		else
		{
			GD.PrintErr($"{error}: {message}");
		}

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
