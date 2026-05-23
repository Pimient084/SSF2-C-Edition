using Godot;
using System.Collections.Generic;

/// Controller para manejar sprites y animaciones de personajes
/// Mapea estados del juego a archivos PNG específicos
public partial class AnimationController : Node2D
{
	private AnimatedSprite2D _sprite;
	private Dictionary<string, SpriteFrame> _animations = new();
	private string _currentAnimation = "";
	private string _characterName = "";
	private string _assetPath = "res://Assets/Characters/";

	public override void _Ready()
	{
		// Sprite2D is a sibling, not a child
		_sprite = GetParent().GetNode<AnimatedSprite2D>("Sprite2D");
		if (_sprite == null)
		{
			GD.PrintErr("AnimationController: Could not find Sprite2D node!");
		}
	}

	public void InitializeCharacter(string characterName)
	{
		_characterName = characterName;
		LoadAnimations(characterName);
	}

	private void LoadAnimations(string characterName)
	{
		// Create SpriteFrames resource
		var spriteFrames = new SpriteFrames();
		spriteFrames.AddAnimation("default");

		// Define animation mappings for the character
		var animationMap = GetAnimationMap(characterName);

		foreach (var kvp in animationMap)
		{
			string animName = kvp.Key;
			string[] frameNames = kvp.Value;

			// Create animation
			spriteFrames.AddAnimation(animName);

			foreach (string frameName in frameNames)
			{
				string texturePath = $"{_assetPath}{characterName}/{frameName}.png";
				var texture = GD.Load<Texture2D>(texturePath);

				if (texture != null)
				{
					spriteFrames.AddFrame(animName, texture);
				}
				else
				{
					GD.PrintErr($"Failed to load texture: {texturePath}");
				}
			}

			// Set animation speed
			spriteFrames.SetAnimationLoop(animName, true);
			spriteFrames.SetAnimationSpeed(animName, 10.0f); // 10 FPS default
		}

		_sprite.SpriteFrames = spriteFrames;
		GD.Print($"Loaded {_animations.Count} animations for {characterName}");
	}

	public void PlayAnimation(CharacterState state, bool flip = false)
	{
		string animName = GetAnimationNameFromState(state);

		if (_sprite != null && animName != _currentAnimation)
		{
			_sprite.Animation = animName;
			_sprite.Play();
			_currentAnimation = animName;
		}

		if (_sprite != null && flip)
		{
			_sprite.FlipH = !_sprite.FlipH;
		}
	}

	private string GetAnimationNameFromState(CharacterState state)
	{
		return state switch
		{
			CharacterState.Idle => "idle",
			CharacterState.Walking => "walk",
			CharacterState.Running => "run",
			CharacterState.Jumping => "jump",
			CharacterState.Falling => "fall",
			CharacterState.Dashing => "dash",
			CharacterState.Attacking => "attack",
			CharacterState.SpecialAttack => "special",
			CharacterState.Stunned => "hurt",
			CharacterState.ShieldActive => "shield",
			CharacterState.Grabbing => "grab",
			CharacterState.Dead => "dead",
			_ => "idle"
		};
	}

	private Dictionary<string, string[]> GetAnimationMap(string characterName)
	{
		if (characterName == "Krystal")
			return GetKrystalAnimations();
		else
			return GetDefaultAnimations();
	}

	private Dictionary<string, string[]> GetKrystalAnimations()
	{
		// Map Krystal's animation files to game states
		// Format: animation_name -> array of sprite frame names
		return new Dictionary<string, string[]>
		{
			// Core states
			["idle"] = GenerateFrameNames("krystal_i", 0, 12),
			["walk"] = GenerateFrameNames("krystal_w", 0, 10),
			["run"] = GenerateFrameNames("krystal_r", 0, 8),
			["jump"] = GenerateFrameNames("krystal_j", 0, 5),
			["fall"] = GenerateFrameNames("krystal_f", 0, 2),
			["dash"] = GenerateFrameNames("krystal_spd", 0, 20),

			// Attacks
			["attack"] = GenerateFrameNames("krystal_a", 0, 28),
			["attack_forward"] = GenerateFrameNames("krystal_af", 0, 10),
			["attack_up"] = GenerateFrameNames("krystal_au", 0, 9),
			["attack_down"] = GenerateFrameNames("krystal_ad", 0, 17),

			// Special attacks
			["special"] = GenerateFrameNames("krystal_sps", 0, 22),
			["special_forward"] = GenerateFrameNames("krystal_spd", 0, 20),
			["special_up"] = GenerateFrameNames("krystal_spu", 0, 15),
			["special_down"] = GenerateFrameNames("krystal_spd", 0, 20),

			// Status states
			["hurt"] = GenerateFrameNames("krystal_h", 0, 4),
			["shield"] = GenerateFrameNames("krystal_shl", 0, 5),
			["grab"] = GenerateFrameNames("krystal_g", 0, 7),
			["dead"] = GenerateFrameNames("krystal_lose", 0, 17),

			// Taunts and victory
			["taunt"] = GenerateFrameNames("krystal_tan", 0, 10),
			["win"] = GenerateFrameNames("krystal_win", 0, 35),

			// Entrance
			["entrance"] = GenerateFrameNames("krystal_en", 0, 10),
		};
	}

	private Dictionary<string, string[]> GetDefaultAnimations()
	{
		// Fallback animation map for unknown characters
		return new Dictionary<string, string[]>
		{
			["idle"] = new[] { "idle_0" },
			["walk"] = new[] { "walk_0" },
			["run"] = new[] { "run_0" },
			["jump"] = new[] { "jump_0" },
			["fall"] = new[] { "fall_0" },
			["attack"] = new[] { "attack_0" },
			["special"] = new[] { "special_0" },
			["hurt"] = new[] { "hurt_0" },
			["shield"] = new[] { "shield_0" },
			["grab"] = new[] { "grab_0" },
			["dead"] = new[] { "dead_0" },
		};
	}

	private string[] GenerateFrameNames(string prefix, int start, int count)
	{
		var frames = new string[count];
		for (int i = 0; i < count; i++)
		{
			// Format: 834_krystal_a0, 834_krystal_a1, etc.
			int frameNumber = start + i;
			frames[i] = $"834_{prefix}{frameNumber}";
		}
		return frames;
	}

	public void SetAnimationSpeed(string animName, float speed)
	{
		if (_sprite?.SpriteFrames != null)
		{
			_sprite.SpriteFrames.SetAnimationSpeed(animName, speed);
		}
	}

	public void Stop()
	{
		_sprite?.Stop();
	}

	public void Play()
	{
		_sprite?.Play();
	}

	public bool IsAnimationFinished()
	{
		return _sprite?.IsAnimationFinished() ?? false;
	}

	public string GetCurrentAnimation()
	{
		return _currentAnimation;
	}
}
