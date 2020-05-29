using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Core.Services
{
    public interface ICache<T> 
    {
        // guarda hasta reinicio creo
        void Set(string key, T cacheValue);
        // guarda hasta la fecha indicada en expires
        void Set(string key, T cacheValue, System.DateTime expires);

        // guarda durante el tiempo especificado en expires
        void Set(string key, T cacheValue, System.TimeSpan expires);

        //Borra por la clave
        void Remove(string key);

        //  Borra por el prefijo de la clave
        void RemovePrefix(string prefixKey);

        //Obtiene valores por la clave de memoria
        T Get(string key);

        //Comprueba si existe el valor en memoria por la clave
        bool Exists(string key);

        int GetSlidingExpiration();

        int GetAbsoluteExpiration();

    }
}
