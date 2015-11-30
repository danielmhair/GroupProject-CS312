using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class IncludeExcludeStates
    {
        private State includeState;
        private State excludeState;

        public IncludeExcludeStates(State includeState, State excludeState)
        {
            this.includeState = includeState;
            this.excludeState = excludeState;
        }
        
        public State IncludeState()
        {
            return includeState;
        }

        public State ExcludeState()
        {
            return excludeState;
        }
    }
}
