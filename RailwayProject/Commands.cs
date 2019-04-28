using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailwayProject
{
	public class Commands {
		// Commands (commandName, numberOfParameters (-1 indicates unlimited parameters available))

		public static readonly Commands NODE = new Commands("NODE", 1);
		public static readonly Commands STATION = new Commands("STATION",2);
		public static readonly Commands CONNECT_UNI = new Commands("CONNECT_UNI",3);
		public static readonly Commands CONNECT_BI = new Commands("CONNECT_BI",3);
		public static readonly Commands CONNECT_DOUBLE_UNI = new Commands("CONNECT_DOUBLE_UNI",3);
		public static readonly Commands ENDTRACK = new Commands("ENDTRACK",1);
		public static readonly Commands LOAD_FILE = new Commands("LOAD_FILE", 1);
        public static readonly Commands VALIDATE = new Commands("VALIDATE", 0);
		public static readonly Commands ROUTE = new Commands("ROUTE", -1);
		public static readonly Commands TRAIN = new Commands("TRAIN", 3);
		public static readonly Commands SIMULATE = new Commands("SIMULATE", 0);
		public static readonly Commands EXIT = new Commands("EXIT", 0);
        public static readonly Commands DRAW = new Commands("DRAW", 1);

		public static readonly String COMMENT_CHAR = "#";


		public static IEnumerable<Commands> Values
		{
			get
			{
				yield return NODE;
				yield return STATION;
				yield return CONNECT_UNI;
				yield return CONNECT_BI;
				yield return CONNECT_DOUBLE_UNI;
				yield return ENDTRACK;
				yield return LOAD_FILE;
                yield return VALIDATE;
				yield return ROUTE;
				yield return TRAIN;
				yield return SIMULATE;
				yield return EXIT;
                yield return DRAW;
			}
		}

		
		private int paramsNum;
		private String command;
		
		/**
		 * 
		 * @param num Number of parameters the command can take (-1 for variable-argument command)
		 * @param state UserStates that are able to perform a command
		 */
		Commands(String commandName, int num){

			this.command = commandName;
			this.paramsNum = num;
		}
		
		/**
		 * 
		 * @return Number of parameters of a given command
		 */
		public int paramsNumber(){ 
			return this.paramsNum;
		}

		public String commandName(){
			return this.command;
		}

		public static Commands valueOf(String p)
		{

			foreach (Commands c in Commands.Values)
			{
				if (c.command == p)
				{
					return c;
				}
			}

			return null;
		}
	}
}
