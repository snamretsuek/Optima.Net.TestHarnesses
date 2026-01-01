using NegotiatRTestHarness.Domain.Password.Policies;
using NegotiatRTestHarness.Domain.Password.Specifications;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Specification;
using Optima.Net.NegotiatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.Justifications
{
    public sealed class PasswordCreationPolicyJustification
    : IPolicyJustification<IProposal>
    {
        public string PolicyName => nameof(PasswordCreationPolicy);

        public IReadOnlyCollection<ISpecification<IProposal>> Specifications { get; }

        public PasswordCreationPolicyJustification(
            PasswordMeetsLengthSpec lengthSpec,
            PasswordMeetsRotationSpec rotationSpec,
            PasswordMeetsEntropySpec entropySpec)
        {
            if (lengthSpec is null) throw new ArgumentNullException(nameof(lengthSpec));
            if (rotationSpec is null) throw new ArgumentNullException(nameof(rotationSpec));
            if (entropySpec is null) throw new ArgumentNullException(nameof(entropySpec));

            Specifications = new ISpecification<IProposal>[]
            {
            lengthSpec,
            rotationSpec,
            entropySpec
            };
        }
    }
}
