using System.Collections.Generic;
using Abp.Auditing;

namespace Syntaq.Falcon.Auditing
{
    public interface IExpiredAndDeletedAuditLogBackupService
    {
        bool CanBackup();
        
        void Backup(List<AuditLog> auditLogs);
    }
}