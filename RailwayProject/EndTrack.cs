using System;
using System.Diagnostics.Contracts;


namespace Model
{
	public class EndTrack : Edge
	{
		Node node;

        [ContractInvariantMethod]
        private void EndTrackObjectInvariant()
        {
            Contract.Invariant(node != null);
        }
		
		public EndTrack(Node node) : base()
		{
			this.node = node;
		}

		public Node Node
		{
			get
			{
				return this.node;
			}
		}
			
	}
}