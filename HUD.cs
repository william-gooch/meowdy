using Godot;
using System;

//References:
//license: Freeware, Non-Commercial
//link: https://www.fontspace.com/merkur-font-f11606

public class HUD : Node
{
	public int Score = 0;
	public int DashCooldownPercentage = 50;
	private Label ScoreLabel;
	private ProgressBar DashCooldownBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScoreLabel = GetNode<Label>("Score");
		DashCooldownBar = GetNode<ProgressBar>("DashCooldownBar");
	}
	
	public override void _Process(float delta) {
		ScoreLabel.Text = Score.ToString();
		DashCooldownBar.Value = DashCooldownPercentage;
	}
}
