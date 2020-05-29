using Domain.Core.Model.Persona;
using Domain.Core.Services;
using Infrastructure.Persistence.SQL.Persona.QueryObjects;
using InfrastructureCache;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace Infrastructure.Persistence.SQL.Persona
{
    public class PersonaQueryRepository : IPersonaQueryRepository
    {
        private readonly IConnectionFactory connection;
        private readonly ICache<IEnumerable<Domain.Core.Model.Persona.Persona>> cacheRepository;

        public PersonaQueryRepository(IConnectionFactory connectionFactory, ICache<IEnumerable<Domain.Core.Model.Persona.Persona>> cacheRepository)
        {
            this.connection = connectionFactory;
            this.cacheRepository = cacheRepository;
        }       

        public async Task<IEnumerable<Domain.Core.Model.Persona.Persona>> GetAll()
        {
            IEnumerable<Domain.Core.Model.Persona.Persona> personaToReturn = cacheRepository.Get(CacheKeys.Personas);
            if (personaToReturn != null)
                return personaToReturn;

            using (IDbConnection dbConnection = connection.Create())
            {
                QueryObject byAll = new PersonaSelect().All();
                personaToReturn = dbConnection.Query<Domain.Core.Model.Persona.Persona>(byAll);

                  cacheRepository.Set(CacheKeys.Personas, personaToReturn);
                // Si lo quiero guardar durante 10 segundos
               // cacheRepository.Set(CacheKeys.Personas, personaToReturn, TimeSpan.FromSeconds(30)); 
                return personaToReturn;
            }
        }


        public async Task<Domain.Core.Model.Persona.Persona> GetById(int id)
        {
            IEnumerable<Domain.Core.Model.Persona.Persona> personaToReturn = cacheRepository.Get(CacheKeys.Personas + id.ToString());
            if (personaToReturn != null)
                return personaToReturn.SingleOrDefault();

            using (IDbConnection dbConnection = connection.Create())
            {
                QueryObject byId = new PersonaSelect().ByID(id);
                personaToReturn = dbConnection.Query<Domain.Core.Model.Persona.Persona>(byId);

                cacheRepository.Set("Persona" + id, personaToReturn);
                return personaToReturn.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Domain.Core.Model.Persona.Persona>> GetBySearchText(string text)
        {
            using (IDbConnection dbConnection = connection.Create())
            {
                QueryObject byAllText = new PersonaSelect().AllBySearchText(text);
                var personaToReturn = dbConnection.Query<Domain.Core.Model.Persona.Persona>(byAllText);
                return personaToReturn;
            }
        }
    }
}
