using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public interface IConfigurable<TConfig>
    {
        public void ApplyConfig(TConfig config);
    }
}
