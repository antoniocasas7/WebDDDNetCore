using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Core.Model.Persona;
using WebDDDNetCore.Data;
using Aplication.Services.Persona;
using RestSharp;
using Aplication.Services.Persona.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebDDDNetCore.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        private readonly ApplicationDbContext db;

        private IPersonaQueryRepository _personasQueryRepository;
        private IPersonaCommandService _personaCommandService;
        private IPersonaQueryService _personaQueryService;

        public string tokenObtenido;
        protected static RestClient client = null;

        private IHttpContextAccessor _httpContextAccessor; // Lo usao para acceder al contexto del usuario logeado

        public PersonasController(ApplicationDbContext context, IPersonaQueryRepository personasQueryRepository, IPersonaCommandService personaCommandService, IPersonaQueryService personaQueryService, IHttpContextAccessor httpContextAccessor)
        {
           this.db = context;
            this._personasQueryRepository = personasQueryRepository;
            this._personaCommandService = personaCommandService;
            this._personaQueryService = personaQueryService;
            this._httpContextAccessor = httpContextAccessor;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {

            //USANDO ENTIFY FRAMEWORK CON EL DBCONTEXT DESDE EL PROYECTO WEBDDNET
             return View(await db.Persona.ToListAsync());

            //USANDO PERSONAS QUERY DE LA INFRASTRUCTURE PERSISTENCE SI NECESITAMOS DATOS DE TIPO DE LA BB:DD
           // return View(await this._personasQueryRepository.GetAll());

            //USANDO PERSONAS QUERYSERVICE DE APPLICATION SERVICE SI NECESITAMOS DATOS DE TIPO DTO
            //var personaDto = await this._personaQueryService.GetAllPersona();
            //return View(personaDto);

            //USANDO LA FUNCION QUE HAY EN LA WEB API, para usarla enla vista cambiar el IEnumerable a PersonaDto
            //List<PersonaDTO> personas = new List<PersonaDTO>();
            //tokenObtenido = await ObtenerTokenAsync();
            //if (!string.IsNullOrEmpty(tokenObtenido))
            //{
            //    // Si Id = 0 son todas las personas. En este caso devuekve todas y que contienen "pe"
            //    personas = GetPersonasDto(0, "pe");
            //}
            //return View(personas);
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await db.Persona
                .FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dni,Nombre,Apellidos,Telefono")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Add(persona);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

                //PersonaDTO varpersonaDTO = new PersonaDTO();
                //varpersonaDTO.Id = persona.Id;
                //varpersonaDTO.nombre = persona.Nombre;
                //varpersonaDTO.dni = persona.Dni;
                //varpersonaDTO.apellidos = persona.Apellidos;
                //varpersonaDTO.telefono = persona.Telefono;
                //var result = await _personaCommandService.CreateNewPersonaAsync(varpersonaDTO);
            }
            //  return View(persona);
          //  return RedirectToAction("Index", "Personas");
            return Redirect("~/Home/Index");
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await db.Persona.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }
            return View(persona);
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dni,Nombre,Apellidos,Telefono")] Persona persona)
        {
            if (id != persona.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(persona);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await db.Persona
                .FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await db.Persona.FindAsync(id);
            db.Persona.Remove(persona);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return db.Persona.Any(e => e.Id == id);
        }


        #region Metodos
        /// <summary>
        ///  Obtiene el Token />
        /// </summary>
        /// <returns>String con el Token</returns>   

        private async Task<string> ObtenerTokenAsync()
        {      
          //  var userName1 = HttpContext.User.Identity.Name; // ESte es de ASP.NET , funciona aqui tambien

            var userName = _httpContextAccessor.HttpContext.User.Identity.Name; // Este es el que se usa en .NetCore


            HttpResponseMessage lista = new HttpResponseMessage();

            HttpClient client = new HttpClient();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("client_id", "api-stag"));
            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formData.Add(new KeyValuePair<string, string>("username", userName));
            formData.Add(new KeyValuePair<string, string>("password", "IoT@123")); // Se pone a pelo porque se supone que el que lo usa para obtener el token lo sabe
           
            formData.Add(new KeyValuePair<string, string>("scope", ""));

            var content = new FormUrlEncodedContent(formData);
            var respuesta = await client.PostAsync("http://localhost:56276/Login", content);

            if (respuesta.IsSuccessStatusCode)
            {
                //Esto es para obtener el Token desglosado de la respuesta.
                var result = respuesta.Content.ReadAsStringAsync().Result;
                var separados = result.Split('"');
                //El 3 es el Token
                return separados[3];
            }
            else
                return string.Empty;

        }


        /// <summary>
        ///     Obtiene una lista de Personas <see cref="PersonaDTO"/>
        /// </summary>
        /// <param name="SearchString">Personas que contienen esa cadena</param>
        /// <param name="Id">Id de la Persona a buscar . Si es = 0 es todas </param>
        /// <returns>Lista de ubicaciones</returns>
        private List<PersonaDTO> GetPersonasDto(int Id, string SearchString)
        {
            try
            {
                client = new RestClient("http://localhost:56276/");
                //  var request = new RestRequest("api/Personas/Get/" + Id, Method.GET)
                var request = new RestRequest("api/Personas/Get/", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };
                // Añdo los parametros 
                request.AddParameter("Id", Id);
                request.AddParameter("SearchString", SearchString);
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddHeader("Authorization", "bearer" + " " + tokenObtenido);

                //  request.AddHeader("OriginAccess", _originAccess);
                IRestResponse<List<PersonaDTO>> response = client.Execute<List<PersonaDTO>>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Data == null && response.Content != null)
                        response.Data = JsonConvert.DeserializeObject<List<PersonaDTO>>(response.Content);
                    if (response.Data != null)
                    {
                        List<PersonaDTO> personas = JsonConvert.DeserializeObject<List<PersonaDTO>>(JsonConvert.SerializeObject(response.Data));
                        return personas;
                    }
                    else
                        //Mensaje de error
                        Console.WriteLine(response.ErrorMessage);
                }
                else
                {
                    //Si no estamos autorizados significa que ha caducado el token, asi que volvemos a hacer el login
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        tokenObtenido = ObtenerTokenAsync().ToString();
                        return GetPersonasDto(Id, SearchString);
                    }
                    throw new Exception("Server Error: " + response.ErrorMessage);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
            return null;
        }
        #endregion
    }
}
