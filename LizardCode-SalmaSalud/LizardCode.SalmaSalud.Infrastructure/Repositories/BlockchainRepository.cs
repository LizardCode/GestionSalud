using System;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Blockchain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using DapperQueryBuilder;
using Dapper;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
	public class BlockchainRepository : IBlockchainRepository
    {
        private readonly IDbContext _context;

        public BlockchainRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<List<Block>> GetAllBlocks(IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                    .QueryBuilder($@" SELECT * FROM Blocks");

            var result = await builder.QueryAsync<Block>(transaction);

            return result.AsList();
        }

        public async Task AddBlock(Block newBlock, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO Blocks
                    (
                        [Index],
                        [Timestamp],
                        [Data],
                        PreviousHash,
                        Hash
                    )
                    VALUES
                    (
                        {newBlock.Index},
                        {newBlock.Timestamp},
                        {newBlock.Data},
                        {newBlock.PreviousHash},
                        {newBlock.Hash}

                    )");

            var results = await builder.ExecuteAsync(transaction);

        }


        public Task<List<Block>> GetBlockchain()
        {
            throw new NotImplementedException();
        }
    }
}

