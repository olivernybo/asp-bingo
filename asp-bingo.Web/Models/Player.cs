using System;

namespace asp_bingo.Web.Models
{
    public class Player
    {
		public string Name { get; set; }
		public string Class { get; set; }
		public string Color { get; set; }
		public int[] Sheet { get; set; }
		public bool IsBanned { get; set; } = false;
    }
}
