module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System
open Maze

type MovementVector = { X:int; Y:int }

type Game1() as thisPacMan =
    inherit Game()

    do thisPacMan.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(thisPacMan)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable whiteTexture = Unchecked.defaultof<Texture2D>
    let mutable rect1Position = {X = 0; Y = 0}
    let mutable rect2Position = Vector2(100.0f, 200.0f)
    let mutable rect3Position = Vector2(100.0f, 100.0f)

    let environment = mazeMatrix
    let wallCode = Maze.WALL
    let speed = 20
    let playerSpeed = 1

    let isWall (position: MovementVector) =
        let xCeiling = int (Math.Ceiling((float position.X) / (float speed)))
        let yCeiling = int (Math.Ceiling((float position.Y) / (float speed)))
        let xFloor = int (Math.Floor((float position.X) / (float speed)))
        let yFloor = int (Math.Floor((float position.Y) / (float speed)))

        xCeiling < 0 || yCeiling < 0 || xFloor < 0 || yFloor < 0
        || yCeiling >= environment.Length || xCeiling >= environment.[yCeiling].Length 
        || yFloor >= environment.Length || xFloor >= environment.[yFloor].Length 
        || environment.[yCeiling].[xCeiling] = wallCode
        || environment.[yFloor].[xFloor] = wallCode
        || environment.[yFloor].[xCeiling] = wallCode
        || environment.[yCeiling].[xFloor] = wallCode

    let movePlayer () =
        let keyboardState = Keyboard.GetState()

        let goLeft = {rect1Position with X = rect1Position.X-playerSpeed}
        let goRight = {rect1Position with X = rect1Position.X+playerSpeed}
        let goUp = {rect1Position with Y = rect1Position.Y-playerSpeed}
        let goDown = {rect1Position with Y = rect1Position.Y+playerSpeed}

        rect1Position <-
            match keyboardState.GetPressedKeys() with
              | [||] -> rect1Position
              | pressedKeys ->
                  match Array.head pressedKeys with
                  | Keys.Left when not (isWall (goLeft)) -> goLeft
                  | Keys.Right when not (isWall (goRight)) -> goRight
                  | Keys.Up when not (isWall (goUp)) -> goUp
                  | Keys.Down when not (isWall (goDown)) -> goDown
                  | _ -> rect1Position

    let moveNPC (npcPosition) =
        let rand = Random()

        npcPosition
        + Vector2((float32) (rand.Next(-1, 2)), (float32) (rand.Next(-1, 2)))

    override thisPacMan.Initialize() =
        whiteTexture <- new Texture2D(thisPacMan.GraphicsDevice, 1, 1)
        whiteTexture.SetData([| Color.White |])

        spriteBatch <- new SpriteBatch(thisPacMan.GraphicsDevice)
        base.Initialize()

    override thisPacMan.LoadContent() =
        // If needed, load content here
        ()

    override thisPacMan.Update(gameTime) =
        movePlayer ()
        rect2Position <- (moveNPC rect2Position)
        rect3Position <- moveNPC rect3Position

        base.Update(gameTime)

    override thisPacMan.Draw(gameTime) =
        thisPacMan.GraphicsDevice.Clear Color.CornflowerBlue

        spriteBatch.Begin()

        let drawMaze = 
            for index_y in [0..mazeMatrix.Length-1] do
                for index_x in [0..mazeMatrix[index_y].Length-1] do
                    match mazeMatrix.[index_y].[index_x] with
                    | code when code = wallCode -> spriteBatch.Draw(
                            whiteTexture,
                            Rectangle(index_x * speed, index_y * speed, speed, speed),
                            Color.Black
                        )
                    | _ -> spriteBatch.Draw(
                            whiteTexture,
                            Rectangle(index_x * speed, index_y * speed, speed, speed),
                            Color.Gray
                        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(rect1Position.X, rect1Position.Y, speed, speed),
            Color.Yellow
        )

        spriteBatch.End()

        base.Draw(gameTime)
