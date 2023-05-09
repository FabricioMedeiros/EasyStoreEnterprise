using ESE.Clients.API.Models;
using ESE.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.Clients.API.Data.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientDbContext _context;

        public ClientRepository(ClientDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Client>> GetAll()
        {
            return await _context.Clients.AsNoTracking().ToListAsync();
        }

        public Task<Client> GetByCpf(string cpf)
        {
            return _context.Clients.FirstOrDefaultAsync(c => c.Cpf.Number == cpf);
        }

        public void Add(Client client)
        {
            _context.Clients.Add(client);
        }

        public async Task<Address> GetAddressById(Guid id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(e => e.ClientId == id);
        }

        public void AddAddress(Address address)
        {
            _context.Addresses.Add(address);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
