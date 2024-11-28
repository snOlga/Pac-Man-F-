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

    let movePlayer () =
        let keyboardState = Keyboard.GetState()
        let speed = 1f
        rect1Position <- rect1Position + 
        match keyboardState.GetPressedKeys() with
        | [||] -> Vector2(0.0f, 0.0f)
        | pressedKeys ->
            match Array.head pressedKeys with
            | Keys.Left -> Vector2(-speed, 0.0f)
            | Keys.Right -> Vector2(speed, 0.0f)
            | Keys.Up -> Vector2(0.0f, -speed)
            | Keys.Down -> Vector2(0.0f, speed)
            | _ -> Vector2(0.0f, 0.0f)
    
    let moveNPC (npcPosition) =
        let rand = Random()
        npcPosition + Vector2((float32) (rand.Next(-1, 2)), (float32) (rand.Next(-1, 2)))

    override x.Initialize() =
        whiteTexture <- new Texture2D(x.GraphicsDevice, 1, 1)
        whiteTexture.SetData([| Color.White |])

        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        base.Initialize()

    override this.LoadContent() =
        // If needed, load content here
        ()

    override this.Update(gameTime) =
        movePlayer()
        rect2Position <- (moveNPC rect2Position)
        rect3Position <- moveNPC rect3Position

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
            Rectangle(int rect2Position.X, int rect2Position.Y, 150, 100),
            Color.Green 
        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(int rect3Position.X, int rect3Position.Y, 150, 100),
            Color.Blue 
        )

        spriteBatch.End()

        base.Draw(gameTime)
