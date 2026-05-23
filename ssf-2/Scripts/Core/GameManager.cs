using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	[Export] public float FixedDeltaTime = 1f / 60f; // 60 FPS

	private InputManager _inputManager;
	private CombatSystem _combatSystem;
	private UIManager _uiManager;

	private Character _player1;
	private Character _player2;

	private int _currentFrame = 0;
	private bool _matchActive = false;
	private MatchState _matchState;

	private const int FIXED_TIMESTEP = 16667; // 60 FPS in microseconds

	public override void _Ready()
	{
		SetPhysicsProcess(false); // We'll use fixed timestep
		CreateSystems();
		InitializeMatch();
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
		var playersNode = GetNode("Players");

		// Find existing character nodes or create them
		_player1 = GetNodeOrNull<Character>("Players/KrystalP1");
		if (_player1 == null)
		{
			_player1 = GetNodeOrNull<Character>("Players/Player1");
		}
		if (_player1 == null)
		{
			// Try to find first child that is a Character
			foreach (Node child in playersNode.GetChildren())
			{
				if (child is Character charNode)
				{
					_player1 = charNode;
					break;
				}
			}
		}

		_player2 = GetNodeOrNull<Character>("Players/FoxP2");
		if (_player2 == null)
		{
			_player2 = GetNodeOrNull<Character>("Players/Player2");
		}
		if (_player2 == null)
		{
			// Create player 2 if not found
			_player2 = new Fox();
			_player2.CharacterIndex = 1;
			_player2.GlobalPosition = new Vector2(1000, 400);
			playersNode.AddChild(_player2);
		}

		// Ensure indices are set
		if (_player1 != null)
		{
			_player1.CharacterIndex = 0;
			_combatSystem.RegisterCharacter(_player1);
		}

		if (_player2 != null)
		{
			_player2.CharacterIndex = 1;
			_combatSystem.RegisterCharacter(_player2);
		}

		_matchState = new MatchState(_player1, _player2);
		_matchActive = true;

		GD.Print($"Match initialized: {_player1?.CharacterName} vs {_player2?.CharacterName}");
	}

	public override void _Process(double delta)
	{
		if (!_matchActive)
			return;

		// Simulate frame
		SimulateFrame();

		// Update UI
		_uiManager?.UpdateUI(_player1, _player2, _currentFrame);

		_currentFrame++;

		// Check match end
		CheckMatchEnd();
	}

	private void SimulateFrame()
	{
		// Get inputs from input manager
		InputFrame player1Input = _inputManager.GetInput(_currentFrame);
		InputFrame player2Input = GetPlayer2Input(); // Will be network input in online

		// Simulate both players
		_player1.SimulateFrame(player1Input, _player2, _currentFrame);
		_player2.SimulateFrame(player2Input, _player1, _currentFrame);

		// Update combat
		_combatSystem.UpdateCombat(_currentFrame);
	}

	private InputFrame GetPlayer2Input()
	{
		// For now, return empty input. Replace with network input in online mode
		// In local multiplayer, you'd add different input bindings
		return new InputFrame();
	}

	private void CheckMatchEnd()
	{
		if (_matchState.IsStockMode)
		{
			if (!_player1.IsAlive() || !_player2.IsAlive())
			{
				EndMatch();
			}
		}
		else
		{
			_matchState.TimeRemaining--;
			if (_matchState.TimeRemaining <= 0)
			{
				EndMatch();
			}
		}
	}

	private void EndMatch()
	{
		_matchActive = false;
		int player1Stocks = _player1.GetStocks();
		int player2Stocks = _player2.GetStocks();

		string winner = player1Stocks > player2Stocks ? "Player 1" : "Player 2";
		GD.Print($"Match ended! {winner} wins!");

		// Show end screen (will implement with UIManager)
	}

	public void PauseMatch()
	{
		_matchActive = false;
	}

	public void ResumeMatch()
	{
		_matchActive = true;
	}

	public void RestartMatch()
	{
		_currentFrame = 0;
		_matchState = new MatchState(_player1, _player2);
		_matchActive = true;
		_player1.ResetPosition();
		_player2.ResetPosition();
	}

	// Getters for rollback system
	public int GetCurrentFrame() => _currentFrame;
	public Character GetPlayer1() => _player1;
	public Character GetPlayer2() => _player2;
	public bool IsMatchActive() => _matchActive;
	public MatchState GetMatchState() => _matchState;
}
