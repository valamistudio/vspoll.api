namespace VSPoll.API.Models
{
    public class Paged
    {
        private const int DEFAULT_SIZE = 3;
        private const int MAX_SIZE = 50;

        private int page;
        public int Page
        {
            get => page;
            set
            {
                if (value < 1)
                    page = 1;
                else
                    page = value;
            }
        }

        private int pageSize;
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
