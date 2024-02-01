
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetFormForPopupFormView
    {
        public Guid? FormId { get; set; }
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// When set will load the form from a release package
        /// </summary>
        public Guid? ReleaseId { get; set; }
        public string SyntaqAuthToken { get; set; }
        public string TenantName { get; set; }

    }
}
