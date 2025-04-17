using Common.Enums;

namespace Common.Models
{
	public abstract class BaseFilter<TOrder>
	{
		public TOrder OrderBy { get; set; }
		public OrderDirection OrderDirection { get; set; }
	}
}
