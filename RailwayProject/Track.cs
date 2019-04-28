using System;
using System.Diagnostics.Contracts;

namespace Model
{
	public class Track : Edge
	{
		protected int length;	//length in meters

        [ContractInvariantMethod]
        private void TrackObjectInvariant()
        {
            Contract.Invariant(length > 0);
        }

		public Track(int length) : base()
		{
			this.length = length;
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}
	}	
}