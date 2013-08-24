using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleServer
{
    class Player
    {
        public String UID;
        private int pos_x;
        private int pos_y;
        public int mapa;
        public string nick;
        private bool logeado;

        public Player(String UiD)
        {
            this.UID = UiD;
            this.logeado = false;
        }

        public void updatePos(int pos_X, int pos_Y)
        {
            this.pos_x = pos_X;
            this.pos_y = pos_Y;
        }

        public void login(string Nick)
        {
            this.nick = Nick;
            this.logeado = true;
        }

        public bool is_loged()
        {
            return this.logeado;
        }
        
    }
}
