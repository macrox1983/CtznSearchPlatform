using Prometheus.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Prometheus.Services
{
    public class DictionaryAggregateCrudService : IDictionaryAggregateCrudService
    {
        
        private readonly IEntityCrudService<CityVm, Guid> _cityCrudService;
        private readonly IEntityCrudService<RoleVm, Guid> _roleCrudService;
        private readonly IEntityCrudService<ApplicationUserVm, Guid> _userCrudService;

        public DictionaryAggregateCrudService(
            
            IEntityCrudService<CityVm, Guid> cityCrudService,           
            IEntityCrudService<RoleVm, Guid> roleCrudService,            
            IEntityCrudService<ApplicationUserVm, Guid> userCrudService
            )
        {
           
            _cityCrudService = cityCrudService ?? throw new ArgumentNullException(nameof(cityCrudService));
            
            _roleCrudService = roleCrudService ?? throw new ArgumentNullException(nameof(roleCrudService));
            
            _userCrudService = userCrudService ?? throw new ArgumentNullException(nameof(userCrudService));
        }
               
        #region Create
       
        public async Task<RoleVm> Create(RoleVm viewEntity)
        {
            return await _roleCrudService.Create(viewEntity);
        }
       
        public async Task<CityVm> Create(CityVm viewEntity)
        {
            return await _cityCrudService.Create(viewEntity);
        }
        

      
        public async Task<ApplicationUserVm> Create(ApplicationUserVm viewEntity)
        {
            return await _userCrudService.Create(viewEntity);
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(RoleVm viewEntity)
        {
            return await _roleCrudService.Delete(viewEntity);
        }
        public async Task<bool> Delete(CityVm viewEntity)
        {
            return await _cityCrudService.Delete(viewEntity);
        }

        public async Task<bool> Delete(ApplicationUserVm viewEntity)
        {
            return await _userCrudService.Delete(viewEntity);
        }
        #endregion

        #region Update

        public async Task<RoleVm> Update(RoleVm viewEntity)
        {
            return await _roleCrudService.Update(viewEntity);
        }
        public async Task<CityVm> Update(CityVm viewEntity)
        {
            return await _cityCrudService.Update(viewEntity);
        }

        public async Task<ApplicationUserVm> Update(ApplicationUserVm viewEntity)
        {
            return await _userCrudService.Update(viewEntity);
        }
        #endregion

        #region GetAll

        public async Task<List<TEntity>> GetAll<TEntity, TKey>()
        {
            return await ((IEntityCrudService<TEntity, TKey>)this).GetAll();
        }

       

        async Task<List<CityVm>> IEntityCrudService<CityVm, Guid>.GetAll()
        {
            return await _cityCrudService.GetAll();
        }

        async Task<List<RoleVm>> IEntityCrudService<RoleVm, Guid>.GetAll()
        {
            return await _roleCrudService.GetAll();
        }

        async Task<List<ApplicationUserVm>> IEntityCrudService<ApplicationUserVm, Guid>.GetAll()
        {
            return await _userCrudService.GetAll();
        }
        #endregion

        #region Find
        public async Task<TEntity> Find<TEntity, TKey>(TKey id)
        {
            return await ((IEntityCrudService<TEntity, TKey>)this).Find(id);
        }
      

       
        async Task<CityVm> IEntityCrudService<CityVm, Guid>.Find(Guid guid)
        {
            return await _cityCrudService.Find(guid);
        }

     

        async Task<RoleVm> IEntityCrudService<RoleVm, Guid>.Find(Guid guid)
        {
            return await _roleCrudService.Find(guid);
        }

      
        async Task<ApplicationUserVm> IEntityCrudService<ApplicationUserVm, Guid>.Find(Guid guid)
        {
            return await _userCrudService.Find(guid);
        }
        #endregion
        
        public List<DictionaryVm> GetDictionaries()
        {
            return new List<DictionaryVm>(new[]
            {
                new DictionaryVm{ Name = "Пользователи", Desription = "Справочник пользователей системы", ActionName="ApplicationUsers"},
                new DictionaryVm{ Name = "Роли", Desription = "Справочник ролей пользователя", ActionName="Roles"},
                new DictionaryVm{ Name = "Департаменты", Desription = "Справочник департаментов", ActionName="Departments"},

                new DictionaryVm{ Name = "Модели самолетов", Desription = "Справочник моделей самолетов", ActionName="AircraftModels"},
                new DictionaryVm{ Name = "Статусы рейсов", Desription = "Справочник статусов рейсов", ActionName="FlightStatuses"},
                new DictionaryVm{ Name = "Государства", Desription = "Справочник государств", ActionName="States"},
                new DictionaryVm{ Name = "Города", Desription = "Справочник городов", ActionName="Cities"},
                new DictionaryVm{ Name = "Аэропорты", Desription = "Справочник аэропортов", ActionName="Airports"},
                new DictionaryVm{ Name = "Авиакомпании", Desription = "Справочник авиакомпаний", ActionName="Airlines"},
                new DictionaryVm{ Name = "Борт", Desription = "Справочник бортов", ActionName="Aircrafts"},

                new DictionaryVm{ Name = "Статусы багажа", Desription = "Справочник статусов багажа", ActionName="BaggageStatuses"},
                new DictionaryVm{ Name = "Типы багажа", Desription = "Справочник типов багажа", ActionName="BaggageTypes"},

                new DictionaryVm{ Name = "Статусы пассажиров", Desription = "Справочник статусов пассажиров", ActionName="PassengerStatuses"},
                new DictionaryVm{ Name = "Типы документов", Desription = "Справочник типов документов", ActionName="DocumentTypes"},
                new DictionaryVm{ Name = "Классы обслуживания", Desription = "Справочник классов обслуживания", ActionName="ServiceClasses"},
            });
        }


     

        #region FindById
        public Task<TEntity> FindById<TEntity, TKey>(string name)
        {
            throw new NotImplementedException();
        }

       

       
        async Task<Guid> IEntityCrudService<CityVm, Guid>.FindByName(string name)
        {
            return await _cityCrudService.FindByName(name);
        }

     


        async Task<Guid> IEntityCrudService<RoleVm, Guid>.FindByName(string name)
        {
            return await _roleCrudService.FindByName(name);
        }


        async Task<Guid> IEntityCrudService<ApplicationUserVm, Guid>.FindByName(string name)
        {
            return await _userCrudService.FindByName(name);
        }
        #endregion
    }
}



/*Очестка DELETE FROM State
	WHERE not (StateCode = '000' OR StateId = '9D49B0C7-AE90-495C-B149-9A6AB661C66F' )*/
