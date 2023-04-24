using Cassino.Application.Dtos.V1.Saldo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassino.Application.Contracts
{
    public interface ISaldoService
    {
        Task<SaldoUsuarioDto> BuscarSaldo(int id);
        Task<Boolean> AtualizarSaldo(SaldoUsuarioDto saldoUsuarioDto);
    }
}
