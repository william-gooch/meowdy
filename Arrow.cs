using Godot;
using System;

public class Arrow : Line2D
{
	[Export]
	public float Length { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Points = new[] {
			new Vector2(  0,  0),
			new Vector2(  0,  0 - Length),
			new Vector2(-25, 25 - Length),
			new Vector2(  0,  0 - Length),
			new Vector2( 25, 25 - Length),
		};
	}
}
