using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;

namespace WrongWarp.Objects
{
    [Serializable]
    public class QuantumGroup
    {
        public QuantumEntityState CurrentState;
        public List<QuantumEntityState> States = new List<QuantumEntityState>();
    }
}
