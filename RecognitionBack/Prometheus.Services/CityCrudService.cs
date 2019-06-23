using Prometheus.Presentation;
using Prometheus.Infrastructure.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Prometheus.Model;
using System.Collections;
using System.Linq;

namespace Prometheus.Services
{
    public class CityCrudService : IEntityCrudService<CityVm, Guid>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityCrudService(ICityRepository cityRepository, IMapper mapper )
        {
            _cityRepository = cityRepository ?? throw new ArgumentException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        public async Task<CityVm> Create(CityVm viewEntity)
        {
            return _mapper.Map<CityVm>(await _cityRepository.Add(_mapper.Map<City>(viewEntity)));
        }

        public async Task<bool> Delete(CityVm viewEntity)
        {
            return await _cityRepository.Delete(_mapper.Map<City>(viewEntity));
        }

        public async Task<List<CityVm>> GetAll()
        {
            return _mapper.Map<List<CityVm>>(await _cityRepository.FindAllAsync());
        }

        public async Task<CityVm> Update(CityVm viewEntity)
        {
            return _mapper.Map<CityVm>(await _cityRepository.Update(_mapper.Map<City>(viewEntity)));
        }
        public async Task<CityVm> Find(Guid guid)
        {
            return _mapper.Map<CityVm>(await _cityRepository.FindBy(guid));
        }



        public async Task CreateFromExcelItem(object item)
        {
                     
        }

        public async Task<Guid> FindByName(string name)
        {
            var city = await _cityRepository.FindByName(name);
            if (city == null)
                return Guid.Empty;
            return city.CityId;
        }
    }
}
