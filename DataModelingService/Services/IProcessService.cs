using System;
using System.Threading.Tasks;
using Refit;
using DTO;
namespace DataModelingService.Services
{
    public interface IProcessService
    {
        [Post("/Token")]
        Task<TokenResult> GetTokenAsync(LoginViewModel token);


    }
}
