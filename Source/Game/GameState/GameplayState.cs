using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameplayState : GameState
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public WaveController WaveController;
    
    public Player Player => (Player) Entities.Find(entity => entity is Player);

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        WaveController = new WaveController(this, Root);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        foreach (Entity entity in Entities) entity.Draw(spriteBatch);

        WaveController.Draw(spriteBatch);

        if (!Root.ShowDebug) return;
        
        foreach (Collider collider in CollisionSystem.Colliders)
        {
            int width = collider.Rectangle.Width;
            int height = collider.Rectangle.Height;
                
            Texture2D rect = new(Root.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];
                
            Array.Fill(data, new Color(Color.White, 0.2F));
            rect.SetData(data);

            Color color = collider.Parent.IsColliding ? Color.Red : Color.Blue;
            
            spriteBatch.Draw(rect, collider.Rectangle.Location.ToVector2(), color);
        }
    }

    public override void Enter()
    {
        Entities.Add(new Player(this, new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
    }

    public override void Exit()
    {
        while (Entities.Count > 0) Entities[0].Destroy();
    }
}