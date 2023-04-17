using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class GameOverState : GameState, IKeyboardPressedEventListener
{
    private Texture2D _gameOverText;
    private Texture2D _restartPrompt;
    private long _timeEntered;
    
    public GameOverState(Game1 root) : base(root)
    {
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
        _timeEntered = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    
    public override List<DrawTask> GetDrawTasks()
    {
        Vector2 textPosition = new(
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 3D));
        
        Vector2 promptPosition = new(
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 2D));

        List<DrawTask> drawTasks = new();
        
        DrawTask gameOverText = new(
            _gameOverText, 
            textPosition, 
            0, 
            LayerDepth.HUD, 
            new List<IDrawTaskEffect>());

        drawTasks.Add(gameOverText);
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (timeNow - _timeEntered > 1000)
        {
            DrawTask restartPrompt = new(
                _restartPrompt,
                promptPosition,
                0,
                LayerDepth.HUD,
                new List<IDrawTaskEffect>());
            
            drawTasks.Add(restartPrompt);
        }

        string scoreText = $"Score: {Root.Score}";
        Color textColor = Palette.GetColor(Palette.Colors.Grey9);
        int textX = 240 - scoreText.Length * 4;
        List<DrawTask> scoreTasks = scoreText.CreateDrawTasks(new Vector2(textX, 150), textColor, LayerDepth.HUD);
        drawTasks.AddRange(scoreTasks);

        return drawTasks;
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        if (e.Keys.Contains(Keys.F)) return;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (timeNow - _timeEntered < 300)
        {
            return;
        }
        
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