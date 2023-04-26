using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class GameOverState : GameState, IKeyboardPressedEventListener
{
    private readonly bool _newHighScore;
    private readonly long _timeEntered;
    private Texture2D _gameOverText;
    private Texture2D _restartPrompt;
    private bool _showNewHighScore;
    private long _lastToggle;
    
    public GameOverState(Game1 root) : base(root)
    {
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
        _timeEntered = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (Root.Score <= Root.HighScore) return;
        
        Root.HighScore = Root.Score;
        _newHighScore = true;
    }
    
    public override List<DrawTask> GetDrawTasks()
    {
        Vector2 textPosition = new Vector2
        (
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 3D)
        );

        Vector2 promptPosition = new Vector2
        (
            (float)Math.Round(Game1.TargetWidth / 2D),
            (float)Math.Round(Game1.TargetHeight / 2D)
        );

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

        int score = (int)Lerp(0, Root.Score, MathF.Min((timeNow - _timeEntered) / 800F, 1));
        string scoreText = $"Score: {score}";
        int textX = 240 - $"Score: {Root.Score}".Length * 4;
        List<DrawTask> scoreTasks = scoreText.CreateDrawTasks(new Vector2(textX, 150), Color.White, LayerDepth.HUD);
        drawTasks.AddRange(scoreTasks);

        if (!_newHighScore)
        {
            string highScoreText = $"High score: {Root.HighScore}";
            int highScoreX = 240 - highScoreText.Length * 4;
            List<DrawTask> highScoreTasks = 
                highScoreText.CreateDrawTasks(new Vector2(highScoreX, 170), Color.White, LayerDepth.HUD);
            drawTasks.AddRange(highScoreTasks);
            
            return drawTasks;
        }

        long timeSinceToggle = timeNow - _lastToggle;
        if (timeSinceToggle > 500)
        {
            _showNewHighScore = !_showNewHighScore;
            _lastToggle = timeNow;
        }
        
        if (!_showNewHighScore) return drawTasks;

        string newHighScoreText = "New high score!";
        int newHighScoreX = 240 - newHighScoreText.Length * 4;
        List<DrawTask> newHighScoreTasks = 
            newHighScoreText.CreateDrawTasks(new Vector2(newHighScoreX, 170), Color.White, LayerDepth.HUD);
        drawTasks.AddRange(newHighScoreTasks);

        return drawTasks;
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        if (e.Keys.Contains(Keys.F)) return;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (timeNow - _timeEntered < 1000)
        {
            return;
        }
        
        Jukebox.PlaySound("RestartGame");
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
    
    private float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }
    
    public override void OnUpdate(object sender, UpdateEventArgs e) { }
}