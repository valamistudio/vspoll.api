using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input;

public abstract class Paged
{
    private const int DEFAULT_SIZE = 3;
    private const int DEFAULT_PAGE = 1;
    private const int MAX_SIZE = 50;

    private int page = DEFAULT_PAGE;
    [Required]
    public int Page
    {
        get => page;
        set => page = value switch
        {
            < DEFAULT_PAGE => DEFAULT_PAGE,
            _ => value,
        };
    }

    private int pageSize = DEFAULT_SIZE;
    [Required]
    public int PageSize
    {
        get => pageSize;
        set => pageSize = value switch
        {
            < 0 => DEFAULT_SIZE,
            > MAX_SIZE => MAX_SIZE,
            _ => value,
        };
    }
}
