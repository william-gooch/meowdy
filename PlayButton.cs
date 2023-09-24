using Godot;
using System;

public class PlayButton : Button
{
	private void _on_PlayButton_pressed()
	{
		PackedScene Stage = GD.Load<PackedScene>("res://Stage.tscn");
		GetTree().ChangeSceneTo(Stage);
		//ResourceLoader.Load<PackedScene>("res://Stage.tscn").Instance();;
	}
}
