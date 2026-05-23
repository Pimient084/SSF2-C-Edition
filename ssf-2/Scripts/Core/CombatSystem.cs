using Godot;
using System.Collections.Generic;

public partial class CombatSystem : Node
{
	private List<Character> _characters = new();
	private HashSet<(int, int)> _hitsCachedThisFrame = new();

	public override void _Ready()
	{
		// Will be populated by GameManager
	}

	public void RegisterCharacter(Character character)
	{
		if (!_characters.Contains(character))
			_characters.Add(character);
	}

	public void UpdateCombat(int frameNumber)
	{
		_hitsCachedThisFrame.Clear();

		// Check all hitboxes against all characters
		for (int i = 0; i < _characters.Count; i++)
		{
			Character attacker = _characters[i];
			var hitboxes = attacker.GetActiveHitboxes();

			for (int h = 0; h < hitboxes.Count; h++)
			{
				Hitbox hitbox = hitboxes[h];

				// Check against all other characters
				for (int d = 0; d < _characters.Count; d++)
				{
					if (i == d) continue; // Can't hit yourself

					Character defender = _characters[d];

					if (CanHit(attacker, defender, hitbox, frameNumber))
					{
						// Use cache to prevent multi-hit on same frame
						var hitKey = (attacker.GetInstanceId().GetHashCode(), defender.GetInstanceId().GetHashCode());

						if (!_hitsCachedThisFrame.Contains(hitKey))
						{
							defender.TakeDamage(hitbox.HitData, attacker);
							_hitsCachedThisFrame.Add(hitKey);
							hitbox.HasHitThisFrame = true;

							GD.Print($"{attacker.CharacterName} hit {defender.CharacterName} for {hitbox.HitData.Damage} damage!");
						}
					}
				}
			}
		}
	}

	private bool CanHit(Character attacker, Character defender, Hitbox hitbox, int frameNumber)
	{
		if (!hitbox.IsActive(frameNumber))
			return false;

		if (hitbox.HasHitThisFrame && !hitbox.HitData.IsMultiHit)
			return false;

		// Use a fixed hurtbox around the defender's position
		var hurtbox = new Rect2(
			defender.GlobalPosition.X - 30,
			defender.GlobalPosition.Y - 100,
			60, 100
		);

		return hitbox.BoundingBox.Intersects(hurtbox);
	}

	public void Reset()
	{
		_characters.Clear();
		_hitsCachedThisFrame.Clear();
	}
}
