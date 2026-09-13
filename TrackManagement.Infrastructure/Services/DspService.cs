using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Services;

namespace TrackManagement.Infrastructure.Services
{

    public class DspService : IDspService
    {
        private readonly IUnitOfWork _uow;

        public DspService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DspDto>> GetAllAsync(CancellationToken ct = default)
        {
            var dsps = await _uow.Dsps.GetAllAsync(ct);
            return dsps.Select(d => new DspDto(d.Id, d.Name)).ToList();
        }
    }
}
