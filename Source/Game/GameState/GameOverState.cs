using System;
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
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 textPosition = new(
            (float)Math.Round((Game1.TargetWidth  - _gameOverText.Width)  / 2D),
            (float)Math.Round((Game1.TargetHeight - _gameOverText.Height) / 3D));
        
        Vector2 promptPosition = new(
            (float)Math.Round((Game1.TargetWidth  - _restartPrompt.Width)  / 2D),
            (float)Math.Round((Game1.TargetHeight - _restartPrompt.Height) / 2D));

        spriteBatch.Draw(_gameOverText, textPosition, Color.White);
        spriteBatch.Draw(_restartPrompt, promptPosition, Color.White);
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        Root.GameStateMachine.ChangeState(new GameplayState(Root));
    }

    public override void Enter()
    {
        _gameOverText = AssetManager.LoadTexture("game over");
        _restartPrompt = AssetManager.LoadTexture("restart");
    }

    public override void Exit()
    {
        InputEventSource.KeyboardPressedEvent -= OnKeyboardPressedEvent;
    }
}