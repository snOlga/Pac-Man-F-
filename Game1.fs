module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System
open Maze

type MovementVector = { X:int; Y:int }

type Game1() as x =
    inherit Game()

    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable whiteTexture = Unchecked.defaultof<Texture2D>
    let mutable rect1Position = {X = 0; Y = 0}
    let mutable rect2Position = Vector2(100.0f, 200.0f)
    let mutable rect3Position = Vector2(100.0f, 100.0f)

    let environment = mazeMatrix
    let speed = 10

    let isWall (position: MovementVector) =
        let x = (position.X) / speed
        let y = (position.Y) / speed

        if x >= 0
           && y >= 0
           && y < environment.Length
           && x < environment.[y].Length
           && environment.[y].[x] <> 1 then
            // environment.[y].[x] = 1
            false
        else
            true

    let movePlayer () =
        let keyboardState = Keyboard.GetState()

        rect1Position <-
            match keyboardState.GetPressedKeys() with
              | [||] -> rect1Position
              | pressedKeys ->
                  match Array.head pressedKeys with
                  | Keys.Left 
                    when not (isWall ({rect1Position with X = rect1Position.X-speed})) 
                    -> {rect1Position with X = rect1Position.X-speed}
                  | Keys.Right 
                    when not (isWall ({rect1Position with X = rect1Position.X+speed})) 
                    -> {rect1Position with X = rect1Position.X+speed}
                  | Keys.Up 
                    when not (isWall ({rect1Position with Y = rect1Position.Y-speed})) 
                    -> {rect1Position with Y = rect1Position.Y-speed}
                  | Keys.Down 
                    when not (isWall ({rect1Position with Y = rect1Position.Y+speed})) 
                    -> {rect1Position with Y = rect1Position.Y+speed}
                  | _ -> rect1Position

    let moveNPC (npcPosition) =
        let rand = Random()

        npcPosition
        + Vector2((float32) (rand.Next(-1, 2)), (float32) (rand.Next(-1, 2)))

    override x.Initialize() =
        whiteTexture <- new Texture2D(x.GraphicsDevice, 1, 1)
        whiteTexture.SetData([| Color.White |])

        spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        base.Initialize()

    override this.LoadContent() =
        // If needed, load content here
        ()

    override this.Update(gameTime) =
        movePlayer ()
        rect2Position <- (moveNPC rect2Position)
        rect3Position <- moveNPC rect3Position

        ``base``.Update(gameTime)

    override this.Draw(gameTime) =
        x.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin()

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(rect1Position.X, rect1Position.Y, 150, 100),
            Color.Red
        )

        // spriteBatch.Draw(
        //     whiteTexture,
        //     Rectangle(int rect2Position.X, int rect2Position.Y, 150, 100),
        //     Color.Green 
        // )

        // spriteBatch.Draw(
        //     whiteTexture,
        //     Rectangle(int rect3Position.X, int rect3Position.Y, 150, 100),
        //     Color.Blue 
        // )

        spriteBatch.End()

        ``base``.Draw(gameTime)
