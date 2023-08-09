using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game_Ruleta.Models.Request
{
    public class Apuesta
    {
        public string nombre { get; set; }
        public float monto { get; set; }

        public float montoApostado { get; set; }
    }
}