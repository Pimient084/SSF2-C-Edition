using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private InputManager _inputManager;
	private CombatSystem _combatSystem;
	private UIManager _uiManager;

	private Character _player1;
	private Character _player2;

	private int _currentFrame = 0;
	private bool _matchActive = false;
	private MatchState _matchState;

	public override void _Ready()
	{
		CreateSystems();
		CallDeferred(nameof(InitializeMatch)); // after scene tree is ready
	}

	private void CreateSystems()
	{
		_inputManager = new InputManager();
		AddChild(_inputManager);

		_combatSystem = new CombatSystem();
		AddChild(_combatSystem);

		_uiManager = GetNodeOrNull<UIManager>("UIManager");
		if (_uiManager == null)
		{
			_uiManager = new UIManager();
			AddChild(_uiManager);
		}
	}

	private void InitializeMatch()
	{
		var playersNode = GetNode<Node>("Players");

		_player1 = CreateCharacter<Krystal>(playersNode, new Vector2(300, 400), 0);
		_player2 = CreateCharacter<Fox>(playersNode, new Vector2(980, 400), 1);

		_combatSystem.RegisterCharacter(_player1);
		_combatSystem.RegisterCharacter(_player2);

		_matchState = new MatchState(_player1, _player2);
		_matchActive = true;

		GD.Print($"Match: {_player1.CharacterName} vs {_player2.CharacterName}");
	}

	private T CreateCharacter<T>(Node parent, Vector2 position, int index) where T : Character, new()
	{
		var character = new T();
		character.CharacterIndex = index;

		// Collision shape - required for CharacterBody2D physics
		var collision = new CollisionShape2D();
		var shape = new RectangleShape2D();
		shape.Size = new Vector2(60, 100);
		collision.Shape = shape;
		character.AddChild(collision);

		// Visual placeholder (color square until sprites load)
		var visual = new ColorRect();
		visual.Size = new Vector2(60, 100);
		visual.Position = new Vector2(-30, -50);
		visual.Color = index == 0 ? new Color(0.4f, 0.6f, 1f) : new Color(1f, 0.65f, 0.2f);
		character.AddChild(visual);

		parent.AddChild(character);
		character.GlobalPosition = position;

		return character;
	}

	public override void _Process(double delta)
	{
		if (!_matchActive || _player1 == null || _player2 == null)
			return;

		InputFrame p1Input = _inputManager.GetInput(_currentFrame);
		InputFrame p2Input = new InputFrame(); // P2 input placeholder

		_player1.SimulateFrame(p1Input, _player2, _currentFrame);
		_player2.SimulateFrame(p2Input, _player1, _currentFrame);

		_combatSystem.UpdateCombat(_currentFrame);

		_uiManager?.UpdateUI(_player1, _player2, _currentFrame);

		_currentFrame++;

		CheckMatchEnd();
	}

	private void CheckMatchEnd()
	{
		if (!_player1.IsAlive() || !_player2.IsAlive())
			EndMatch();
	}

	private void EndMatch()
	{
		_matchActive = false;
		string winner = _player1.GetStocks() > _player2.GetStocks()
			? _player1.CharacterName
			: _player2.CharacterName;
		GD.Print($"Match ended! {winner} wins!");
	}

	public int GetCurrentFrame() => _currentFrame;
}
