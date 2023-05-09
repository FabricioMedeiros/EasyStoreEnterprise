using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.Clients.API.Models
{
    public interface IClientRepository : IRepository<Client>
    {
        void Add(Client client);

        Task<IEnumerable<Client>> GetAll();
        Task<Client> GetByCpf(string cpf);
        Task<Address> GetAddressById(Guid id);
        void AddAddress(Address address);
    }
}
