using System;

namespace VSPoll.API.Models
{
    public class VotersQuery : Paged
    {
        public Guid Option { get; set; }
    }
}
