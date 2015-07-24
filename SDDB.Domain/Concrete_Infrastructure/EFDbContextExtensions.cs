using System;
using System.Threading.Tasks;

using SDDB.Domain.DbContexts;

namespace SDDB.Domain.Infrastructure
{
    //IDbEntityExtensions------------------------------------------------------------------------------------------------------//
    public static class EFDbContextExtensions
    {
        //attempt to save changes to DBContext, retry if exception on deadlock thrown
        public static async Task SaveChangesWithRetryAsync(this EFDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                try
                {
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    break;
                }
                catch (Exception e)
                {
                    if (i == 10 || !e.GetBaseException().Message.Contains("Deadlock")) { throw; }
                }
                await Task.Delay(200).ConfigureAwait(false);
            }
        }
    }
}
