using System;
using System.Net;
using System.Transactions;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;
using SDDB.Domain.DbContexts;
using System.Threading.Tasks;

namespace SDDB.Domain.Services
{
    public class DBLogger : ILogger
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private int dbLoggingLevel;
        private int procTooLongmSec;
        private EFDbContext dbContext;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DBLogger(int dbLogginglevel, int procTooLongmSec, EFDbContext dbContext)
        {
            this.dbLoggingLevel = dbLogginglevel;
            this.procTooLongmSec = procTooLongmSec;
            this.dbContext = dbContext;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void LogResult(DBResult result)
        {
            result.Id = Guid.NewGuid().ToString();
            result.UserName = (String.IsNullOrEmpty(result.UserName)) ? "_unknown_" : result.UserName;
            result.UserHostAddress = (String.IsNullOrEmpty(result.UserHostAddress)) ? "_unknown_" : result.UserHostAddress;
            result.ServiceName = (String.IsNullOrEmpty(result.ServiceName)) ? "_unknown_" : result.ServiceName;

            result.DtStart = (result.DtStart <= DateTime.Parse("1900-01-01")) ? DateTime.Now : result.DtStart;
            result.DtEnd = (result.DtEnd <= DateTime.Parse("1900-01-01")) ? result.DtStart : result.DtEnd;
            var procTime = result.DtEnd - result.DtStart;

            if (procTooLongmSec > 0 && procTime.TotalMilliseconds > procTooLongmSec)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.StatusDescription += String.Format(" Process ran {0:F3} seconds.", procTime.TotalSeconds);
            }

            var saveToDB = false;
            switch (dbLoggingLevel)
            {
                case 0:
                    return;
                case 1:
                    if (result.StatusCode != HttpStatusCode.OK) saveToDB = true;
                    break;
                default:
                    saveToDB = true;
                    break;
            }
            if (saveToDB)
            {
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dbContext.DBResults.Add(result);
                    dbContext.SaveChanges();
                    trans.Complete();
                }
            }
        }
    }
}
