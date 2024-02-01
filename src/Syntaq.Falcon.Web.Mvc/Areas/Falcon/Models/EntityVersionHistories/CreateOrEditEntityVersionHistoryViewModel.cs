using Syntaq.Falcon.EntityVersionHistories.Dtos;
using System.Collections.Generic;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.EntityVersionHistories
{
    public class CreateOrEditEntityVersionHistoryModalViewModel
    {
        public CreateOrEditEntityVersionHistoryDto EntityVersionHistory { get; set; }

        public string UserName { get; set; }

        public List<EntityVersionHistoryUserLookupTableDto> EntityVersionHistoryUserList { get; set; }

        public bool IsEditMode => EntityVersionHistory.Id.HasValue;
    }
}