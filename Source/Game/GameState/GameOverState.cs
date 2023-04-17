using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameOverState : GameState, IKeyboardPressedEventListener
{
    private Texture2D _gameOverText;
    private Texture2D _restartPrompt;
    
    public GameOverState(Game1 root) : base(root)
    {
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
    }
    
    public override List<DrawTask> GetDrawTasks()
    {
        Vector2 textPosition = new(
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 3D));
        
        Vector2 promptPosition = new(
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 2D));

        DrawTask gameOverText = new(
            _gameOverText, 
            textPosition, 
            0, 
            LayerDepth.HUD, 
            new List<IDrawTaskEffect>());

        DrawTask restartPrompt = new(
            _restartPrompt,
            promptPosition,
            0,
            LayerDepth.HUD,
            new List<IDrawTaskEffect>());
        
        string scoreText = $"Score: {Root.Score}";
        Color textColor = Palette.GetColor(Palette.Colors.Grey9);
        int textX = 240 - scoreText.Length * 4;
        List<DrawTask> scoreTasks = scoreText.CreateDrawTasks(new Vector2(textX, 150), textColor, LayerDepth.HUD);

        return new List<DrawTask> { gameOverText, restartPrompt }.Concat(scoreTasks).ToList();
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        Root.GameStateMachine.ChangeState(new GameplayState(Root));
    }

    public override void Enter()
    {
        _gameOverText = AssetManager.Load<Texture2D>("GameOver");
        _restartPrompt = AssetManager.Load<Texture2D>("Restart");
    }

    public override void Exit()
    {
        InputEventSource.KeyboardPressedEvent -= OnKeyboardPressedEvent;
    }
}