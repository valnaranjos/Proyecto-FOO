using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IVerificationFlowService
    {
        Task<bool> SendVerificationCodeAsync(int userId, string email, string purpose, string subject, string bodyTemplate);
        bool ValidateAndRemoveCode(int userId, string purpose, string code);
    }
}
