using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dfe.Server.Engine;

namespace dfe.Server.Services
{
    public class CircuitHandlerService : CircuitHandler
    {
        public ConcurrentDictionary<string, Circuit> circuits { get; set; }
        public event EventHandler event_circuitsChanged;

        protected virtual void OnCircuitsChanged() => event_circuitsChanged?.Invoke(this, EventArgs.Empty);

        public CircuitHandlerService()
        {
            circuits = new ConcurrentDictionary<string, Circuit>();
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine("Connection opened: {0}", circuit.ToString());
            circuits[circuit.Id] = circuit;
            OnCircuitsChanged();
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine("Connection closed: {0}", circuit.ToString());
            Circuit removed_circuit;
            circuits.TryRemove(circuit.Id, out removed_circuit);
            OnCircuitsChanged();
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine("Connection interrupted: {0}", circuit.ToString());
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine("Connection restored: {0}", circuit.ToString());
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}
