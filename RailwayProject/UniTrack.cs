using System;
using System.Diagnostics.Contracts;

namespace Model
{ 
	public class UniTrack : Track
	{
		private Node startNode;
		private Node endNode;

        [ContractInvariantMethod]
        private void UniTrackObjectInvariant()
        {
            Contract.Invariant(startNode != null);
            Contract.Invariant(endNode != null);
            Contract.Invariant(endNode != startNode);
        }

        public UniTrack(Node startNode, Node endNode, int length) : base(length)
		{
			this.startNode = startNode;		
			this.endNode = endNode;
		}

		public Node StartNode
		{
			get
			{
				return this.startNode;
			}
		}

		public Node EndNode
		{
			get
			{
				return this.endNode;
			}
		}


	}	
}
