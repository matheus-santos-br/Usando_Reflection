using ByteBank.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace ConsultoriaXYZ.Plugins
{
    public class RelatorioJSON : IRelatorio<Boleto>
    {
        private const string PastaDestino = @"C:\dev\C# Reflection manipule dinamicamente tipos e assemblies\Impressao";

        public RelatorioJSON()
        {
        }

        public void Processar(List<Boleto> boletos)
        {
            var boletosPorCedenteList = PegaBoletosAgrupados(boletos);

            GravarArquivo(boletosPorCedenteList);
        }

        private void GravarArquivo(List<BoletosPorCedente> listaObjetos)
        {
            // Caminho do arquivo JSON
            string caminhoArquivo = Path.Combine(PastaDestino, $"{typeof(BoletosPorCedente).Name}.json");

            // Usar Reflection para obter propriedades do tipo genérico
            PropertyInfo[] objetoProperties = typeof(BoletosPorCedente).GetProperties();

            string json = JsonSerializer.Serialize<List<BoletosPorCedente>>(listaObjetos);

            // Escrever os dados no arquivo JSON
            File.WriteAllText(caminhoArquivo, json);

            Console.WriteLine($"Arquivo '{caminhoArquivo}' criado com sucesso!");
        }

        private List<BoletosPorCedente> PegaBoletosAgrupados(List<Boleto> boletos)
        {
            // Agrupar boletos por cedente
            var boletosAgrupados = boletos.GroupBy(b => new
            {
                b.CedenteNome,
                b.CedenteCpfCnpj,
                b.CedenteAgencia,
                b.CedenteConta
            });

            // Lista para armazenar instâncias de BoletosPorCedente
            List<BoletosPorCedente> boletosPorCedenteList = new List<BoletosPorCedente>();

            // Iterar sobre os grupos de boletos por cedente
            foreach (var grupo in boletosAgrupados)
            {
                // Criar instância de BoletosPorCedente
                BoletosPorCedente boletosPorCedente = new BoletosPorCedente
                {
                    CedenteNome = grupo.Key.CedenteNome,
                    CedenteCpfCnpj = grupo.Key.CedenteCpfCnpj,
                    CedenteAgencia = grupo.Key.CedenteAgencia,
                    CedenteConta = grupo.Key.CedenteConta,
                    Valor = grupo.Sum(b => b.Valor),
                    Quantidade = grupo.Count()
                };

                // Adicionar à lista
                boletosPorCedenteList.Add(boletosPorCedente);
            }

            return boletosPorCedenteList;
        }

    }
}