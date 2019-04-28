using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Model;
using GraphDisplay;

namespace RailwayProject
{
	static class Parser
	{

		public static Boolean parseLine(String line) {
            Contract.Requires(line != null);
		
			Commands com;

			if (line.StartsWith(Commands.COMMENT_CHAR))
			{
				//System.Console.WriteLine("Commented line incomming - skipping");
				return true;
			}

			string[] command;
			command = line.Split(" ".ToCharArray());
			
			
			if((com = Commands.valueOf(command[0])) == null){
				if (!(command[0].Equals("")) || command.Length != 1)
				{
					System.Console.WriteLine("Unknown command");
				}
				return true;
			}
			
				
			if(com.paramsNumber() == command.Length-1 || com.paramsNumber() == -1)
			{
				if (com.Equals(Commands.NODE))
				{
					// NODE id

					//addNode(id)
                    try
                    {
						Program.addNode(command[1]);

                    }
                    catch (ApplicationException e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
				}

				else if (com.Equals(Commands.STATION))
				{
					// STATION name id

					//addStation(name, id)
					try
					{
						Program.addStation(command[1], command[2]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}


				else if (com.Equals(Commands.CONNECT_UNI))
				{
					// CONNECT_UNI startNodeId endNodeId

					// uniTrackConnection startNodeId endNodeId
					try
					{
						Program.uniTrackConnection(command[1], command[2], command[3]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}

				else if (com.Equals(Commands.CONNECT_BI))
				{
					// CONNECT_BI node1Id node2Id

					// biTrackConnection node1Id node2Id
					try
					{
						Program.biTrackConnection(command[1], command[2], command[3]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}

                else if (com.Equals(Commands.CONNECT_DOUBLE_UNI))
                {
					// CONNECT_DOUBLE_UNI startNodeId endNodeId

					// uniTrackDoubleConnection startNodeId endNodeId
					try
					{
						Program.uniTrackDoubleConnection(command[1], command[2], command[3]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}

				else if (com.Equals(Commands.ENDTRACK))
				{
					//ENDTRACK nodeId


					/* we need to think it over again, I mean implementation of stations and nodes
					 * and make it work good with the parses
					 * Maciek
					 */

					//addEndTrack nodeId
					try
					{
						Program.addEndTrack(command[1]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}

				else if (com.Equals(Commands.LOAD_FILE))
				{
					try
					{
					    return Program.loadFile(command[1]);
                    }
                    catch (ApplicationException e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
				}

				else if (com.Equals(Commands.VALIDATE))
				{
					try
					{
						Program.ValidateModel();
						System.Console.WriteLine("Model Validation completed - no errors");
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}


				}

				else if (com.Equals(Commands.ROUTE))
				{
					String[] nodes = new String[command.Length-2];
					for(int i=0;i<=nodes.Length-1;i++)
					{
						nodes[i] = command[i+2];
					}

					try
					{
						Simulation.addRoute(command[1], nodes);
						
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}

				}
				else if (com.Equals(Commands.TRAIN))
				{
					try
					{
						Simulation.addTrain(command[1], Convert.ToInt32(command[2]), command[3]);
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}
				}

				else if (com.Equals(Commands.SIMULATE))
				{
					try
					{
						Simulation.simulate();
						System.Console.WriteLine("\nTrain simulation completed - no errors");
					}
					catch (ApplicationException e)
					{
						System.Console.WriteLine(e.Message);
					}
				}

				else if (com.Equals(Commands.EXIT))
				{

					return false;

				}

                else if (com.Equals(Commands.DRAW))
                {
                    try
                    {
                        Display.Draw(command[1]);
                    }
                    catch (ApplicationException e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                }
			}
			else
			{
				System.Console.WriteLine("!Unknown parameters for command: "+command[0]);
			}
					
				
			return true;
		}
	}
}
