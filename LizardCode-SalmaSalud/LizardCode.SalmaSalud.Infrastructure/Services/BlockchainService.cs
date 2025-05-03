using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Blockchain;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Data;

namespace LizardCode.SalmaSalud.Infrastructure.Services
{

    public class BlockchainService : IBlockchainService
    {
        private readonly IBlockchainRepository _blockchainRepository;

        public BlockchainService(IBlockchainRepository blockchainRepository)
        {
            _blockchainRepository = blockchainRepository;
        }


        public async Task <Block> GetLatestBlock(IDbTransaction transaction = null)
        {
            var blocks = await  _blockchainRepository.GetAllBlocks(transaction);
            return blocks.LastOrDefault();
        }

        public async Task AddBlock(Block newBlock, IDbTransaction transaction = null)
        {
            var latest = await GetLatestBlock(transaction);
            var genesis = default(Block);

            if (latest == null)
            {
                genesis = CreateGenesisBlock();
                genesis.Hash = CalculateHash(genesis);
                await _blockchainRepository.AddBlock(genesis, transaction);
            }
            else {
                newBlock.Index = latest?.Index ?? genesis.Index;
                newBlock.Index++;
                newBlock.PreviousHash = latest?.Hash ?? genesis.Hash;
                newBlock.Timestamp = DateTime.Now;
                
                newBlock.Hash = CalculateHash(newBlock);
                await _blockchainRepository.AddBlock(newBlock, transaction);
            }

        }

        public async Task<bool> IsChainValid()
        {
            var blocks = await  _blockchainRepository.GetAllBlocks();

            for (int i = 1; i < blocks.Count; i++)
            {
                Block currentBlock = blocks[i];
                Block previousBlock = blocks[i - 1];

                if (currentBlock.Hash != CalculateHash(currentBlock))
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task DisplayBlockchainAsync()
        {
            var blocks = await _blockchainRepository.GetBlockchain();
            foreach (Block block in blocks)
            {
                Console.WriteLine("Index: " + block.Index);
                Console.WriteLine("Timestamp: " + block.Timestamp);
                Console.WriteLine("Data: " + block.Data);
                Console.WriteLine("Previous Hash: " + block.PreviousHash);
                Console.WriteLine("Hash: " + block.Hash + "\n");
            }
        }

        private Block CreateGenesisBlock()
        {
            return new Block { Index = 0, Timestamp = DateTime.Now, Data = "Genesis Block", PreviousHash = null };
        }


        private string CalculateHash(Block block)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes($"{block.Index}{block.Timestamp}{block.PreviousHash}{JsonConvert.SerializeObject(block.Data)}");
                var hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

    }
}

