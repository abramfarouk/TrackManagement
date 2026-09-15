using Microsoft.EntityFrameworkCore;
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
using TrackManagement.Infrastructure.Repositories;
using TrackManagement.Persistence.Context;
using TrackManagement.Persistence.Repositories;
using TrackManagement.Persistence.Services;

namespace TrackManagement.Persistence
{
    public static class DI
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<TrackManagementDbContext>(options =>
            {
                    options.UseSqlServer(configuration.GetConnectionString("TrackManagementDb"));
                
            });

            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddScoped<IDspRepository, DspRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

         
            return services;
        }
    }

}
