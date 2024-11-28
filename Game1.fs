module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System

type Game1 () as x =
    inherit Game()

    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable whiteTexture = Unchecked.defaultof<Texture2D>
    let mutable rect1Position = Vector2(100.0f, 100.0f)
    let mutable rect2Position = Vector2(100.0f, 100.0f)
    let mutable rect3Position = Vector2(100.0f, 100.0f)

    override x.Initialize() =
        whiteTexture <- new Texture2D(x.GraphicsDevice, 1, 1)
        whiteTexture.SetData([| Color.White |])

        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        base.Initialize()

    override this.LoadContent() =
        // If needed, load content here
        ()

    override this.Update(gameTime) =
        let keyboardState = Keyboard.GetState()

        let speed = 200.0f * float32 gameTime.ElapsedGameTime.TotalSeconds

        if keyboardState.IsKeyDown(Keys.Left) then
            rect1Position <- rect1Position + Vector2(-speed, 0.0f)
        if keyboardState.IsKeyDown(Keys.Right) then
            rect1Position <- rect1Position + Vector2(speed, 0.0f)
        if keyboardState.IsKeyDown(Keys.Up) then
            rect1Position <- rect1Position + Vector2(0.0f, -speed)
        if keyboardState.IsKeyDown(Keys.Down) then
            rect1Position <- rect1Position + Vector2(0.0f, speed)
        
        let rand = Random()

        //rect2Position <- rect2Position + Vector2((float32) rand.Next(0, 200), (float32) rand.NextDouble(0.0f, 200.0f))

        base.Update(gameTime)

    override this.Draw(gameTime) =
        x.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin()

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(int rect1Position.X, int rect1Position.Y, 150, 100),
            Color.Red
        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(300, 100, 150, 100),
            Color.Green 
        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(500, 100, 150, 100),
            Color.Blue 
        )

        spriteBatch.End()

        base.Draw(gameTime)
