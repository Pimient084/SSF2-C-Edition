using Godot;

/// Krystal - Star Fox character
/// Lightweight, fast character with staff-based attacks
/// Features: Blaster projectile, magic recovery move, fast combo gameplay
public partial class Krystal : Character
{
	private int _blasterCooldown = 0;
	private const int BLASTER_COOLDOWN = 40; // Frames between shots

	public Krystal()
	{
		CharacterName = "Krystal";
		CharacterIndex = 0;
	}

	public override CharacterStats GetDefaultStats()
	{
		return new CharacterStats
		{
			// Krystal is lightweight and fast
			Weight = 77f,              // Light character (lower weight = more knockback)
			MovementSpeed = 1.3f,      // Fast ground movement
			AirAcceleration = 1.0f,    // Good air control
			JumpHeight = 1.15f,        // Above average jump
			DashSpeed = 1.8f,          // Fast dash
			MaxAirJumps = 1,           // One air jump
			ShieldDurability = 75f     // Good shield (scale 1.25 in flash)
		};
	}

	public override void SimulateFrame(InputFrame input, Character opponent, int frameNumber)
	{
		// Update cooldowns
		if (_blasterCooldown > 0)
			_blasterCooldown--;

		base.SimulateFrame(input, opponent, frameNumber);
	}

	protected override void HandleInput(InputFrame input)
	{
		base.HandleInput(input);

		// Blaster special (B neutral) - rapid fire projectiles
		if (input.SpecialPressed && _blasterCooldown <= 0 && _currentState != CharacterState.Stunned)
		{
			CreateBlasterShot();
			_blasterCooldown = BLASTER_COOLDOWN;
		}
	}

	protected override void CreateAttackHitbox(float angle, HitType type)
	{
		// Krystal's staff-based attacks
		int damage = 0;
		float knockback = 0f;
		int hitstun = 0;

		if (type == HitType.Special)
		{
			// Smash attacks with charged versions
			damage = 13;  // Base smash damage
			knockback = 14f;
			hitstun = 15;
		}
		else
		{
			// Normal attacks
			damage = 8;
			knockback = 8f;
			hitstun = 12;
		}

		var hitData = new HitData(
			damage: damage,
			knockback: knockback,
			angle: angle,
			type: type,
			hitstun: hitstun
		);

		var hitbox = new Hitbox(
			id: _activeHitboxes.Count,
			box: GetAttackHitboxBounds(),
			hit: hitData,
			startFrame: _currentFrame,
			endFrame: _currentFrame + 8
		);

		_activeHitboxes.Add(hitbox);
	}

	private void CreateBlasterShot()
	{
		// Blaster projectile - light damage, fast, multiple in air
		var blasterDamage = new HitData(
			damage: 2,
			knockback: 0f,  // Minimal knockback for combo tool
			angle: 0f,
			type: HitType.Normal,
			hitstun: 2
		);

		var hitbox = new Hitbox(
			id: _activeHitboxes.Count,
			box: new Rect2(
				GlobalPosition.X + (_isFacingRight ? 100f : -100f),
				GlobalPosition.Y - 30f,
				20,
				20
			),
			hit: blasterDamage,
			startFrame: _currentFrame,
			endFrame: _currentFrame + 30  // Projectile lasts longer
		);

		_activeHitboxes.Add(hitbox);
		GD.Print($"Krystal blaster shot fired!");
	}

	// Recovery move: Magic Up-B (teleportation style recovery)
	protected override void Jump()
	{
		// Krystal has a special recovery move (up-B in air)
		// But standard jump is still good
		_velocity.Y = -920f * _stats.JumpHeight; // Slightly higher base jump
		_currentState = CharacterState.Jumping;
		_frameInState = 0;
	}

	public override void TakeDamage(HitData hit, Character attacker)
	{
		// Krystal's shield properties
		_shieldHealth -= hit.Damage * 0.5f; // Shield is better than base

		base.TakeDamage(hit, attacker);
	}


	protected override Rect2 GetAttackHitboxBounds()
	{
		// Staff range - longer than unarmed fighters
		float offsetX = _isFacingRight ? 100f : -100f;
		return new Rect2(GlobalPosition.X + offsetX - 50, GlobalPosition.Y - 70, 100, 130);
	}
}
