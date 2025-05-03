using System;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Blockchain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
	public interface IBlockchainRepository
	{
        Task<List<Block>> GetAllBlocks(IDbTransaction transaction = null);
        Task AddBlock(Block newBlock, IDbTransaction transaction = null);
        Task<List<Block>> GetBlockchain();
    }
}

