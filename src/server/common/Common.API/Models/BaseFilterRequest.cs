using Common.Enums;

namespace Common.API.Models;


public abstract class BaseFilterRequest<TOrderBy>
{
    public TOrderBy OrderBy { get; set; }
    public OrderDirection OrderDirection { get; set; }
}