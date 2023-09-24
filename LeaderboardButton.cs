using Godot;
using System;

public class LeaderboardButton : Button
{
    public override void _Ready()
    {
		LeaderboardManager leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		Disabled = !leaderboard.IsAuthenticated;
		leaderboard.Connect(nameof(LeaderboardManager.AuthenticationSuccess), this, nameof(Enable));
    }

	private void Enable() {
		Disabled = false;
	}

    private void _on_LeaderboardButton_pressed()
	{
		GD.Print("Go to leaderboard");
		GetTree().ChangeScene("res://leaderboard/LeaderboardScreen.tscn");
	}
}
