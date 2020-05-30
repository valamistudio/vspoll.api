namespace VSPoll.API.Models
{
    public abstract class Paged
    {
        private const int DEFAULT_SIZE = 3;
        private const int DEFAULT_PAGE = 1;
        private const int MAX_SIZE = 50;

        private int page = DEFAULT_PAGE;
        public int Page
        {
            get => page;
            set
            {
                if (value < DEFAULT_PAGE)
                    page = DEFAULT_PAGE;
                else
                    page = value;
            }
        }

        private int pageSize = DEFAULT_SIZE;
        public int PageSize
        {
            get => pageSize;
            set
            {
                if (value < 0)
                    pageSize = DEFAULT_SIZE;
                else if (value > MAX_SIZE)
                    pageSize = MAX_SIZE;
                else
                    pageSize = value;
            }
        }
    }
}
