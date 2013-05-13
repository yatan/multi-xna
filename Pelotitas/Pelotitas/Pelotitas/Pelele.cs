using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pelotitas
{
    public class Pelele
    {
        public string uid;
        private Vector2 posicion;

        public Pelele(string uid)
        {
            this.uid = uid;
        }

        public void setPosition(int posX, int posY)
        {
            this.posicion = new Vector2(posX, posY);
        }

        public Vector2 getPosition()
        {
            return posicion;
        }
    }
}
