using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Model;

namespace RailwayProject
{

	public class Program
	{
		public static bool isValidated = false;

		public static NodeList<Node> nodes = new NodeList<Node>();
		public static List<Track> tracks = new List<Track>();


		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(nodes != null);
			Contract.Invariant(tracks != null);
		}


        [STAThread]
		static void Main(string[] args)
		{			
			Contract.Requires<ApplicationException>(args.Length <= 1, "Too many parameters.\n");

			String line;
			Boolean loop = true;

			if (args.Length == 1)
			{
				System.Console.WriteLine("Found input file - parsing\n");
				line = "LOAD_FILE " + args[0];
				loop = RailwayProject.Parser.parseLine(line);
			}
			else
			{
				System.Console.WriteLine("No input specified - use 'LOAD_FILE filename' to load data from the file or add data manually.\n");
			}

			while (loop == true)
			{

				System.Console.Write(">> ");
				line = System.Console.ReadLine();
				loop = RailwayProject.Parser.parseLine(line);

			}
		}

		public bool IsValidated
		{
			get
			{
				return Program.isValidated;
			}
		}

		public static Boolean loadFile(string inputFile)
		{
			Contract.Requires<ApplicationException>(inputFile != null, "The file name should not be null.\n");

            TextReader tr;
            try
            {
                tr = new StreamReader(inputFile);
            }
            catch(Exception e)
            {
                throw new ApplicationException("Cannot open input file " + inputFile + "\n");
            }

			String line = null;
			Boolean loop = true;
			while ((line = tr.ReadLine()) != null && loop==true)
			{
				System.Console.WriteLine(line);
				loop = RailwayProject.Parser.parseLine(line);
			}
			tr.Close();

			return loop;
			//Program.isValidated = false;

		}

		private static Boolean validateConnection(Node startNode, Node endNode)
		{
            Contract.Requires(startNode != null && endNode != null);
			NodeList<Node> toBeInvestigatedNodes = new NodeList<Node>();
			NodeList<Node> tempList = new NodeList<Node>();

			NodeList<Node> alreadyCheckedNodes = new NodeList<Node>();

			toBeInvestigatedNodes.Add(startNode);

			while (true)
			{
				foreach (Node sourceNode in toBeInvestigatedNodes)
				{
					foreach (Node node in sourceNode.getReachableNodes())
					{
						if (alreadyCheckedNodes.Contains(node))
						{
							continue;
						}


						if (node.Equals(endNode))
						{
							return true;
						}

						tempList.Add(node);
						alreadyCheckedNodes.Add(node);
					}
				}

				if (tempList.Count == 0)
				{
					return false;
				}

				toBeInvestigatedNodes.Clear();
				toBeInvestigatedNodes.AddRange(tempList);
				tempList.Clear();
			}
		}

		public static void ValidateModel()
		{
            Contract.Requires<ApplicationException>(Program.nodes.All((Node sNode) => Program.nodes.Where((Node endNode) => endNode != sNode).All((Node endNode) => Program.validateConnection(sNode, endNode))), "Model Validation failed!");

            //foreach (Node sNode in Program.nodes)
            //{
            //    foreach (Node endNode in Program.nodes.Where((Node node) => node != sNode))
            //    {                    
                    //if (Program.validateConnection(sNode, endNode) == false)
                    //{                        
                    //    throw new ApplicationException("Model Validation failed -cannot reach node " + endNode.Id + " from " + sNode.Id + " !");
                    //}
            //    }
            //}
            //Contract.EndContractBlock();

            Simulation.CleanData();
            Program.isValidated = true;
        }



        public static void addNode(String id)
        {
			Contract.Requires<ApplicationException>(id != null, "Id cannot be set to null\n");
            Contract.Requires<ApplicationException>(nodes.containsId(id) == null, "!A node with this id already exists!\n");
            //if (nodes.containsId(id) != null)
            //{
            //    throw new ApplicationException("!The node of id '" + id + "' already exists!\n");
            //}
            //Contract.EndContractBlock();
			          
			nodes.Add(new Node(id));
			Program.isValidated = false;
		}

		public static void addStation(String name, String id)
		{
			Contract.Requires<ApplicationException>(id != null, "Station id cannot be set to null\n");
            Contract.Requires<ApplicationException>(name != null, "Station name cannot be set to null\n");
            Contract.Requires<ApplicationException>(nodes.containsId(id) == null && nodes.containsName(name) == null, "!A Station with this id or this name already exists!\n");
            //if (!(nodes.containsId(id) == null && nodes.containsName(name) == null))
            //{
            //    throw new ApplicationException("!The Station of id '" + id + "' or name '" + name + "' already exists!\n");
            //}
            //Contract.EndContractBlock();


			// Station extends Node
			nodes.Add(new Station(id, name));

			Program.isValidated = false;
		}

		public static void addEndTrack(String nodeId)
		{
            Contract.Requires<ApplicationException>(nodeId != null, "No node id specified for adding endtrack\n");
            Contract.Requires<ApplicationException>(nodes.containsId(nodeId) != null, "!There is no node with this id - cannot add an endtrack!\n");
            //if (nodes.containsId(nodeId) == null)
            //{
            //    throw new ApplicationException("!The Node of id '" + nodeId + "' does not exist - cannot add an endtrack!\n");
            //}
            //Contract.EndContractBlock();


			Node node = nodes.containsId(nodeId);

			node.addEndTrack();
		}

		public static void uniTrackConnection(String startNodeId, String endNodeId, String length)
		{
            Contract.Requires<ApplicationException>(startNodeId != null, "Cannot add uniTrack connection - startNodeId is null\n");
            Contract.Requires<ApplicationException>(endNodeId != null, "Cannot add uniTrack connection - endNodeId is null\n");
            Contract.Requires<ApplicationException>(Convert.ToInt32(length) > 0, "Length must be numeric and positive\n");
            Contract.Requires<ApplicationException>(nodes.containsId(startNodeId) != null && nodes.containsId(endNodeId) != null, "!One of the nodes does not exist - cannot set up a connection!\n");
            //if (nodes.containsId(startNodeId) == null || nodes.containsId(endNodeId) == null)
            //{
            //    throw new ApplicationException("One of the nodes does not exist - cannot set up a connection\n");
            //}
            //Contract.EndContractBlock();


			Node startNode = nodes.containsId(startNodeId);
			Node endNode = nodes.containsId(endNodeId);

			// if the nodes were found, add a unitrack connecting startNode with endNode
			UniTrack uniTrack = new UniTrack(startNode, endNode, Convert.ToInt32(length));
			tracks.Add((Track)uniTrack);
			startNode.addTrack((Track)uniTrack);
			endNode.addTrack((Track)uniTrack);

			Program.isValidated = false;

		}

		public static void biTrackConnection(String node1Id, String node2Id, String length)
		{
            Contract.Requires<ApplicationException>(node1Id != null, "Cannot add biTrack connection - node1Id is null\n");
            Contract.Requires<ApplicationException>(node2Id != null, "Cannot add biTrack connection - node2Id is null\n");
            Contract.Requires<ApplicationException>(Convert.ToInt32(length) > 0, "Length must be numeric and positive\n");
            Contract.Requires<ApplicationException>(nodes.containsId(node1Id) != null && nodes.containsId(node2Id) != null, "!One of the nodes does not exist - cannot set up a connection!\n");
            //if (nodes.containsId(node1Id) == null || nodes.containsId(node2Id) == null)
            //{
            //    throw new ApplicationException("One of the nodes does not exist - cannot set up a connection\n");
            //}
            //Contract.EndContractBlock();

			Node node1 = nodes.containsId(node1Id);
			Node node2 = nodes.containsId(node2Id);

			BiTrack biTrack = new BiTrack(node1, node2, Convert.ToInt32(length));
			tracks.Add(biTrack);
			node1.addTrack((Track)biTrack);
			node2.addTrack((Track)biTrack);

			Program.isValidated = false;
		}

		public static void uniTrackDoubleConnection(String node1Id, String node2Id, String length)
		{
			// Setting two uni-direction tracks in both directions

            Contract.Requires<ApplicationException>(node1Id != null, "Cannot add uniTrack double connection - node1Id is null\n");
            Contract.Requires<ApplicationException>(node2Id != null, "Cannot add uniTrack double connection - node2Id is null\n");
			Contract.Requires<ApplicationException>(Convert.ToInt32(length) > 0, "Length must be numeric and positive\n");
            Contract.Requires<ApplicationException>(nodes.containsId(node1Id) != null && nodes.containsId(node2Id) != null, "!One of the nodes does not exist - cannot set up a connection!\n");
            //if (nodes.containsId(node1Id) == null || nodes.containsId(node2Id) == null)
            //{
            //    throw new ApplicationException("One of the nodes does not exist - cannot set up a connection\n");
            //}
            //Contract.EndContractBlock();

			Node node1 = nodes.containsId(node1Id);
			Node node2 = nodes.containsId(node2Id);

            UniTrack uniTrack1 = new UniTrack(node1, node2, Convert.ToInt32(length));
            UniTrack uniTrack2 = new UniTrack(node2, node1, Convert.ToInt32(length));

            tracks.Add(uniTrack1);
            node1.addTrack((Track)uniTrack1);
            node2.addTrack((Track)uniTrack1);

            tracks.Add(uniTrack2);
            node1.addTrack((Track)uniTrack2);
            node2.addTrack((Track)uniTrack2);

			Program.isValidated = false;
		}
	}
}