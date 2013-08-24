using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Pelotitas
{
    public class Menu
    {
        public int Seleccion { get; set; }
        Texture2D Textura;
        private SpriteFont font;
        private ContentManager content;
        private KeyboardState oldState;
        private KeyboardState state;
        private Game1 game1;
        private bool jugar = false;
        public String nick = "yatan";
        public String pass = "*******";

        //Notificacion
        private Texture2D t_notificacion;
        private String notificacion = "";
        private int timeNotificacion;

        public bool hayConexion = false;

        enum GameStates { MenuPrincipal, MenuLogin, MenuOpciones };
        GameStates gameState = GameStates.MenuPrincipal;

        string text = "";


        Keys[] keysToCheck = new Keys[] { 
    Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
    Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
    Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
    Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
    Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
    Keys.Z, Keys.Back, Keys.Space, Keys.D1 };


        public Menu(int seleccion, ContentManager content, Game1 game)
        {
            this.content = content;
            //content.RootDirectory = "Content";
            Seleccion = seleccion;
            font = content.Load<SpriteFont>("Fuente1");
            Textura = content.Load<Texture2D>("dedo");
            t_notificacion = content.Load<Texture2D>("Menu/notificacion");
            game1 = game;

        }



        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textura, new Rectangle(50, 433 + (25 * Seleccion), 250, 107), Color.White);
            //Mostrar notificacion
            if (timeNotificacion > 0)
            {
                spriteBatch.Draw(t_notificacion, new Rectangle(50, 25, 12*notificacion.Length, 50), Color.White);
                spriteBatch.DrawString(font, notificacion.ToString(), new Vector2(60, 35), Color.Red);
            }

            if (gameState == GameStates.MenuPrincipal)
            {
                if (this.hayConexion == true)
                {
                    spriteBatch.DrawString(font, "Jugar Online", new Vector2(400, 450), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(font, "Jugar Online - (No disponible)", new Vector2(400, 450), Color.Red);
                }
                spriteBatch.DrawString(font, "OPCIONES", new Vector2(400, 475), Color.Blue);
                spriteBatch.DrawString(font, "Salir", new Vector2(400, 500), Color.Red);
            }
            else if (gameState == GameStates.MenuOpciones)
            {
                spriteBatch.DrawString(font, "OPCIONES", new Vector2(375, 425), Color.Black);
                spriteBatch.DrawString(font, "    Sonido:", new Vector2(300, 450), Color.Blue);
                spriteBatch.DrawString(font, "Fullscreen:", new Vector2(300, 475), Color.Red);
                spriteBatch.DrawString(font, "Atras", new Vector2(400, 500), Color.Black);
            }
            else if (gameState == GameStates.MenuLogin)
            {
                spriteBatch.DrawString(font, " Usuario:", new Vector2(300, 450), Color.Black);
                spriteBatch.DrawString(font, "Password:", new Vector2(300, 475), Color.Blue);
                spriteBatch.DrawString(font, nick, new Vector2(400, 450), Color.Red);
                spriteBatch.DrawString(font, pass, new Vector2(400, 475), Color.Black);
                spriteBatch.DrawString(font, "LOGIN", new Vector2(400, 500), Color.Blue);
            }
        }

        public bool Estado()
        {
            if (jugar == true)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            this.jugar = false;
        }

        public void Update()
        {
            state = Keyboard.GetState();
            if (timeNotificacion > 0)
            {
                timeNotificacion -= 1;
            }

            if (oldState.IsKeyUp(Keys.Down) && state.IsKeyDown(Keys.Down))
            {
                if (Seleccion >= 0 && Seleccion < 2)
                    Seleccion += 1;
            }
            else if (oldState.IsKeyUp(Keys.Up) && state.IsKeyDown(Keys.Up))
            {
                if (Seleccion > 0 && Seleccion <= 2)
                    Seleccion -= 1;
            }
            else if (oldState.IsKeyUp(Keys.Enter) && state.IsKeyDown(Keys.Enter))
            {
                if (gameState == GameStates.MenuPrincipal && Seleccion == 1)
                    gameState = GameStates.MenuOpciones;
                else if (gameState == GameStates.MenuOpciones && Seleccion == 2)
                    gameState = GameStates.MenuPrincipal;
                else if (gameState == GameStates.MenuPrincipal && Seleccion == 2)
                {
                    //Salir
                    game1.Exit();

                }
                else if (gameState == GameStates.MenuLogin && Seleccion == 2)
                {
                    //Jugar
                    jugar = true;
                }
                else if (gameState == GameStates.MenuPrincipal && Seleccion == 0)
                {
                    if (this.hayConexion == true)
                    {
                        gameState = GameStates.MenuLogin;
                    }
                }



            }


            foreach (Keys key in keysToCheck)
            {
                if (CheckKey(key))
                {
                    AddKeyToText(key);
                    break;
                }
            }
            oldState = state;
            //base.Update(gameTime);

        }



        public void notificar(String text, bool largo = false)
        {
            if (largo == true)
            {
                this.timeNotificacion = 360;
            }
            else
            {
                this.timeNotificacion = 180;
            }
            this.notificacion = text;
        }

        private void AddKeyToText(Keys key)
        {
            string newChar = "";

            if (text.Length >= 20 && key != Keys.Back)
                return;

            switch (key)
            {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.D1:
                    newChar += "1";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.Back:
                    if (Seleccion == 0)
                    {
                        if (nick.Length != 0)
                            nick = nick.Remove(nick.Length - 1);
                    }
                    else if (Seleccion == 1)
                    {
                        if (pass.Length != 0)
                            pass = pass.Remove(pass.Length - 1);
                    }
                    break;

            }
            if (state.IsKeyDown(Keys.RightShift) ||
                state.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }
            if (Seleccion == 0)
                nick += newChar;
            else if (Seleccion == 1)
                pass += newChar;
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && state.IsKeyUp(theKey);
        }


    }
}
