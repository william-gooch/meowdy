using Godot;
using System;

public class TitleScreenButton : Button
{
	private void _on_TitleScreenButton_pressed()
	{
		PackedScene Stage = GD.Load<PackedScene>("res://stages/TitleScreen.tscn");
		GetTree().ChangeSceneTo(Stage);
	}
}
