using Godot;
using System;

public class HUD : Node
{
	public int Score = 0;
	private Label ScoreLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScoreLabel = GetNode<Label>("Score");
	}
	
	public override void _Process(float delta) {
		ScoreLabel.Text = Score.ToString();
	}
	
	public void AddScore(int Addition) {
		Score += Addition;
	}
}
