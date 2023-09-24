using Godot;
using System;
using System.Threading.Tasks;

public class LeaderboardScreen : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private LeaderboardManager _leaderboard;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		GetNode<Label>("CenterContainer/PanelContainer/VBoxContainer/ScoreDisplay").Text = "You scored: " + _leaderboard.CurrentScore;
		GetNode<LineEdit>("CenterContainer/PanelContainer/VBoxContainer/HBoxContainer/NameEntry").Text = _leaderboard.PlayerName;
	}

	public void OnButtonPress()
	{
		string newName = GetNode<LineEdit>("CenterContainer/PanelContainer/VBoxContainer/HBoxContainer/NameEntry").Text;

		Task.Run(() => _leaderboard.SetPlayerName(newName));   
		Task.Run(() => _leaderboard.RecordScore());   
	}
}
