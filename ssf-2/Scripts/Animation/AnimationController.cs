using Godot;
using System.Collections.Generic;

public partial class AnimationController : Node2D
{
	private AnimatedSprite2D _sprite;
	private string _currentAnimation = "";
	private string _assetPath = "res://Assets/Characters/";

	public override void _Ready()
	{
		// Sprite2D is a sibling node
		_sprite = GetParent().GetNodeOrNull<AnimatedSprite2D>("Sprite2D");
	}

	public void InitializeCharacter(string characterName)
	{
		if (_sprite == null)
		{
			GD.PrintErr($"AnimationController: no AnimatedSprite2D found for {characterName}");
			return;
		}

		var spriteFrames = new SpriteFrames();
		spriteFrames.RemoveAnimation("default");

		var animMap = GetKrystalAnimations();

		foreach (var kvp in animMap)
		{
			string animName = kvp.Key;
			string[] frameNames = kvp.Value;

			spriteFrames.AddAnimation(animName);
			spriteFrames.SetAnimationLoop(animName, true);
			spriteFrames.SetAnimationSpeed(animName, 10f);

			foreach (string frameName in frameNames)
			{
				string path = $"{_assetPath}{characterName}/{frameName}.png";
				var tex = GD.Load<Texture2D>(path);
				if (tex != null)
					spriteFrames.AddFrame(animName, tex);
			}
		}

		_sprite.SpriteFrames = spriteFrames;
		PlayState(CharacterState.Idle);
		GD.Print($"AnimationController: loaded animations for {characterName}");
	}

	public void PlayAnimation(CharacterState state, bool stateChanged)
	{
		if (_sprite == null || _sprite.SpriteFrames == null) return;

		string animName = StateToAnim(state);

		if (!_sprite.SpriteFrames.HasAnimation(animName))
			animName = "idle";

		if (stateChanged || _currentAnimation != animName)
		{
			_currentAnimation = animName;
			_sprite.Animation = animName;
			_sprite.Play();
		}
	}

	private void PlayState(CharacterState state)
	{
		if (_sprite == null || _sprite.SpriteFrames == null) return;

		string animName = StateToAnim(state);
		if (_sprite.SpriteFrames.HasAnimation(animName))
		{
			_currentAnimation = animName;
			_sprite.Animation = animName;
			_sprite.Play();
		}
	}

	private string StateToAnim(CharacterState state) => state switch
	{
		CharacterState.Idle         => "idle",
		CharacterState.Walking      => "walk",
		CharacterState.Running      => "run",
		CharacterState.Jumping      => "jump",
		CharacterState.Falling      => "fall",
		CharacterState.Dashing      => "dash",
		CharacterState.Attacking    => "attack",
		CharacterState.SpecialAttack => "special",
		CharacterState.Stunned      => "hurt",
		CharacterState.ShieldActive => "shield",
		CharacterState.Grabbing     => "grab",
		CharacterState.Dead         => "dead",
		_ => "idle"
	};

	// --- Krystal animation map ---
	// Frame names correspond to PNG files: {prefix}{index}.png
	// e.g. krystal_i0 -> 834_krystal_i0.png (prefix includes number)

	private Dictionary<string, string[]> GetKrystalAnimations() => new()
	{
		["idle"]    = Frames("834_krystal_i", 0, 12),
		["walk"]    = Frames("834_krystal_w", 0, 10),
		["run"]     = Frames("834_krystal_r", 0, 8),
		["jump"]    = Frames("834_krystal_j", 0, 5),
		["fall"]    = Frames("834_krystal_f", 0, 2),
		["dash"]    = Frames("834_krystal_spd", 0, 20),
		["attack"]  = Frames("834_krystal_a", 0, 28),
		["special"] = Frames("834_krystal_sps", 0, 22),
		["hurt"]    = Frames("834_krystal_h", 0, 4),
		["shield"]  = Frames("834_krystal_shl", 0, 5),
		["grab"]    = Frames("834_krystal_g", 0, 7),
		["dead"]    = Frames("834_krystal_lose", 0, 17),
		["win"]     = Frames("834_krystal_win", 0, 35),
	};

	private static string[] Frames(string prefix, int start, int count)
	{
		var result = new string[count];
		for (int i = 0; i < count; i++)
			result[i] = $"{prefix}{start + i}";
		return result;
	}
}
