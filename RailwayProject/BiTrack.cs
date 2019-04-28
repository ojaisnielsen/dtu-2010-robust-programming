using System;
using System.Linq;
using System.Diagnostics.Contracts;


namespace Model
{ 
	public class BiTrack : Track
	{
		Node[] nodes;

        [ContractInvariantMethod]
        private void BiTrackObjectInvariant()
        {
            Contract.Invariant(nodes != null);            
            Contract.Invariant(nodes.All((Node node) => node != null));
            Contract.Invariant(nodes.Length == 2);
        }
		
		public BiTrack(Node node1, Node node2, int length) : base(length)
		{
			this.nodes = new Node[2];
			this.nodes[0] = node1;
			this.nodes[1] = node2;
		}

        public Node FirstNode
        {
			get
			{
				return this.nodes[0];
			}
        }

		public Node SecondNode
		{
			get
			{
				return this.nodes[1];
			}
		}
	}	
}