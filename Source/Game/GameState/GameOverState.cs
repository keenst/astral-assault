#region
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheGameOfDoomHmmm.Source.Graphics;
using TheGameOfDoomHmmm.Source.Input;
#endregion

namespace TheGameOfDoomHmmm.Source.Game.GameState;

internal sealed class GameOverState : GameState, IKeyboardPressedEventListener
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

    internal override void Draw()
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

        m_gameOverText.DrawTexture2D(textPosition, 0f, LayerOrdering.Foreground);

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if ((timeNow - m_timeEntered) > 1000) m_restartPrompt.DrawTexture2D(promptPosition, 0, LayerOrdering.Foreground);

        int score = (int)Lerp(0, Root.Score, MathF.Min((timeNow - m_timeEntered) / 800F, 1));
        string scoreText = $"Score: {score}";
        int textX = (int)(textPosition.X - scoreText.Size().X / 2f);
        scoreText.Draw
            (new Vector2(textX, 150), Color.White, 0f, new Vector2(0, 0), 1f, LayerOrdering.Foreground);

        if (!m_newHighScore)
        {
            string highScoreText = $"High score: {Root.HighScore}";
            int highScoreX = (int)(textPosition.X - highScoreText.Size().X / 2f);
            highScoreText.Draw
                (new Vector2(highScoreX, 170), Color.White, 0f, new Vector2(0, 0), 1f, LayerOrdering.Foreground);

            return;
        }

        long timeSinceToggle = timeNow - m_lastToggle;

        if (timeSinceToggle > 500)
        {
            m_showNewHighScore = !m_showNewHighScore;
            m_lastToggle = timeNow;
        }

        if (!m_showNewHighScore) return;

        const string newHighScoreText = "New high score!";
        int newHighScoreX = (int)(textPosition.X - newHighScoreText.Size().X / 2f);
        newHighScoreText.Draw(new Vector2(newHighScoreX, 170), Color.White, 0f, new Vector2(0, 0), 1f, LayerOrdering.Foreground);
    }

    internal override void Enter()
    {
        m_gameOverText = AssetManager.Load<Texture2D>("GameOver");
        m_restartPrompt = AssetManager.Load<Texture2D>("Restart");
    }

    internal override void Exit()
    {
        InputEventSource.KeyboardPressedEvent -= OnKeyboardPressedEvent;
    }

    private static float Lerp(float firstFloat, float secondFloat, float by) => firstFloat * (1 - by) + secondFloat * by;

    public override void OnUpdate(object sender, UpdateEventArgs e) { }
}