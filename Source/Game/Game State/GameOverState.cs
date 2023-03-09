using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class GameOverState : GameState, IKeyboardEventListener, IUpdateEventListener
{
    private Texture2D _gameOverText;
    private Texture2D _restartPrompt;
    private bool _restartPromptVisible;
    private readonly long _enterTime;
    
    private const int RestartPromptDelayMS = 500;
    
    public GameOverState(Game1 root) : base(root)
    {
        _enterTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        InputEventSource.KeyboardEvent += OnKeyboardEvent;
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 textPosition = new(
            (float)Math.Round((Game1.TargetWidth  - _gameOverText.Width)  / 2D),
            (float)Math.Round((Game1.TargetHeight - _gameOverText.Height) / 3D));

        spriteBatch.Draw(_gameOverText, textPosition, Color.White);

        if (!_restartPromptVisible) return;
        
        Vector2 promptPosition = new(
            (float)Math.Round((Game1.TargetWidth  - _restartPrompt.Width)  / 2D),
            (float)Math.Round((Game1.TargetHeight - _restartPrompt.Height) / 2D));
        
        spriteBatch.Draw(_restartPrompt, promptPosition, Color.White);
    }

    public void OnKeyboardEvent(object sender, KeyboardEventArgs e)
    {
        if (!_restartPromptVisible) return;
        
        Root.GameStateMachine.ChangeState(new GameplayState(Root));
    }
    
    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (_restartPromptVisible) return;
        
        if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - _enterTime > RestartPromptDelayMS)
        {
            _restartPromptVisible = true;
        }
    }

    public override void Enter()
    {
        _gameOverText = AssetManager.LoadTexture("game over");
        _restartPrompt = AssetManager.LoadTexture("restart");
    }

    public override void Exit()
    {
        InputEventSource.KeyboardEvent -= OnKeyboardEvent;
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }
}