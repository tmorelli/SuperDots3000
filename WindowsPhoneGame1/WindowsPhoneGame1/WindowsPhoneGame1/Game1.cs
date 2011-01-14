using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace WindowsPhoneGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 backLocation;
        Texture2D backSprite;
        Texture2D dotSprite;
        Texture2D lineSprite;
        Texture2D vlineSprite;

        Texture2D player1MarkerSprite;
        Texture2D player2MarkerSprite;

       

        SoundEffect sfx;
        SoundEffectInstance sndBackground;

        SoundEffect sfxDing;
        SoundEffectInstance sndDing;

        SoundEffect sfxComplete;
        SoundEffectInstance sndComplete;

        SoundEffect sfxClunk;
        SoundEffectInstance sndClunk;

        SpriteFont scoreFont;
        SpriteFont labelFont;



        int SQUARE_VALUE = 100;

        int X_GRID = 9;
        int Y_GRID = 7;
        int GRID_SPACING = 65;
        int Y_GRID_START = 30;
        int X_GRID_START = 40;
        int DOT_CENTER_OFFSET = 8;

        int ODDS_EXTRA_LINE_BONUS = 2;
        int ODDS_RANDOM_LINE_BONUS = 2;

        int HORIZONTAL_LINES;
        int VERTICAL_LINES;
        bool [,] horizLines;
        bool [,] vertLines;

        int playerTurn = 0;
        int player1Score = 0;
        int player2Score = 0;
    
        bool [,] player1Markers;
        bool [,] player2Markers;

 
        Point touchDownPoint = new Point();
        Point touchUpPoint = new Point();

        /******
        * Extra Line Bonus
        * 
        */
        bool playingExtraLineBonusAnimation = false;
        Point extraLineBonusLocation = new Point();
        int onTicks = -1;
        int offTicks = -1;
        Texture2D extraLineBonusSprite;

        int timeSinceLastTick =0;
        int extraLineBonusElapsedTime =0;



        Random r = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            backLocation = new Vector2(0, 0);

            HORIZONTAL_LINES = (X_GRID - 1) * Y_GRID;
            VERTICAL_LINES = (Y_GRID - 1) * X_GRID;

            horizLines = new bool [X_GRID+1,Y_GRID+1];
            vertLines = new bool [X_GRID+1,Y_GRID+1];
            player1Markers = new bool[X_GRID - 1, Y_GRID - 1];
            player2Markers = new bool[X_GRID - 1, Y_GRID - 1];

            InitLines();
            InitTouchPoints();
            InitPlayerMarkers();

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backSprite = this.Content.Load<Texture2D>("screen");
            dotSprite = this.Content.Load<Texture2D>("dot");
            lineSprite = this.Content.Load<Texture2D>("line");
            vlineSprite = this.Content.Load<Texture2D>("line_v");
            player1MarkerSprite = this.Content.Load<Texture2D>("blue_marker");
            player2MarkerSprite = this.Content.Load<Texture2D>("green_marker");

            extraLineBonusSprite = this.Content.Load<Texture2D>("extra_line_bonus");

            scoreFont = this.Content.Load<SpriteFont>("SpriteFont1");
            labelFont = this.Content.Load<SpriteFont>("SpriteFont2");



            sfx = this.Content.Load<SoundEffect>("background");
            sndBackground = sfx.CreateInstance();
            sndBackground.IsLooped = true;
            sndBackground.Play();

            sfxClunk = this.Content.Load<SoundEffect>("clunk");
            sndClunk = sfxClunk.CreateInstance();
            sndClunk.IsLooped = false;

            sfxComplete = this.Content.Load<SoundEffect>("complete1");
            sndComplete = sfxComplete.CreateInstance();
            sndComplete.IsLooped = false;

            sfxDing = this.Content.Load<SoundEffect>("ding");
            sndDing = sfxDing.CreateInstance();
            sndDing.IsLooped = false;


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        private void InitLines()
        {
            //	  for (int xx = 0; xx< X_GRID-1; xx++)
            for (int xx = 0; xx < X_GRID + 1; xx++)
            {
                //		  for (int yy = 0; yy< Y_GRID; yy++)
                for (int yy = 0; yy < Y_GRID + 1; yy++)
                {
                    horizLines[xx,yy] = new bool();
                    horizLines[xx,yy] = false;
                    
                }
            }

            //	  for (int xx = 0; xx< X_GRID; xx++)
            for (int xx = 0; xx < X_GRID + 1; xx++)
            {
                //		  for (int yy = 0; yy< Y_GRID-1; yy++)
                for (int yy = 0; yy < Y_GRID + 1; yy++)
                {
                    vertLines[xx,yy] = new bool();
                    vertLines[xx,yy] = false;
                }
            }
        }

        private void DrawScore()
        {
            String text = "SCORE";
            spriteBatch.DrawString(scoreFont, text, new Vector2(640,40), Color.White);

            text = "Player 1:  " + player1Score.ToString ();
            spriteBatch.DrawString(labelFont, text, new Vector2(640,100), Color.White);

            text = "Player 2:  " + player2Score.ToString();
            spriteBatch.DrawString(labelFont, text, new Vector2(640, 130), Color.White);


            text = "Next Move:";
            spriteBatch.DrawString(labelFont, text, new Vector2(640, 330), Color.White);

            if (playerTurn == 0)
                text = "Player 1 (Blue)";
            else
                text = "Player 2 (Green)";
            spriteBatch.DrawString(labelFont, text, new Vector2(640, 360), Color.White);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (playingExtraLineBonusAnimation)
                TickExtraLineBonusAnimation(gameTime);


            TouchCollection tc = TouchPanel.GetState();
            if (tc.Count >0)
            {
                foreach (TouchLocation tl in tc)
                {
                    if (tl.State == TouchLocationState.Released)
                    {
                        touchUpPoint.X = (int)tl.Position.X;
                        touchUpPoint.Y = (int)tl.Position.Y;
                        ProcessTouchUp();

                        Console.WriteLine("UP!!");
                    }
                    if (tl.State == TouchLocationState.Pressed)
                    {
                        touchDownPoint.X = (int)tl.Position.X;
                        touchDownPoint.Y = (int)tl.Position.Y;

                        Console.WriteLine("UP!!");
                    }


                }

            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void ProcessTouchUp()
        {
            //Did we go vertical or horizontal, 
            //or not enough for either

            bool turnComplete = false;
            /*
            if (rlb.GetState() == RandomLineBonus.SHOW_MENU)
            {
                int btnPressed = rlb.PointPressed(touchUpPoint);
                if (btnPressed == RandomLineBonus.NO)
                {
                    if (playerTurn == 0)
                        playerTurn = 1;
                    else
                        playerTurn = 0;
                    invalidate();

                }
                if (btnPressed == RandomLineBonus.NEITHER)
                {
                    return;
                }
                if (btnPressed == RandomLineBonus.YES)
                {
                    //In here the player wants to use the extra line
                    //Deduct points...
                    //Choose a random line out of the 4
                    //Draw that one and evaluate...
                    PlaceRandomLineBonus();
                }

            }
             */
            //Check for horizontal swipe within bounds
            if (Math.Abs(touchDownPoint.X - touchUpPoint.X) > 50 &&
                    Math.Abs(touchDownPoint.X - touchUpPoint.X) < 75)
            {
                if (Math.Abs(touchDownPoint.Y - touchUpPoint.Y) > 50)
                {
                    InitTouchPoints();
                    return;
                }
                //Need to find the closest place to start drawing...
                int y = Y_GRID_START;
                int x = X_GRID_START;

                for (int xx = 0; xx < X_GRID; xx++)
                {
                    for (int yy = 0; yy < Y_GRID; yy++)
                    {
                        if ((Math.Abs(touchUpPoint.Y - (y + DOT_CENTER_OFFSET)) < 20) &&
                            (Math.Abs(touchUpPoint.X - (x + DOT_CENTER_OFFSET)) < 20))
                        {
                            if (touchDownPoint.X > touchUpPoint.X)
                            {
                                if (horizLines[xx,yy] == false)
                                {
                                    turnComplete = true;
                                    horizLines[xx,yy] = true;
                                    sndClunk.Play();
                                    UpdateHorizMarkers(xx, yy);
                                }
                            }
                            else if (xx > 0)
                            {
                                if (horizLines[xx - 1,yy] == false)
                                {
                                    turnComplete = true;
                                    horizLines[xx - 1,yy] = true;
                                    sndClunk.Play();
                                    UpdateHorizMarkers(xx - 1, yy);
                                }
                            }
                        }
                        y += GRID_SPACING;
                    }
                    y = Y_GRID_START;
                    x += GRID_SPACING;
                }
            }
            //check for vertical swipe within bounds

            else if (Math.Abs(touchDownPoint.Y - touchUpPoint.Y) > 50 &&
                    Math.Abs(touchDownPoint.Y - touchUpPoint.Y) < 75)
            {
                if (Math.Abs(touchDownPoint.X - touchUpPoint.X) > 50)
                {
                    InitTouchPoints();
                    return;
                }
                int y = Y_GRID_START;
                int x = X_GRID_START;

                for (int xx = 0; xx < X_GRID; xx++)
                {
                    for (int yy = 0; yy < Y_GRID; yy++)
                    {
                        if ((Math.Abs(touchUpPoint.Y - (y + DOT_CENTER_OFFSET)) < 20) &&
                            (Math.Abs(touchUpPoint.X - (x + DOT_CENTER_OFFSET)) < 20))
                        {
                            if (touchDownPoint.Y > touchUpPoint.Y)
                            {
                                if (vertLines[xx,yy] == false)
                                {
                                    turnComplete = true;
                                    vertLines[xx,yy] = true;
                                    sndClunk.Play();
                                    UpdateVertMarkers(xx, yy);
                                }
                            }
                            else if (yy > 0)
                            {
                                if (vertLines[xx,yy - 1] == false)
                                {
                                    turnComplete = true;
                                    vertLines[xx,yy - 1] = true;
                                    sndClunk.Play();
                                    UpdateVertMarkers(xx, yy - 1);
                                }
                            }
                        }
                        y += GRID_SPACING;
                    }
                    y = Y_GRID_START;
                    x += GRID_SPACING;
                }

            }
            if (turnComplete == true)
            {
                
                if (r.Next(ODDS_EXTRA_LINE_BONUS) == 0)
                {
                    playingExtraLineBonusAnimation = true;
                    extraLineBonusLocation.X = 150;
                    extraLineBonusLocation.Y = 150;
                    onTicks = 0;
                    offTicks = -1;
                    timeSinceLastTick = 0;
                    extraLineBonusElapsedTime = 0;


                    //Play Bonus Animation...
                    //Do not switch players..

                }
                /*
                else if (r.nextInt(ODDS_RANDOM_LINE_BONUS) == 0)
                {

                    playingRandomLineBonus = true;
                    rlb.SetStateShowMenu();

                    onTicks = 0;
                    offTicks = -1;
                    //Now we need to choose 4 different lines
                    //That are not already drawn
                    //Need array of random vert lines
                    //and random horiz lines
                    //Init all lines in all arrays to -1 to signify the end
                    //Then step through and set the points to random array indexes


                    for (int x = 0; x < RANDOM_LINE_COUNT; x++)
                    {
                        randomVertLines[x].x = -1;
                        randomVertLines[x].y = -1;
                        randomHorizLines[x].x = -1;
                        randomHorizLines[x].y = -1;
                    }

                    int totalLinesFound = 0;
                    int totalTries = 0;
                    int totalHorizFound = 0;
                    int totalVertFound = 0;
                    while ((totalLinesFound < RANDOM_LINE_COUNT) && (totalTries < 1000))
                    {
                        totalTries++;

                        int dir = r.nextInt(2);
                        if (dir == 0)
                        {
                            int x = r.nextInt(X_GRID - 1);
                            int y = r.nextInt(Y_GRID - 1);
                            if (horizLines[x][y] == false)
                            {
                                //Check to make sure we didnt already choose this line..
                                boolean dup = false;
                                for (int j = 0; j < totalHorizFound; j++)
                                {
                                    if (randomHorizLines[j].x == x && randomHorizLines[j].y == y)
                                    {
                                        dup = true;
                                    }
                                }
                                if (dup == false)
                                {
                                    randomHorizLines[totalHorizFound].x = x;
                                    randomHorizLines[totalHorizFound].y = y;
                                    totalHorizFound++;
                                    totalLinesFound++;
                                }
                            }
                        }
                        else
                        {
                            int x = r.nextInt(X_GRID - 1);
                            int y = r.nextInt(Y_GRID - 1);
                            if (vertLines[x][y] == false)
                            {
                                //Check to make sure we didnt already choose this line..
                                boolean dup = false;
                                for (int j = 0; j < totalVertFound; j++)
                                {
                                    if (randomVertLines[j].x == x && randomVertLines[j].y == y)
                                    {
                                        dup = true;
                                    }
                                }
                                if (dup == false)
                                {
                                    randomVertLines[totalVertFound].x = x;
                                    randomVertLines[totalVertFound].y = y;
                                    totalVertFound++;
                                    totalLinesFound++;
                                }
                            }
                        }
                    }
                    counter = new MyCount(10000, 250);
                    counter.start();
                }*/
                else
                {
                    if (playerTurn == 0)
                        playerTurn = 1;
                    else
                        playerTurn = 0;
                }
                 

                InitTouchPoints();
            }
        }

        private void EndExtraLineBonusAnimation()
        {
            playingExtraLineBonusAnimation = false;
            extraLineBonusLocation.X = -300;
            extraLineBonusLocation.Y = -300;
            timeSinceLastTick = 0;
            extraLineBonusElapsedTime = 0;
            onTicks = -1;
            offTicks = -1;
        }
        private void TickExtraLineBonusAnimation(GameTime _gt)
        {
            timeSinceLastTick += _gt.ElapsedGameTime.Milliseconds;
            extraLineBonusElapsedTime += _gt.ElapsedGameTime.Milliseconds;

            if (timeSinceLastTick >= 250)
            {
                timeSinceLastTick = 0;
            }
            else
            {
                return;
            }
            if (extraLineBonusElapsedTime >= 4000)
            {
                timeSinceLastTick =0;
                extraLineBonusElapsedTime =0;

                EndExtraLineBonusAnimation();
            }

            if (onTicks >= 0)
            {
                onTicks++;
                if (onTicks >= 3)
                {
                    onTicks = -1;
                    offTicks = 0;
                }
            }
            else if (offTicks >= 0)
            {
                offTicks++;
                if (offTicks >= 1)
                {
                    offTicks = -1;
                    onTicks = 0;
                }
            }
        }
        private void InitTouchPoints()
        {
            touchDownPoint.X = -1;
            touchDownPoint.Y = -1;
            touchUpPoint.X = -1;
            touchUpPoint.Y = -1;
        }
        private void InitPlayerMarkers()
        {
            for (int x = 0; x < X_GRID - 1; x++)
            {
                for (int y = 0; y < Y_GRID - 1; y++)
                {
                    player1Markers[x,y] = false;
                    player2Markers[x,y] = false;
                }
            }
        }
 
private void UpdateHorizMarkers(int _x, int _y)
  { 
	  //Now we need to go through all attached
	  //and see if we have 4 adjacent
	  
	  int matchesFound = 0;
	  if (_y >0 && horizLines[_x,_y-1] == true)
	  {
		  if (vertLines[_x,_y-1]==true && vertLines[_x+1,_y-1] == true)
		  {
			  if (playerTurn == 0)
			  {
				  matchesFound ++;
				  player1Markers[_x,_y-1] = true;
			  }
			  else
			  {
				  matchesFound ++;
				  player2Markers[_x,_y-1] = true;				  
			  }
		  }
	  }
	  if (_y < Y_GRID-1 && horizLines[_x,_y+1] == true)
	  {
		  if (vertLines[_x,_y]==true && vertLines[_x+1,_y] == true)
		  {
			  if (playerTurn == 0)
			  {
				  matchesFound ++;
				  player1Markers[_x,_y] = true;
			  }
			  else
			  {
				  matchesFound ++;
				  player2Markers[_x,_y] = true;				  
			  }
		  }
	  }
	  if (playerTurn == 0)
	  {
		  if (matchesFound == 1)
		  {
              sndComplete.Play();
		    player1Score += matchesFound*SQUARE_VALUE ;
            CheckGameCompleted();
          }
		  else if (matchesFound >1)
		  {
              sndComplete.Play();
              player1Score += 2 * matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          }
	  }
	  if (playerTurn == 1)
	  {
		  if (matchesFound == 1)
		  {
              sndComplete.Play();
              player2Score += matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          }
		  else if (matchesFound >1)
		  {
              sndComplete.Play();
              player2Score += 2 * matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          }
	  }

  }
  private void UpdateVertMarkers(int _x, int _y)
  {
	  int matchesFound = 0;
	  if (_x >0 && vertLines[_x-1,_y] == true)
	  {
		  if (horizLines[_x-1,_y]==true && horizLines[_x-1,_y+1] == true)
		  {
			  if (playerTurn == 0)
			  {
				  matchesFound++;
				  player1Markers[_x-1,_y] = true;
			  }
			  else
			  {
				  matchesFound++;
				  player2Markers[_x-1,_y] = true;				  
			  }
		  }
	  }
	  if (vertLines[_x+1,_y] == true)
	  {
		  if (horizLines[_x,_y]==true && horizLines[_x,_y+1] == true)
		  {
			  if (playerTurn == 0)
			  {
				  matchesFound++;
				  player1Markers[_x,_y] = true;
			  } 
			  else
			  {
				  matchesFound++;
				  player2Markers[_x,_y] = true;				  
			  }
		  }
	  }
	  if (playerTurn == 0)
	  {
		  if (matchesFound == 1)
		  {
              sndComplete.Play();
              player1Score += matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
		  }
		  else if (matchesFound >1)
		  {
              sndComplete.Play();
              player1Score += 2 * matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          }
	  }
	  if (playerTurn == 1)
	  {
		  if (matchesFound == 1)
		  {
              sndComplete.Play();
              player2Score += matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          }
		  else if (matchesFound >1)
		  {
              sndComplete.Play();
              player2Score += 2 * matchesFound*SQUARE_VALUE;
              CheckGameCompleted();
          } 
	  } 
  }
  private void CheckGameCompleted()
  {
      //Check inside here if there are no more squares left.
      //Set our state here

  }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backSprite, backLocation, Color.White);
            DrawGrid();
            DrawMarkers();
            DrawLines();
            DrawScore();
            if (playingExtraLineBonusAnimation && onTicks >= 0)
                DrawExtraLineBonusAnimation();
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        private void DrawMarkers()
        {
            int xx = 60;
            int yy = 50;
            for (int x = 0; x < X_GRID - 1; x++)
            {
                for (int y = 0; y < Y_GRID - 1; y++)
                {
                    if (player1Markers[x,y] == true)
                    {
                        Vector2 v = new Vector2(xx, yy);
                        spriteBatch.Draw(player1MarkerSprite, v, Color.White);

                    }
                    else if (player2Markers[x,y] == true)
                    {
                        Vector2 v = new Vector2(xx, yy);
                        spriteBatch.Draw(player2MarkerSprite, v, Color.White);

                    }
                    yy += GRID_SPACING;
                }
                xx += GRID_SPACING;
                yy = 50;
            }
        }
        private void DrawLines()
        {
            int y = Y_GRID_START;
            int x = X_GRID_START;

            for (int xx = 0; xx < X_GRID - 1; xx++)
            {
                for (int yy = 0; yy < Y_GRID; yy++)
                {
                    if (horizLines[xx,yy] == true)
                    {
                        Vector2 v = new Vector2(x + DOT_CENTER_OFFSET, y + DOT_CENTER_OFFSET);
                        spriteBatch.Draw(lineSprite, v, Color.White);
 
                    }
                    y += GRID_SPACING;
                }
                y = Y_GRID_START;
                x += GRID_SPACING;
            }

            y = Y_GRID_START;
            x = X_GRID_START;
            for (int xx = 0; xx < X_GRID; xx++)
            {
                for (int yy = 0; yy < Y_GRID - 1; yy++)
                {
                    if (vertLines[xx,yy] == true)
                    {
                        Vector2 v = new Vector2(x + DOT_CENTER_OFFSET, y + DOT_CENTER_OFFSET);
                        spriteBatch.Draw(vlineSprite, v, Color.White);
                    }
                    y += GRID_SPACING;
                }
                y = Y_GRID_START;
                x += GRID_SPACING;
            }
        }
        private void DrawExtraLineBonusAnimation()
        {
            spriteBatch.Draw(extraLineBonusSprite, new Vector2(extraLineBonusLocation.X, extraLineBonusLocation.Y), Color.White);

        }

        public void DrawGrid()
        {
            int y = Y_GRID_START;
            int x = X_GRID_START;
            for (int xx = 0; xx < X_GRID; xx++)
            {
                for (int yy = 0; yy < Y_GRID; yy++)
                {
                    Vector2 v = new Vector2(x, y);
                    spriteBatch.Draw(dotSprite, v, Color.White);

                    y += GRID_SPACING;
                }
                y = Y_GRID_START;
                x += GRID_SPACING;
            }
        }
    }
}
