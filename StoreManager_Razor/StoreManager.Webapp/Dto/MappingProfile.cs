using System;
using AutoMapper;
using StoreManager.Application.Model;

namespace StoreManager.Webapp.Dto
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{

			CreateMap<StoreDto, Store>(); // Wir mappen von StoreDto auf Store | POST
            CreateMap<Store, StoreDto>(); // Wir mappen von Store auf StoreDto | GET
        }
	}
}

