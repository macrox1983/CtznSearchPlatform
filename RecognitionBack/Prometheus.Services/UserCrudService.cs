using AutoMapper;
using Prometheus.Infrastructure.RepositoryBase;
using Prometheus.Model;
using Prometheus.Presentation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Services
{
    public class UserCrudService : IEntityCrudService<ApplicationUserVm, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserCrudService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ApplicationUserVm> Create(ApplicationUserVm viewEntity)
        {

            return _mapper.Map<ApplicationUserVm>(await _userRepository.Add(_mapper.Map<ApplicationUser>(viewEntity)));
        }

        public async Task<bool> Delete(ApplicationUserVm viewEntity)
        {
            return await _userRepository.Delete(_mapper.Map<ApplicationUser>(viewEntity));
        }

        public async Task<List<ApplicationUserVm>> GetAll()
        {
            var users = await _userRepository.FindAllAsync();
            return _mapper.Map<List<ApplicationUserVm>>(users);
        }

        public async Task<ApplicationUserVm> Update(ApplicationUserVm viewEntity)
        {
            return _mapper.Map<ApplicationUserVm>(await _userRepository.Update(_mapper.Map<ApplicationUser>(viewEntity)));
        }
        public async Task<ApplicationUserVm> Find(Guid guid)
        {
            return _mapper.Map<ApplicationUserVm>(await _userRepository.FindBy(guid));
        }

        public async Task<Guid> FindByName(string name)
        {
            var applicationUser = await _userRepository.FindByName(name);
            if (applicationUser == null)
                return Guid.Empty;
            return applicationUser.UserId;
        }
    }
}
