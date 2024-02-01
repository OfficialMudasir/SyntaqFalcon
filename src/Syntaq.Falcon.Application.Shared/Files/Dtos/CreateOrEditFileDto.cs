using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Files.Dtos
{
    public class SaveFileDto //: EntityDto<long?>
    {
        public Guid RecordId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemGroupId { get; set; }
        public FileDto File { get; set; }
        public string AccessToken { get; set; }
    }

    public class CreateOrEditFileDto
    {
        public string dir { get; set; }
        public FileDto file { get; set; }
        public string name { get; set; }
    }

    public class FileDto
    {
        public bool File { get; set; }
        public string Name { get; set; }
        public Guid Token { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string OriginalName { get; set; }
    }
}