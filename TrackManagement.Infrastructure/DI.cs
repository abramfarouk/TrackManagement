using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Application.Interfaces.Services;
using TrackManagement.Infrastructure.Auth;
using TrackManagement.Infrastructure.Services;

namespace TrackManagement.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<IDspService, DspService>();

            return services;
        }
    }

}
