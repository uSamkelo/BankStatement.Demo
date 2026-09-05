using AutoMapper;
using BankStatement.Demo.DTOs;
using BankStatement.Demo.Entities;

namespace BankStatement.Demo.Mapping
{
    public class BankStatementProfile : Profile
    {
        public BankStatementProfile()
        {
            CreateMap<BankStatementEntity, BankStatementDto>();
            CreateMap<BankStatementDto, BankStatementEntity>();
        }
    }
}
