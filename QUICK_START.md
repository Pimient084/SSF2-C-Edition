# SSF2 C# Edition - Quick Start Guide

## 🚀 Getting Started (5 Minutes)

### Prerequisites
- Godot Engine 4.6+ with C# support
- .NET 8.0 or later installed
- Windows, Mac, or Linux

### Step 1: Open the Project
1. Open Godot Engine
2. Click "Open Project"
3. Navigate to: `C:\Users\ADMIN\Documents\GitHub\SSF2-C-Edition\ssf-2\`
4. Click "Open"
5. Wait for project to load and compile C#

### Step 2: Run the Game
1. Press **F5** or click the "Run Project" button
2. See the game launch with Krystal fully animated!

### Step 3: Play!
```
Arrow Keys (← →)     Move left/right
Up Arrow / Space     Jump
Z                    Attack
X                    Special / Blaster
C                    Shield
V                    Grab
```

---

## 📊 What You'll See

- **Krystal** on the left (P1) with full sprite animations
- **Fox placeholder** on the right (P2) with colored square
- **Ground** at the bottom
- **HUD** showing damage %, stocks, and frame counter

---

## ⚙️ If You Get Errors

### Error: "Main.tscn failed to parse"
✅ **Fixed!** Scene files have been corrected.

### Error: "Script not found"
- Click the "Rescan" button in FileSystem
- Or restart Godot

### Error: "Texture not found"
- Ensure files are in: `ssf-2/Assets/Characters/Krystal/`
- Check that all 711 PNG files copied successfully

### Sprites not showing?
1. Check that Krystal's `CharacterName = "Krystal"` is set
2. Look at Godot console for texture load errors
3. Verify file path matches: `Assets/Characters/Krystal/834_krystal_*.png`

---

## 🎮 Test Features

### Movement & Physics
- Walk left/right (watch walk animation)
- Press space to jump (see jump animation)
- Fall down (fall animation)
- Land on ground

### Combat
- Press Z for normal attack (28-frame attack animation)
- Press X for blaster (rapid fire projectiles)
- Press C for shield (shield animation)
- Try attacking another character

### UI
- Bottom left: Frame counter (should reach 60 per second)
- Top left: P1 stats (character name, damage %, stocks)
- Top right: P2 stats

---

## 📁 Important Files

| File | Purpose |
|------|---------|
| `Main.tscn` | Main game scene (run this) |
| `ssf-2/Scripts/Core/GameManager.cs` | Game loop (60 FPS fixed) |
| `ssf-2/Scripts/Characters/Krystal.cs` | Krystal character logic |
| `ssf-2/Scripts/Animation/AnimationController.cs` | Sprite system |
| `ssf-2/Assets/Characters/Krystal/` | 711 sprite PNG files |

---

## 🔧 Quick Customization

### Change Player 1 Character
Edit `GameManager.cs`, find `InitializeMatch()`:
```csharp
_player1 = GetNodeOrNull<Character>("Players/KrystalP1");
// Change to:
_player1 = new Fox();
// or:
_player1 = new YourCustomCharacter();
```

### Change Player 2 Character
Same place, edit the `_player2` initialization.

### Adjust Game Speed
Edit `GameManager.cs`:
```csharp
[Export] public float FixedDeltaTime = 1f / 60f; // Change 60 to desired FPS
```

### Change Attack Damage
Edit `Krystal.cs`:
```csharp
protected override void CreateAttackHitbox(float angle, HitType type)
{
    int damage = type == HitType.Special ? 15 : 10; // Adjust these values
    float knockback = type == HitType.Special ? 15f : 10f;
    // ...
}
```

---

## 📚 Documentation

- **ARCHITECTURE.md** - How all systems work
- **GAME_DEVELOPMENT_GUIDE.md** - How to add new characters
- **KRYSTAL_CHARACTER_GUIDE.md** - Krystal stats and strategy
- **SPRITE_INTEGRATION_GUIDE.md** - How sprite system works
- **SESSION_SUMMARY.md** - Complete overview of what was built

---

## 🎯 Common Tasks

### Add a New Character
1. Copy sprite files to `Assets/Characters/YourName/`
2. Create `Scripts/Characters/YourName.cs` (inherit from Character)
3. Override `GetDefaultStats()` with character stats
4. Create `Scenes/Characters/yourname.tscn` scene
5. Update `AnimationController.cs` with animation mapping
6. Instantiate in `GameManager.cs`

### Change Attack Properties
Edit `Krystal.cs` → `CreateAttackHitbox()` method

### Adjust Physics (gravity, jump, speed)
Edit `Krystal.cs` → `GetDefaultStats()` method

### Add Sound Effects
(Coming in Phase 3)

---

## ✅ Verification Checklist

After opening the project, verify:

- [ ] Project opens without critical errors
- [ ] F5 launches the game
- [ ] Krystal sprite is visible
- [ ] Can move left/right
- [ ] Can jump
- [ ] Can attack (Z key)
- [ ] Attack animation plays
- [ ] Blaster fires (X key)
- [ ] HUD shows frame counter
- [ ] No console errors

---

## 🐛 Report Issues

If something doesn't work:
1. Check Godot Output console for error messages
2. Verify file paths match exactly
3. Restart Godot and rescan files
4. Ensure .NET 8.0 is installed
5. Check that all 711 Krystal PNGs are in Assets folder

---

## 🎉 You're All Set!

The game is ready to play and modify. Have fun, and feel free to experiment with:
- New characters
- Balance changes
- Animation timing
- UI improvements
- Special effects

**Happy developing!** 🎮⭐

---

*Built with Godot 4.6 + C# for ultimate fighting game power*
