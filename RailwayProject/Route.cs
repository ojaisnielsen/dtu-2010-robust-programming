using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Model;

namespace TrainSimulation
{
	public class Route
	{
		private String id;

		private NodeList<Node> nodes;

		private Boolean isComplete;

        [ContractInvariantMethod]
        private void RouteObjectInvariant()
        {
            Contract.Invariant(id != null);
            Contract.Invariant(nodes != null);
            Contract.Invariant(nodes.All((Node node) => node != null));
        }

		public Route(String id)
		{
			this.id = id;

			this.nodes = new NodeList<Node>();
			isComplete = false;
		}

		public Route(String id,NodeList<Node> nodes, Boolean isComplete)
		{
			this.id = id;
			this.nodes = new NodeList<Node>();

			foreach (Node node in nodes)
			{
				this.addNextNode(node);
			}

			this.isComplete = isComplete;
		}

		public String Id
		{
			get
			{
				return this.id;
			}
		}


		public void addNextNode(Node node)
		{
			Contract.Requires(node != null);
            Contract.Requires<ApplicationException>(this.nodes.Count() == 0 || this.nodes[this.nodes.Count() - 1].getReachableNodes().Contains(node), "!The node list is inconsistent!\n");
            //if (this.nodes.Count() != 0 && !this.nodes[this.nodes.Count() - 1].getReachableNodes().Contains(node))
            //{
            //    throw new ApplicationException("The node list is inconsistent - node '" + node.Id + "' cannot be reached from node '" + this.nodes[this.nodes.Count() - 1].Id);        
            //}
            //Contract.EndContractBlock();

            this.nodes.Add(node);
		}

		public void finishRoute()
		{
			this.isComplete = true;
		}

		public NodeList<Node> showRoute()
		{
			return this.nodes;
		}
	}

	public class RouteList<T> : List<T> where T : Route
	{
		public T containsId(String routeId)
		{
            Contract.Ensures(Contract.Result<T>() == null || Contract.Result<T>().Id == routeId);
			foreach (T t in this)
			{
				if (t.Id == routeId)
				{
					return t;
				}
			}

			return null;
		}
	}
}
