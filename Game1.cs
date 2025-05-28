using CoopTanks.Code;
using CoopTanks.Code.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.Extended;
using SharpDX.Direct2D1.Effects;
using MonoGame.Extended.Animations;

namespace CoopTanks;

enum State
{
    SplashScreen,
    Game,
    Final,
    Pause,
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player1;
    private Player player2;
    State state = State.SplashScreen;
    private int screenTailsHeight = 16;
    private int screenTailsWidth = 16;

    // Debug.WriteLine($"Player1 Position: {player1.position}"); Проверка значений вручную

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = 32 * screenTailsWidth;
        _graphics.PreferredBackBufferHeight = 32 * screenTailsHeight;

        _graphics.IsFullScreen = false;  // Оконный режим
        _graphics.ApplyChanges();
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        player1 = new Player(new Vector2(32, 32), new[] { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space });
        player2 = new Player(new Vector2(448, 32), new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightShift });
        Players.AddPlayer(player1);
        Players.AddPlayer(player2);
        Enemies.AddEnemy(new Enemy(new Vector2(128, 128), Players.players));
        Walls.CreateWalls();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SplashScreen.Background = Content.Load<Texture2D>("grass");
        Wall.Texture = Content.Load<Texture2D>("wall");

        player1.Texture = Content.Load<Texture2D>("tankHeroLeft");
        player1.TextureLeft = Content.Load<Texture2D>("tankHeroLeft");
        player1.TextureRight = Content.Load<Texture2D>("tankHeroRight");
        player1.TextureUp = Content.Load<Texture2D>("tankHeroUp");
        player1.TextureDown = Content.Load<Texture2D>("tankHeroDown");

        player2.Texture = Content.Load<Texture2D>("tankHero2Left");
        player2.TextureLeft = Content.Load<Texture2D>("tankHero2Left");
        player2.TextureRight = Content.Load<Texture2D>("tankHero2Right");
        player2.TextureUp = Content.Load<Texture2D>("tankHero2Up");
        player2.TextureDown = Content.Load<Texture2D>("tankHero2Down");

        Bullet.Texture = Content.Load<Texture2D>("bullet");
        foreach (var enemy in Enemies.enemies)
        {
            enemy.Texture = Content.Load<Texture2D>("enemyLeft");
            enemy.TextureLeft = Content.Load<Texture2D>("enemyLeft");
            enemy.TextureRight = Content.Load<Texture2D>("enemyRight");
            enemy.TextureUp = Content.Load<Texture2D>("enemyUp");
            enemy.TextureDown = Content.Load<Texture2D>("enemyDown");
        }

        // Отрисовка Хитбокса пули отдельно от текстуры
        Bullet.LoadDebugTexture(GraphicsDevice);
        //Enemy.LoadDebugTexture(GraphicsDevice);
        //Player.LoadDebugTexture(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        switch (state)
        {
            case State.SplashScreen:
                SplashScreen.Update();
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) state = State.Game;
                break;
            case State.Game:
                for (int i = Bullet.ActiveBullets.Count - 1; i >= 0; i--)
                {
                    Bullet.ActiveBullets[i].Update(gameTime);
                }
                foreach (var enemy in Enemies.enemies)
                {
                    enemy.Update(gameTime);
                }
                player1.Update(gameTime);
                player2.Update(gameTime);
                // Обновляем все активные пули
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) state = State.SplashScreen;
                break;
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        switch (state)
        {
            case State.SplashScreen:
                SplashScreen.Draw(_spriteBatch);
                break;
            case State.Game:
                // Отрисовка сетки
                for (int x = 0; x <= screenTailsWidth; x++)
                {
                    for (int y = 0; y <= screenTailsHeight; y++)
                    {
                        _spriteBatch.DrawRectangle(
                            new Rectangle(x * 32, y * 32, 32, 32),
                            Color.Gray * 0.2f,
                            1f);
                    }
                }
                /*
                // Отрисовка направления движения
                if (player1.Movement.IsMoving)
                {
                    Vector2 dir = player1.Movement.TargetPosition - player1.position;
                    dir.Normalize();
                    _spriteBatch.DrawLine(
                        player1.position + new Vector2(16, 16),
                        player1.position + new Vector2(16, 16) + dir * 20,
                        Color.Red,
                        2f);
                }
                if (player2.Movement.IsMoving)
                {
                    Vector2 dir = player2.Movement.TargetPosition - player2.position;
                    dir.Normalize();
                    _spriteBatch.DrawLine(
                        player2.position + new Vector2(16, 16),
                        player2.position + new Vector2(16, 16) + dir * 20,
                        Color.Red,
                        2f);
                }
                */
                player1.Draw(_spriteBatch);
                player2.Draw(_spriteBatch);
                foreach (var enemy in Enemies.enemies)
                {
                    enemy.Draw(_spriteBatch);
                }
                foreach (var wall in Walls.walls)
                {
                    wall.Draw(_spriteBatch);
                }
                foreach (var bullet in Bullet.ActiveBullets)
                {
                    bullet.Draw(_spriteBatch);
                }
                break;
        }
        base.Draw(gameTime);

        _spriteBatch.End();   
    }
}