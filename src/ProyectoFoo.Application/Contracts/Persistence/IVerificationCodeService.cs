using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
        public interface IVerificationCodeService
        {
            /// <summary>
            /// Genera y almacena un código de verificación para un propósito específico.
            /// </summary>
            string GenerateCode(int userId, string purpose);

            /// <summary>
            /// Valida un código de verificación para un propósito específico.
            /// </summary>
            bool ValidateCode(int userId, string purpose, string code);

            /// <summary>
            /// Elimina un código de verificación después de su uso.
            /// </summary>
            void RemoveCode(int userId, string purpose);

    }
}
