using Godot;
using System;

public class BigRat : Rat
{
	[Export]
	public float FlashTime { get; set; }
	[Export]
	public float DashCooldown { get; set; }
	[Export]
	public float DashWarning { get; set; }
	[Export]
	public float DashSpeed { get; set; }
	[Export]
	public float DashDamping { get; set; }

	private Vector2 _velocity = Vector2.Zero;

	private float _dashTimer;
	private float _warningTimer = 0;
	private Arrow _warningArrow;
	private Vector2 _plannedDirection;
	private float _flashCooldown = 0;

	public BigRat() : base() {
		_dashTimer = DashCooldown;
	}

	public override void _Ready()
	{
		base._Ready();
		_warningArrow = GetNode<Arrow>("WarningArrow");

		Connect(nameof(HitEventHandler), this, nameof(OnHit));
	}

	public override Vector2 Move(float delta) {
		if (_dashTimer <= 0 && _warningTimer <= 0) {
			_velocity = _plannedDirection * DashSpeed;
			_dashTimer = DashCooldown;
			Sprite.SpeedScale = 1f;
			_warningArrow.Hide();
		} else if (_warningTimer <= 0) {
			base.Move(delta);
		}

		_velocity *= 1 - (DashDamping * delta);
		if (_velocity.Length() < 10)
		{
			_velocity = Vector2.Zero;
		}

		Position += _velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);

		if(_dashTimer > 0 && _dashTimer - delta <= 0) {
			_warningTimer = DashWarning;
			_plannedDirection = DirectionToPlayer();
			_warningArrow.Show();
			_warningArrow.GlobalRotation = Vector2.Up.AngleTo(_plannedDirection);
			Sprite.SpeedScale = 3f;
		}
		_dashTimer = Mathf.Max(0, _dashTimer - delta);
		_warningTimer = Mathf.Max(0, _warningTimer - delta);

		if (_flashCooldown <= 0) {
			Sprite.Modulate = new Color("#ffffff");
		}
		_flashCooldown = Mathf.Max(0, _flashCooldown - delta);

		return _velocity;
	}

	private void OnHit()
	{
		Sprite.Play("bloody_walk");
		Sprite.Modulate = new Color("#960000");
		_flashCooldown = FlashTime;
	}
}
