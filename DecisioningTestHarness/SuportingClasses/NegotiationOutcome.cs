using System.Collections.Generic;
using Optima.Net;
using Optima.Net.Decisioning;

namespace TestHarness
{
    /// <summary>
    /// Simple in-memory implementation of INegotiationOutcome for demonstration purposes.
    /// </summary>
    public sealed class NegotiationOutcome : INegotiationOutcome
    {
        public NegotiationDisposition Disposition { get; }
        public Optional<object> Proposal { get; }
        public IReadOnlyCollection<object> Evidence { get; }

        public NegotiationOutcome(
            NegotiationDisposition disposition,
            Optional<object> proposal,
            IReadOnlyCollection<object> evidence)
        {
            Disposition = disposition;
            Proposal = proposal;
            Evidence = evidence;
        }

        public static NegotiationOutcome CounterProposal(object proposal, params object[] evidence) =>
            new(NegotiationDisposition.Modified, Optional<object>.Some(proposal), evidence);
    }
}
