#region
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class GameOverState : GameState, IKeyboardPressedEventListener
{
    private readonly bool m_newHighScore;
    private readonly long m_timeEntered;
    private Texture2D m_gameOverText;
    private long m_lastToggle;
    private Texture2D m_restartPrompt;
    private bool m_showNewHighScore;

    public GameOverState(Game1 root) : base(root)
    {
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
        m_timeEntered = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (Root.Score <= Root.HighScore) return;

        Root.HighScore = Root.Score;
        m_newHighScore = true;
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        if (e.Keys.Contains(Keys.F)) return;

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((timeNow - m_timeEntered) < 1000) return;

        Jukebox.PlaySound("RestartGame");
        Root.GameStateMachine.ChangeState(new GameplayState(Root));
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

        List<DrawTask> drawTasks = new List<DrawTask>();

        DrawTask gameOverText = new DrawTask
            (m_gameOverText, textPosition, 0, LayerDepth.HUD);

        drawTasks.Add(gameOverText);

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((timeNow - m_timeEntered) > 1000)
        {
            DrawTask restartPrompt = new DrawTask
                (m_restartPrompt, promptPosition, 0, LayerDepth.HUD);

            drawTasks.Add(restartPrompt);
        }

        int score = (int)Lerp(0, Root.Score, MathF.Min((timeNow - m_timeEntered) / 800F, 1));
        string scoreText = $"Score: {score}";
        int textX = 240 - $"Score: {Root.Score}".Length * 4;
        ReadOnlySpan<DrawTask> scoreTasks = scoreText.AsSpan().CreateDrawTasks
            (new Vector2(textX, 150), Color.White, LayerDepth.HUD, false);
        drawTasks.AddRange(scoreTasks.ToArray());

        if (!m_newHighScore)
        {
            string highScoreText = $"High score: {Root.HighScore}";
            int highScoreX = 240 - highScoreText.Length * 4;
            ReadOnlySpan<DrawTask> highScoreTasks =
                highScoreText.AsSpan().CreateDrawTasks(new Vector2(highScoreX, 170), Color.White, LayerDepth.HUD, false);
            drawTasks.AddRange(highScoreTasks.ToArray());

            return drawTasks;
        }

        long timeSinceToggle = timeNow - m_lastToggle;

        if (timeSinceToggle > 500)
        {
            m_showNewHighScore = !m_showNewHighScore;
            m_lastToggle = timeNow;
        }

        if (!m_showNewHighScore) return drawTasks;

        string newHighScoreText = "New high score!";
        int newHighScoreX = 240 - newHighScoreText.Length * 4;
        ReadOnlySpan<DrawTask> newHighScoreTasks =
            newHighScoreText.AsSpan().CreateDrawTasks(new Vector2(newHighScoreX, 170), Color.White, LayerDepth.HUD, true);
        drawTasks.AddRange(newHighScoreTasks.ToArray());

        return drawTasks;
    }

    public override void Enter()
    {
        m_gameOverText = AssetManager.Load<Texture2D>("GameOver");
        m_restartPrompt = AssetManager.Load<Texture2D>("Restart");
    }

    public override void Exit()
    {
        InputEventSource.KeyboardPressedEvent -= OnKeyboardPressedEvent;
    }

    private float Lerp(float firstFloat, float secondFloat, float by) => firstFloat * (1 - by) + secondFloat * by;

    public override void OnUpdate(object sender, UpdateEventArgs e) { }
}