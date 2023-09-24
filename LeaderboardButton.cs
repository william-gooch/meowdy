using Godot;
using System;

public class LeaderboardButton : Button
{
	private void _on_LeaderboardButton_pressed()
	{
		GD.Print("Go to leaderboard");
		var leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		GetTree().ChangeScene("res://leaderboard/LeaderboardScreen.tscn");
	}
}
