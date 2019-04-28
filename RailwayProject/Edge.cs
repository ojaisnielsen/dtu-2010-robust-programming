using System;


namespace Model
{

	public abstract class Edge
	{
		protected bool isOccupied;
		
		public Edge()
		{
			this.isOccupied = false;
		}
		
		public bool IsOccupied
		{
			get
			{
				return  this.isOccupied;
			}
			set
			{
				this.isOccupied = value;
			}
		}
	}
}
