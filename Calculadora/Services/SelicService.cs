

using CalculadoraDebitos.Data;
using CalculadoraDebitos.DTO;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraDebitos.Services
{
    public class SelicService
    {
        private readonly AppDbContext _context;

        public SelicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(decimal fator, List<ItemSelic> memoria)> CalcularSelic(
            decimal baseCalculo,
            DateTime vencimento,
            DateTime pagamento)
        {
            var inicio = new DateTime(vencimento.Year, vencimento.Month, 1).AddMonths(1);
            //Não inclui mês de pagamento
            //var fim = new DateTime(pagamento.Year, pagamento.Month, 1).AddMonths(-1);
            //inclui mês de pagamento
            var fim = new DateTime(pagamento.Year, pagamento.Month, 1);

            if (inicio > fim)
                return (1m, new List<ItemSelic>());

            var taxas = await _context.SelicMensal
                .Where(x =>
                    x.Ano >= 2026 && // 👈 SELIC só a partir de 2026
                    (x.Ano > inicio.Year || (x.Ano == inicio.Year && x.Mes >= inicio.Month)) &&
                    (x.Ano < fim.Year || (x.Ano == fim.Year && x.Mes <= fim.Month)))
                .OrderBy(x => x.Ano)
                .ThenBy(x => x.Mes)
                .ToListAsync();

            decimal fator = 0m;
            decimal baseAtual = baseCalculo;

            var memoria = new List<ItemSelic>();

            foreach (var taxa in taxas)
            {
                decimal valor = baseAtual * (taxa.PercentualMensal / 100m);

                memoria.Add(new ItemSelic
                {
                    Ano = taxa.Ano,
                    Mes = taxa.Mes,
                    Taxa = taxa.PercentualMensal,
                    Base = baseAtual,
                    // Valor = valor
                    Valor = valor // / 100
                });

                // baseAtual += valor;
               // baseAtual = valor;

                //fator *= (1 + (taxa.PercentualMensal / 100m));
                fator += ((taxa.PercentualMensal / 100m));
            }

            return (fator, memoria);
        }
    }
}




/*  --Primeira Versao
using CalculadoraDebitos.Data;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraDebitos.Services
{
    public class SelicService
    {
        private readonly AppDbContext _context;

        public SelicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalcularSelic(
            DateTime dataInicio,
            DateTime dataFim)
        {
            var inicio = new DateTime(dataInicio.Year, dataInicio.Month, 1);
            var fim = new DateTime(dataFim.Year, dataFim.Month, 1);

            var taxas = await _context.SelicMensal
                .Where(x => x.Competencia >= inicio && x.Competencia <= fim)
                .OrderBy(x => x.Competencia)
                .ToListAsync();

            decimal fator = 1m;

            foreach (var taxa in taxas)
            {
                fator *= (1 + (taxa.PercentualMensal / 100m));
            }

            return fator - 1;
        }
    }
}
*/