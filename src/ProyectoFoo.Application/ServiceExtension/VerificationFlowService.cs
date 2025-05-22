using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class VerificationFlowService : IVerificationFlowService
    {
        private readonly IVerificationCodeService _codeService;
        private readonly IEmailService _emailService;

        public VerificationFlowService(IVerificationCodeService codeService, IEmailService emailService)
        {
            _codeService = codeService;
            _emailService = emailService;
        }

        public async Task<bool> SendVerificationCodeAsync(int userId, string email, string purpose, string subject, string bodyTemplate)
        {
            var code = _codeService.GenerateCode(userId, purpose);
            var body = string.Format(bodyTemplate, code);
            await _emailService.SendEmailAsync(email, subject, body);
            return true;
        }

        public bool ValidateAndRemoveCode(int userId, string purpose, string code)
        {
            var valid = _codeService.ValidateCode(userId, purpose, code);
            if (valid)
                _codeService.RemoveCode(userId, purpose);
            return valid;
        }
    }
}
