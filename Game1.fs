module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System
open Maze

type Direction =
    | Left
    | Right
    | Up
    | Down
    | None

type MovementVector = { X:int; Y:int; DirectionType:Direction }

type Game1() as thisPacMan =
    inherit Game()

    do thisPacMan.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(thisPacMan)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable whiteTexture = Unchecked.defaultof<Texture2D>
    let mutable playerPos = {X = 0; Y = 0; DirectionType = Direction.None}
    let mutable npc1Pos = {X = 200; Y = 120; DirectionType = Direction.None}
    let mutable npc2Pos = {X = 200; Y = 200; DirectionType = Direction.None}
    let mutable score = 0

    let environment = mazeMatrix
    let wallCode = Maze.WALL
    let blockSize = 20
    let speed = 1

    let isWall (position: MovementVector) =
        let xCeiling = int (Math.Ceiling((float position.X) / (float blockSize)))
        let yCeiling = int (Math.Ceiling((float position.Y) / (float blockSize)))
        let xFloor = int (Math.Floor((float position.X) / (float blockSize)))
        let yFloor = int (Math.Floor((float position.Y) / (float blockSize)))

        xCeiling < 0 || yCeiling < 0 || xFloor < 0 || yFloor < 0
        || yCeiling >= environment.Length || xCeiling >= environment.[yCeiling].Length 
        || yFloor >= environment.Length || xFloor >= environment.[yFloor].Length 
        || environment.[yCeiling].[xCeiling] = wallCode
        || environment.[yFloor].[xFloor] = wallCode
        || environment.[yFloor].[xCeiling] = wallCode
        || environment.[yCeiling].[xFloor] = wallCode
    
    let moveSomeone positionBefore direction =
        let goLeft = {positionBefore with X = positionBefore.X-speed; DirectionType=Direction.Left}
        let goRight = {positionBefore with X = positionBefore.X+speed; DirectionType=Direction.Right}
        let goUp = {positionBefore with Y = positionBefore.Y-speed; DirectionType=Direction.Up}
        let goDown = {positionBefore with Y = positionBefore.Y+speed; DirectionType=Direction.Down}

        match direction with
            | Direction.Left when not (isWall (goLeft)) -> goLeft
            | Direction.Right when not (isWall (goRight)) -> goRight
            | Direction.Up when not (isWall (goUp)) -> goUp
            | Direction.Down when not (isWall (goDown)) -> goDown
            | _ -> { positionBefore with DirectionType=Direction.None }
    
    let eatApple (playerNewPos) =
        let xCeiling = int (Math.Ceiling((float playerNewPos.X) / (float blockSize)))
        let yCeiling = int (Math.Ceiling((float playerNewPos.Y) / (float blockSize)))
        let xFloor = int (Math.Floor((float playerNewPos.X) / (float blockSize)))
        let yFloor = int (Math.Floor((float playerNewPos.Y) / (float blockSize)))

        score <- score +
            match environment with
                | point when environment[yCeiling][xCeiling] = Maze.APPL -> 
                    environment[yCeiling][xCeiling] <- Maze.EMPT
                    Maze.APPL
                | point when environment[yCeiling][xFloor] = Maze.APPL -> 
                    environment[yCeiling][xFloor] <- Maze.EMPT
                    Maze.APPL
                | point when environment[yFloor][xFloor] = Maze.APPL -> 
                    environment[yFloor][xFloor] <- Maze.EMPT
                    Maze.APPL
                | point when environment[yFloor][xCeiling] = Maze.APPL -> 
                    environment[yFloor][xCeiling] <- Maze.EMPT
                    Maze.APPL
                | _ -> Maze.EMPT

    let movePlayer () =
        let keyboardState = Keyboard.GetState()

        playerPos <-
            match keyboardState.GetPressedKeys() with
              | [||] -> playerPos
              | pressedKeys ->
                  eatApple playerPos
                  match Array.head pressedKeys with
                  | Keys.Left -> moveSomeone playerPos Direction.Left
                  | Keys.Right -> moveSomeone playerPos Direction.Right
                  | Keys.Up -> moveSomeone playerPos Direction.Up
                  | Keys.Down -> moveSomeone playerPos Direction.Down
                  | _ -> playerPos

    let moveNPC (npcPosition) =
        let rand = Random()
        let moveDirect = rand.Next(4)

        match npcPosition.DirectionType with
          | Direction.Left -> moveSomeone npcPosition Direction.Left
          | Direction.Right -> moveSomeone npcPosition Direction.Right
          | Direction.Up -> moveSomeone npcPosition Direction.Up
          | Direction.Down -> moveSomeone npcPosition Direction.Down
          | _ -> { npcPosition with DirectionType = 
                                    match moveDirect with 
                                    | 0 -> Direction.Left 
                                    | 1 -> Direction.Right 
                                    | 2 -> Direction.Up 
                                    | 3 -> Direction.Down
                                    | _ -> Direction.None }

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
        npc1Pos <- moveNPC npc1Pos
        npc2Pos <- moveNPC npc2Pos

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
                            Rectangle(index_x * blockSize, index_y * blockSize, blockSize, blockSize),
                            Color.Black
                        )
                    | code when code = Maze.APPL -> spriteBatch.Draw(
                            whiteTexture,
                            Rectangle(index_x * blockSize, index_y * blockSize, blockSize, blockSize),
                            Color.Green
                        )
                    | _ -> spriteBatch.Draw(
                            whiteTexture,
                            Rectangle(index_x * blockSize, index_y * blockSize, blockSize, blockSize),
                            Color.Gray
                        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(playerPos.X, playerPos.Y, blockSize, blockSize),
            Color.Yellow
        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(npc1Pos.X, npc1Pos.Y, blockSize, blockSize),
            Color.Red
        )

        spriteBatch.Draw(
            whiteTexture,
            Rectangle(npc2Pos.X, npc2Pos.Y, blockSize, blockSize),
            Color.Bisque
        )

        spriteBatch.End()

        base.Draw(gameTime)
