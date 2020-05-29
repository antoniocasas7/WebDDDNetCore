using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.IO;
using System.Web;


namespace InfrastructureCache
{
    // HE ADPTADO ESTA CLASE A .NET CORE ,YA QUE LA CACHE Y EL CONTEXTO DE USUARIO CAMBIAN
    public class HttpRuntimeCache<T> : Domain.Core.Services.ICache<T>
    {

        private const int AbsoluteMinutesCache = 720;

        private const int SlidingMinutesCache = 720;

        private IHttpContextAccessor _httpContextAccessor; // Lo uso para acceder al contexto del usuario logeado

        private IMemoryCache _cache;

        private bool _webapi = false;

        //CONSTRUCTOR UNICO, REGISTRO DE AUTOFAC DE LA LLAMADA DESDE LA WEB API Y DESDE LA WEB
        public HttpRuntimeCache(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, bool webapi = false)
        {
            this._httpContextAccessor = httpContextAccessor; // Desde la llamada desde la WebApi viene a null pero no lo necesito porque tenemos el Token

            this._cache = cache;
            //Esto es para la llamada desde la web api, QUE VENDRA A NULL
            if(this._cache == null)
                this._cache = new MemoryCache(new MemoryCacheOptions());

            this._webapi = webapi;
        }


        /// <summary>
        /// Guarda en memoria por la clave _Key , loa valores de _oCacheValue, hasta nueva request creo
        /// </summary>
        public void Set(string _Key, T _oCacheValue)
        {                   
            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if ((_oCacheValue == null || string.IsNullOrEmpty(_Key)))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.Set :: Parámetros de entrada incorrectos");

           _cache.Set(_Key, _oCacheValue);         
        }

        /// <summary>
        /// Guarda en memoria por la clave _Key , loa valores de _oCacheValue, hasta la fecha _dtExpires
        /// </summary>
        public void Set(string _Key, T _oCacheValue, System.DateTime _dtExpires)
        {

            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if (_oCacheValue == null || string.IsNullOrEmpty(_Key))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.Set :: Parámetros de entrada incorrectos");

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Guarda los datos en meoria hasta la fecha _dtExpires
                .SetAbsoluteExpiration(_dtExpires);

            // Save data in cache.
            _cache.Set(_Key, _oCacheValue, cacheEntryOptions);
          
        }


        /// <summary>
        /// Guarda en memoria por la clave _Key , loa valores de _oCacheValue, durante el tiempo _dtExpires
        /// </summary>
        public void Set(string _Key, T _oCacheValue, System.TimeSpan _dtExpires)
        {
            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if (_oCacheValue == null || string.IsNullOrEmpty(_Key))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.Set :: Parámetros de entrada incorrectos");

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Guarda los datos en memoria durante el tiempo _dtExpires
                .SetSlidingExpiration(_dtExpires);

            // Save data in cache.
            _cache.Set(_Key, _oCacheValue, cacheEntryOptions);
        }


        /// <summary>
        /// Borra por la clave
        /// </summary>
        public void Remove(string _Key)
        {
            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if (string.IsNullOrEmpty(_Key))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.Set :: Parámetros de entrada incorrectos");

            _cache.Remove(_Key);
        }


        /// <summary>
        /// Borra por el prefijo de la clave
        /// </summary>
        public void RemovePrefix(string prefixKey)
        {
            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            foreach (DictionaryEntry dicache in HttpRuntime.Cache)
            {
                if (dicache.Key.ToString().ToLower().StartsWith(prefixKey.ToLower()))
                    _cache.Remove(dicache.Key.ToString());
            }
        }

        /// <summary>
        /// Obtiene valores por la clave de memoria
        /// </summary>
        public T Get(string _Key)
        {
            //if (System.Web.HttpContext.Current == null). Este es en .Net
            if(_webapi == false)
            { 
                if(_httpContextAccessor.HttpContext == null) //Este es en .Net Core
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if (string.IsNullOrEmpty(_Key))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Parámetros de entrada incorrectos");

            _cache.TryGetValue(CacheKeys.Personas, out T personaToReturn);
            return personaToReturn;

        }


        /// <summary>
        /// Comprueba si existe el valor en memoria por la clave
        /// </summary>
        public bool Exists(string _Key)
        {

            if (_webapi == false) // Si la llamada es desde la Web comprobamos el Contexto del usuario
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Persistence.InMemory.HttpRuntimeCache.GetValue :: Entorno HttpContext incorrecto.");
            }

            if (string.IsNullOrEmpty(_Key))
                throw new InvalidDataException("Persistence.InMemory.HttpRuntimeCache.Exists :: Parámetros de entrada incorrectos");

            if (_cache.Get(_Key) != null)
                return true;

            return false;
        }

        public int GetSlidingExpiration()
        {
            return SlidingMinutesCache;
        }

        public int GetAbsoluteExpiration()
        {
            return AbsoluteMinutesCache;
        }
    } 
}
