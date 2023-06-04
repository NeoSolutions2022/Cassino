﻿using Cassino.Application.Dtos.V1.Aposta;
using Cassino.Domain.Entities;

namespace Cassino.Application.Contracts
{
    public interface IApostaService
    {
        void RegistrarAposta(AdicionarApostaDto apostaDto);
        Task<List<VMApostaDto>> ObterApostasDeUsuario(int id);
        Task<List<VMApostaDto>> ObterTodasApostas();
    }
}
