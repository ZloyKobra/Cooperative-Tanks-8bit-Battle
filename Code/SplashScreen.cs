using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CoopTanks.Code
{
    static class SplashScreen
    {
        static public Texture2D Background {  get; set; }
        static int timeCounter = 0;
        static Color color;

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, color);
        }

        static public void Update()
        {
            color = Color.FromNonPremultiplied(255, 255, 255, (timeCounter < 256) ? timeCounter : 255);
            timeCounter++;
        }
    }
}
