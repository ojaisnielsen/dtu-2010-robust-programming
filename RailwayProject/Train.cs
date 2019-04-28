using System;
using System.Diagnostics.Contracts;
using Model;
using System.Collections.Generic;


namespace TrainSimulation
{
	public class Train
	{
		private String id;

		private int speed = 100; //number of meters that train is moving each iteration

		private int nextNodeIndex;
		private Edge currentEdge;
		private int position;

		private Route route;

        [ContractInvariantMethod]
        private void TrainObjectInvariant()
        {
            Contract.Invariant(this.route != null);
            Contract.Invariant(this.speed >= 0);
            Contract.Invariant(this.id != null);
            Contract.Invariant(this.position >= 0);
        }

		public Train(String id, Route route, int speed)
		{

			this.id = id;
			this.speed = speed;

			this.currentEdge = null;
			this.position = 0;
			this.nextNodeIndex = -1;

			this.route = route;
		}

		public String Id
		{
			get
			{
				return this.id;
			}
		}

		public int Speed
		{
			get
			{
				return this.speed;
			}
		}

		public Node LastVisitedNode
		{
			get
			{
				return this.route.showRoute()[this.nextNodeIndex - 1];
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public Edge CurrentEdge
		{
			get
			{
				return this.currentEdge;
			}
		}

		public bool updatePosition(int distance)
		{
            Contract.Requires(distance >= 0);
			Contract.Requires(RailwayProject.Simulation.IsRunning);

			if(this.nextNodeIndex == -1)
			//start of the simulation
			{
				this.nextNodeIndex = 1;
				Edge tempEdge = this.switchEdge(this.nextNodeIndex);
				if (tempEdge != null)
				{
					this.currentEdge = tempEdge;
					this.currentEdge.IsOccupied = true;
				}
			}

			if(currentEdge is Track)
			{
				
				this.position += distance;

				Track currentTrack = (Track)this.currentEdge;

				if (this.position >= currentTrack.Length)
				//edge switching
				{
					if (this.route.showRoute().Count == this.nextNodeIndex+1)
					//final station reached
					{
						return true;
					}


					Edge tempEdge = this.switchEdge(this.nextNodeIndex+1);

					if (tempEdge != null)
					{
						this.currentEdge.IsOccupied = false;
						this.currentEdge = tempEdge;
						this.currentEdge.IsOccupied = true;

						this.nextNodeIndex++;

						if (this.currentEdge is Track)
						//compute position on new track
						{
							int newDistance = this.position - currentTrack.Length;
							this.position = 0;
							bool finish = this.updatePosition(newDistance);
							if (finish == true)
							{
								return true;
							}
						}
					}
					else
					{
						this.position = currentTrack.Length;
					}
				}
			}
			else if (currentEdge is EndTrack)
			{
				this.currentEdge.IsOccupied = false;
				this.currentEdge = this.switchEdge(this.nextNodeIndex);
				this.currentEdge.IsOccupied = true;
			}

			return false;
		}

		private Edge switchEdge(int nextNodeIndex)
		{
            Contract.Requires(nextNodeIndex < route.showRoute().Count && nextNodeIndex >= 0);
			Edge tempEdge = route.showRoute()[nextNodeIndex-1].getAvailableConnectionTo(route.showRoute()[nextNodeIndex]);

			if (tempEdge == null)
			{
				tempEdge = this.route.showRoute()[nextNodeIndex-1].getAvailableEndTrack();

			}

			return tempEdge;
		}
	}

	public class TrainList<T> : List<T> where T : Train
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
