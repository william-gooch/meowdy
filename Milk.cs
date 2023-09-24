using Godot;
using System;

public class Milk : Area2D
{
	private HUD HUD;
	public override void _Ready() {
		HUD = GetNode<HUD>("/root/Stage/HUD");
		AnimatedSprite Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("default");
	}
	private void _on_Milk_area_entered(object area)
	{
		if (area is Player) {
			HUD.Call("AddHealth");
			this.QueueFree();
		}
	}
}
