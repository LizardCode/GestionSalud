using System.Data;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Blockchain;

namespace LizardCode.SalmaSalud.Application.Interfaces.Services
{
    public interface IBlockchainService
    {
        Task AddBlock(Block newBlock, IDbTransaction transaction = null);
        Task<Block> GetLatestBlock(IDbTransaction transaction = null);
        Task<bool> IsChainValid();
        Task DisplayBlockchainAsync();
    }
}