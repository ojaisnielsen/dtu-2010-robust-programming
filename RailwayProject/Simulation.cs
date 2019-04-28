using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Model;
using TrainSimulation;

namespace RailwayProject
{
	static class Simulation
	{

		private static bool isRunning;

		private static RouteList<Route> routes = new RouteList<Route>();
		private static TrainList<Train> trains = new TrainList<Train>();

		public static bool IsRunning
		{
			get
			{
				return RailwayProject.Simulation.isRunning;
			}
		}

		public static void CleanData()
		{
			Simulation.routes = new RouteList<Route>();
			Simulation.trains = new TrainList<Train>();
		}

		public static void addRoute(String routeId, String[] ids)
		{
            Contract.Requires<ApplicationException>(routeId != null, "Cannot create a route with null id");
            Contract.Requires<ApplicationException>(ids.Length >= 2, "Route has to consist of at least 2 nodes");
            Contract.Requires<ApplicationException>(ids.Contains(null) == false, "Cannot create a routeId - one of the Node ids is null");

			Contract.Requires<ApplicationException>(Program.isValidated == true, "The model needs to be validated before simulating");

            Contract.Requires<ApplicationException>(routes.containsId(routeId) == null, "!A route with the same id already exists!\n");
            //if (routes.containsId(routeId) != null)
            //{
            //    throw new ApplicationException("!The route of id '" + routeId + "' already exists!\n");
            //}

            Contract.Requires<ApplicationException>(ids.Select((string id) => Program.nodes.containsId(id)).All((Node node) => node != null), "!One of the ids doesn't refer to an existing node!\n");

            //Contract.EndContractBlock();


            NodeList<Node> nodeList = new NodeList<Node>();
            ids.ToList().ForEach((string id) => nodeList.Add(Program.nodes.containsId(id)));

            Simulation.routes.Add(new Route(routeId, nodeList, true));

		}

		public static void addTrain(String trainId, int speed, String routeId)
		{
			Contract.Requires<ApplicationException>(trainId != null, "Train id cannot be null");
			Contract.Requires<ApplicationException>(routeId != null, "Route id cannot be null");
			Contract.Requires<ApplicationException>(speed > 0, "Train speed must be greater than 0!");

            Contract.Requires<ApplicationException>(Program.isValidated, "The model needs to be validated before simulating");

            Contract.Requires<ApplicationException>(trains.containsId(trainId) == null, "!A train with this id already exists!\n");
            //if (tempRoute != null)
            //{
            //    throw new ApplicationException("!The train of id '" + trainId + "' already exists!\n");
            //}

            Contract.Requires<ApplicationException>(Simulation.routes.containsId(routeId) != null, "The route does not exist!\n");
            //if (Simulation.routes.containsId(routeId) == null)
            //{
            //    throw new ApplicationException("The route of id '" + routeId + "' does not exist!\n");
            //}
            //Contract.EndContractBlock();

			Route tempRoute = Simulation.routes.containsId(routeId);
            RailwayProject.Simulation.trains.Add(new Train(trainId, tempRoute, speed));

		}

		public static void simulate()
		{
            Contract.Requires<ApplicationException>(RailwayProject.Simulation.trains.Count > 0, "No trains specified in the simulation");

            Contract.Requires<ApplicationException>(Program.isValidated == true, "The model needs to be validated before simulating");

			Simulation.isRunning = true;

			List<Train> tempList = new List<Train>(RailwayProject.Simulation.trains);

			int i=1;
			while (tempList.Count > 0)
			{
				System.Console.WriteLine("\n\nITERATION " + Convert.ToString(i++) + "\n");

				TrainList<Train> trainsToRemove = new TrainList<Train>(); 

				foreach (Train train in tempList)
				{
					bool temp = train.updatePosition(train.Speed);
					if (temp == true)
					{
						trainsToRemove.Add(train);
						System.Console.WriteLine("Train " + train.Id + " has reached the destination!");
						continue;
					}

					if (train.CurrentEdge is UniTrack)
					{
						float currentPosition = (float)train.Position / ((UniTrack)train.CurrentEdge).Length * 100;

						if (train.LastVisitedNode.Equals(((UniTrack)train.CurrentEdge).StartNode))
						{
							System.Console.WriteLine("Train '" + train.Id + "' is on a UniTrack connecting " + ((UniTrack)train.CurrentEdge).StartNode.Id + " and " + ((UniTrack)train.CurrentEdge).EndNode.Id + ", so far it traveled " + currentPosition.ToString() + "% of the distance");
						}
						else
						{
							//ERROR!!!
							throw new ApplicationException("TRAIN SIMULATION MALFUNCTION, UNITRACK");
						}
					}
					else if (train.CurrentEdge is BiTrack)
					{
						float currentPosition = (float)train.Position / ((BiTrack)train.CurrentEdge).Length * 100;

						if (train.LastVisitedNode.Equals(((BiTrack)train.CurrentEdge).FirstNode))
						{
							System.Console.WriteLine("Train '" + train.Id + "' is on a BiTrack connecting " + ((BiTrack)train.CurrentEdge).FirstNode.Id + " and " + ((BiTrack)train.CurrentEdge).SecondNode.Id + ", so far it traveled " + currentPosition.ToString() + "% of the distance");

						}
						else if (train.LastVisitedNode.Equals(((BiTrack)train.CurrentEdge).SecondNode))
						{
							System.Console.WriteLine("Train '" + train.Id + "' is on a BiTrack connecting " + ((BiTrack)train.CurrentEdge).SecondNode.Id + " and " + ((BiTrack)train.CurrentEdge).FirstNode.Id + ", so far it traveled " + currentPosition.ToString() + "% of the distance");
						}
						else
						{
							//ERROR!!!
							throw new ApplicationException("TRAIN SIMULATION MALFUNCTION, BITRACK");
						}
					}
					else if (train.CurrentEdge is EndTrack)
					{
						if (train.LastVisitedNode.Equals(((EndTrack)train.CurrentEdge).Node))
						{
							System.Console.WriteLine("Train '" + train.Id + "' is on an EndTrack belonging to node " + ((EndTrack)train.CurrentEdge).Node.Id);
						}
						else
						{
							//ERROR!!!
							throw new ApplicationException("TRAIN SIMULATION MALFUNCTION, ENDTRACK");
						}
					}
					else
					{
						//ERROR!!!
						throw new ApplicationException("TRAIN SIMULATION OVERLOAD - Train " + train.Id + "couldn't start because no track was available");
					}
				}

				foreach (Train t in trainsToRemove)
				{
					tempList.Remove(t);
				}
			}

			Simulation.isRunning = false;
		}
	}
}
