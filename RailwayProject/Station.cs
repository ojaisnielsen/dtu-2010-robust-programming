using System;
using System.Diagnostics.Contracts;


namespace Model
{
	public class Station : Node
	{
		private string name;

        [ContractInvariantMethod]
        private void StationObjectInvariant()
        {
            Contract.Invariant(name != null);
        }
		
		
		public Station(string id, string name) : base(id)
		{
			this.name = name;
		}

		public String getName()
		{
			return name;
		}
	}	
}
