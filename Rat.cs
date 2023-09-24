using Godot;
using System;

public class Rat : Enemy
{
	[Export]
	public int Speed { get; set; }
	[Export]
	public float SwarmFactor { get; set; } // How much the enemies will swarm the player.

	private Random _rnd;
	private float _movementTimer = 0;
	private Vector2 _velocity = Vector2.Zero;

	protected HitAudio Audio { get; private set; }

	public Rat() : base()
	{
		_rnd = new Random();
	}

	public override void _Ready()
	{
		base._Ready();
		Audio = GetNode<HitAudio>("HitAudio");
	}

	public override Vector2 Move(float delta)
	{
		if (IsDead)
		{
			return base.Move(delta);
		}

		if (_movementTimer <= 0)
		{
			// Gaussian random movement. Directions further towards the player are more likely to occur, based on the swarming factor.
			float u1 = (float)_rnd.NextDouble();
			float u2 = (float)_rnd.NextDouble();
			float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(Mathf.Tau * u2);
			float randAngle = randStdNormal / SwarmFactor;
			_velocity = DirectionToPlayer().Rotated(randAngle);
			_movementTimer = 0.2f;
		}
		_movementTimer = Mathf.Max(0, _movementTimer - delta);

		_velocity = _velocity.Normalized() * Speed;
		return _velocity + base.Move(delta);
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		if (_velocity.x != 0)
		{
			Sprite.FlipH = _velocity.x > 0;
		}
	}

	protected override void OnHit(Vector2 direction)
	{
		Audio.Hit();
		base.OnHit(direction);
	}
}
