using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameOverState : GameState, IKeyboardPressedEventListener
{
    private Texture2D m_gameOverText;
    private Texture2D m_restartPrompt;

    public GameOverState(Game1 root) : base(root)
    {
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
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

        DrawTask gameOverText = new DrawTask
            (m_gameOverText, textPosition, 0, LayerDepth.HUD, new List<IDrawTaskEffect>());

        DrawTask restartPrompt = new DrawTask
            (m_restartPrompt, promptPosition, 0, LayerDepth.HUD, new List<IDrawTaskEffect>());

        return new List<DrawTask> { gameOverText, restartPrompt };
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        Root.GameStateMachine.ChangeState(new GameplayState(Root));
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

    public override void OnUpdate(object sender, UpdateEventArgs e) { }
}