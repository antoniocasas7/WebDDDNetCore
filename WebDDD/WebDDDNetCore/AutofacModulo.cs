using Aplication.Services.Persona;
using Aplication.Services.Persona.Implementation;
using Autofac;
using Autofac.Features.Variance;
using Domain.Core.Model.Persona;
using Domain.Core.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using InfrastructureCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace WebDDDNetCore
{
    public class AutofacModulo : Autofac.Module
    {
        public IMemoryCache cache;
        public IHttpContextAccessor httpContextAccessor;
        public bool webapi = false;

        protected override void Load(ContainerBuilder builder)
        {
            //DEPENDENCIAS DE LA APLICACION

            // APPLICATION SERVICE
            builder.RegisterType<PersonaQueryService>().As<IPersonaQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<PersonaCommandService>().As<IPersonaCommandService>().InstancePerLifetimeScope();

            //* INFRASTRUCTURE

            //CacheRepository for Ad
              builder.RegisterType<InfrastructureCache.HttpRuntimeCache<IEnumerable<Domain.Core.Model.Persona.Persona>>>().As<ICache<IEnumerable<Domain.Core.Model.Persona.Persona>>>().InstancePerLifetimeScope();

            //Sql connection type & connectionString
            builder.RegisterType<Infrastructure.Persistence.SQL.SqlConnectionFactory>()
                .As<Infrastructure.Persistence.SQL.IConnectionFactory>()
                .WithParameter("connectionString", Startup.StaticConfig.GetConnectionString("DefaultConnection"))
                .InstancePerLifetimeScope();

            builder.RegisterType<Infrastructure.Persistence.SQL.Persona.PersonaQueryRepository>().As<IPersonaQueryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<Infrastructure.Persistence.SQL.Persona.PersonaCommandRepository>().As<IPersonaCommnadRepository>().InstancePerLifetimeScope();
            //*

            //DOMAIN SERVICES - ACL

            //ACL for external API façade
            //builder.RegisterType<ACL.PostalCodeAdapter>().As<Domain.Core.Services.IPostalCodeAdapter>().InstancePerRequest();
            //builder.RegisterType<ACL.PostalCodeTranslator>().As<Domain.Core.Services.IPostalCodeTranslator>().InstancePerRequest();
            //*
            //*


            //MEDIATR
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
        
            //**
        }
    }
}
