using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Common.Dto
{
    public class MessageOutput
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }        
    }
}