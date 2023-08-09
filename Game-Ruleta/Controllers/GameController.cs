using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL;
using System.Web.Http.Cors;
using System.Data.Entity;
using Newtonsoft.Json.Linq;

namespace Game_Ruleta.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GameController : ApiController
    {
        // GET: api/Game        
        public IEnumerable<jugadores> Get()
        {
            using (GameEntities db = new GameEntities()){
                return db.jugadores.ToList();
            }
        }

        // GET: api/Game/numberRandom
        [Route("api/Game/numberRandom")]
        [HttpGet]
        public int numberRandom() //Devuelve un numero random
        {
            Random random = new Random();
            
            int numero = random.Next(1, 36);

            return numero;
        }

        // POST: api/Game
        public HttpResponseMessage Post([FromBody] Models.Request.Apuesta apuesta){
            int resp = 0;
            HttpResponseMessage response = null;

            try{
                using (GameEntities db = new GameEntities()){

                    var existeJugador = db.jugadores.Where(d => d.nombre == apuesta.nombre).Count();
                    //Si no existe este jugador guarda en la BD al nuevo jugador
                    if (existeJugador == 0){
                        jugadores jg = new jugadores();
                        jg.nombre = apuesta.nombre;
                        jg.monto = apuesta.monto;

                        db.Entry(jg).State = EntityState.Added;
                        resp = db.SaveChanges();
                    }else{
                        
                        jugadores oJugador = db.jugadores.Where(j => j.nombre == j.nombre).First();

                        var diferencia = oJugador.monto - apuesta.montoApostado;

                        double resultadoApuesta = 0;
                        //Se suma la ganancia a su monto del jugador
                        if ( apuesta.monto > 0 ){
                            //Si el jugador no tiene un monto guarda el valor ganado
                            if (oJugador.monto == 0){
                                resultadoApuesta = apuesta.monto;
                            }else { //Se suma la ganancia a su monto
                                var ganancia = apuesta.monto - apuesta.montoApostado;
                                resultadoApuesta = (double)(diferencia + ganancia);
                            }
                        }else{ //si perdio en el juego se resta el valor apostado de su monto
                            resultadoApuesta = (double)diferencia;
                        }

                        oJugador.monto = resultadoApuesta;
                        db.Entry( oJugador).State = EntityState.Modified;
                        resp = db.SaveChanges();
                    }

                    response = Request.CreateResponse(HttpStatusCode.OK, resp);
                }
            }catch (Exception ex){
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;      
        }

    }
}
