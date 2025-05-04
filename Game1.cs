using CoopTanks.Code;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    private Player player;
    State state = State.SplashScreen;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        player = new Player(new Vector2(32f, 32f));
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SplashScreen.Background = Content.Load<Texture2D>("grass");
        player.Texture = Content.Load<Texture2D>("tankHero (1)");
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
                player.Update(gameTime);
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
                player.Draw(_spriteBatch);
                break;
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}