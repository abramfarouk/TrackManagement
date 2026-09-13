using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Domain.Common;

namespace TrackManagement.Domain.Entities
{
    public class Dsp : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<TrackDistribution> TrackDistributions { get; set; } = new List<TrackDistribution>();
    }
}
