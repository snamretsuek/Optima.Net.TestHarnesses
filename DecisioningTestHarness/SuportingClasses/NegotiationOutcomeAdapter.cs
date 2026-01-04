using Optima.Net;
using Optima.Net.Decisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness
{
    internal sealed class NegotiationOutcomeAdapter : INegotiationOutcome
    {
        public NegotiationDisposition Disposition { get; }
        public Optional<object> Proposal { get; }
        public IReadOnlyCollection<object> Evidence { get; }

        public NegotiationOutcomeAdapter(
            NegotiationDisposition disposition,
            Optional<object> proposal,
            IReadOnlyCollection<object> evidence)
        {
            Disposition = disposition;
            Proposal = proposal;
            Evidence = evidence;
        }
    }
}
