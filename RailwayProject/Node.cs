using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Model
{

	public class Node
	{
		protected String id;
		protected List<Track> tracks;
		protected List<EndTrack> endTracks;

        protected NodeList<Node> reachableNodes;
        protected NodeList<Node> accessingNodes;

        [ContractInvariantMethod]
        private void NodeObjectInvariant()
        {
            Contract.Invariant(id != null);
            Contract.Invariant(tracks != null);
            Contract.Invariant(endTracks != null);
            Contract.Invariant(reachableNodes != null);
            Contract.Invariant(accessingNodes != null);
            Contract.Invariant(tracks.All((Track track) => track != null));
            Contract.Invariant(endTracks.All((EndTrack track) => track != null));
            Contract.Invariant(reachableNodes.All((Node node) => node != null));
            Contract.Invariant(accessingNodes.All((Node node) => node != null));
            Contract.Invariant(tracks.All((Track track) => track is UniTrack || track is BiTrack));
        }
		
		
		public Node(String id)
		{
            tracks = new List<Track>();
            endTracks = new List<EndTrack>();

            reachableNodes = new NodeList<Node>();
            accessingNodes = new NodeList<Node>();

			this.id = id;
		}
		
		public String Id {
			get
			{
				return this.id;
			}
		}

        public List<EndTrack> EndTracks
        {
            get
            {
                return this.endTracks;
            }
        }

		public void addTrack(Track track)
		{
			this.tracks.Add(track);

            if (track is UniTrack)
            {
                UniTrack uTrack = (UniTrack)track;
                if (uTrack.StartNode.Equals(this))
                {
                    reachableNodes.Add(uTrack.EndNode);
                }
                else if (uTrack.EndNode.Equals(this))
                {
                    accessingNodes.Add(uTrack.StartNode);
                }
                else
                {
                    //unexpected error!!!
                    System.Console.WriteLine("Error in UniTrack Nodes assignment!!!");
                }
            }
            else if (track is BiTrack)
            {
                BiTrack bTrack = (BiTrack)track;
                if (bTrack.FirstNode.Equals(this))
                {
                    reachableNodes.Add(bTrack.SecondNode);
                    accessingNodes.Add(bTrack.SecondNode);
                }
                else if (bTrack.SecondNode.Equals(this))
                {
                    reachableNodes.Add(bTrack.FirstNode);
                    accessingNodes.Add(bTrack.FirstNode);
                }
                else
                {
                    //unexpected error!!!
                    System.Console.WriteLine("Error in BiTrack Nodes assignment!!!");
                }
            }
            else
            {
                //unexpecter error!!!
                System.Console.WriteLine("A track is neither UniTrack nor BiTrack");
            }

		}

		public void addEndTrack()
        {
			endTracks.Add(new EndTrack(this));
		}

        public NodeList<Node> getReachableNodes()
        {
            return reachableNodes;
        }

        public NodeList<Node> getAccessingNodes()
        {
            return accessingNodes;
        }

		public Track getAvailableConnectionTo(Node node)
		{
            Contract.Requires(node != null);
			Contract.Requires(reachableNodes.Contains(node), "Entered node cannot be reached from this node");

			foreach (Track track in this.tracks)
			{
				if (track.IsOccupied == false)
				{
					if (track is UniTrack)
					{
						if (((UniTrack)track).EndNode.Equals(node))
						{
							return track;
						}
					}
					else if (track is BiTrack)
					{
						if (((BiTrack)track).FirstNode.Equals(node) || ((BiTrack)track).SecondNode.Equals(node))
						{
							return track;
						}
					}
					else
					{
						//unknown Track type
						throw new ArgumentException("Unknown Track type entered");
					}
				}

				
			}

			return null;

		}

		public Track getAvailableConnectionFrom(Node node)
		{
			Contract.Requires(accessingNodes.Contains(node), "Entered node cannot reach this node");

			foreach (Track track in this.tracks)
			{
				if (track.IsOccupied == false)
				{
					if (track is UniTrack)
					{
						if (((UniTrack)track).StartNode.Equals(node))
						{
							return track;
						}
					}
					else if (track is BiTrack)
					{
						if (((BiTrack)track).FirstNode.Equals(node) || ((BiTrack)track).SecondNode.Equals(node))
						{
							return track;
						}
					}
				}
			}

			return null;

		}

		public EndTrack getAvailableEndTrack()
		{
			foreach (EndTrack end in this.endTracks)
			{
				if (end.IsOccupied == false)
				{
					return end;
				}
			}

			return null;
		}
	}



	public class NodeList<T> : List<T> where T : Node
	{
		public T containsId(String nodeId)
		{
            Contract.Ensures(Contract.Result<T>() == null || Contract.Result<T>().Id == nodeId);
			foreach (T t in this)
			{
				if (t.Id == nodeId)
				{
					return t;
				}
			}

			return null;
		}

		public Station containsName(String stationName)
		{
            Contract.Ensures(Contract.Result<Station>() == null || Contract.Result<Station>().getName() == stationName);
			foreach (Node t in this)
			{
				if (t is Station){

					if (((Station)t).getName().Equals(stationName))
					{

						return (Station)t;
					}
					
				}

			}

			return null;
		}
	}
}
