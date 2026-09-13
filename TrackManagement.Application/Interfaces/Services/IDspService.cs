using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;

namespace TrackManagement.Application.Interfaces.Services
{
    public interface IDspService
    {
        Task<List<DspDto>> GetAllAsync(CancellationToken ct = default);
    }
}
