using Godot;
using System;

//References:
//license: Freeware, Non-Commercial
//link: https://www.fontspace.com/merkur-font-f11606

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
}
