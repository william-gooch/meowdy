using Godot;
using System;

public class PauseButton : TextureButton
{
	private void _on_PauseButton_toggled(bool button_pressed)
	{
		GetTree().Paused = button_pressed;
	}
}
