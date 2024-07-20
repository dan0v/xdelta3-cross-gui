using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xdelta3_cross_gui.Models
{
    public record struct PatchCreationJob(string Options, string Source, string Goal, string PatchDestination);
}
