using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{

    public class DraftInput
    {
        public Guid Id { get; set; }

        public bool Draft { get; set; }

    }

    public class PublishInput 
    {
		public Guid Id { get; set; }

		public bool Publish { get; set; }
		 
    }

    public class FinaliseInput 
    {
        public Guid Id { get; set; }

        public bool Finalise { get; set; }

    }

    public class FinaliseUnlockInput
    {
        public Guid Id { get; set; }

        public bool FinaliseUnlock { get; set; }

    }
}