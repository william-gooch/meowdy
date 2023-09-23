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

    public override void _Ready()
    {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		leaderboard.Connect(nameof(LeaderboardManager.AuthenticationSuccess), this, nameof(OnAuthenticationSuccess));
		leaderboard.Connect(nameof(LeaderboardManager.LeaderboardChanged), this, nameof(CreateLeaderboard));

		if (leaderboard.LeaderboardItems != null) {
			CreateLeaderboard();
		}
    }

	private void OnAuthenticationSuccess () {
		Task.Run(() => SetupLeaderboard());
	}

    private async Task SetupLeaderboard() {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		Error error;
		string message;

		GD.Print("Getting leaderboard...");
		(error, _, message) = await leaderboard.GetLeaderboard();
		if (error == Error.Ok)
		{
			GD.Print("Got leaderboard!");
		}
		else
		{
			GD.PrintErr($"{error}: {message}");
		}
	}

	private void CreateLeaderboard() {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		Node loadingText = GetNodeOrNull("MarginContainer/LoadingText");
		if(loadingText != null) {
			loadingText.QueueFree();
		}

		Node container = GetNode(Container);
		foreach (Node child in container.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var item in leaderboard.LeaderboardItems) {
			var newItem = LeaderboardItem.Instance<LeaderboardItem>();
			newItem.Rank = item.Rank;
			newItem.PlayerName = item.PlayerName;
			newItem.Score = item.Score;
			container.AddChild(newItem);
		}
	}
}
