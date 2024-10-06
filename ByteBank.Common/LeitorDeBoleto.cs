using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Common
{
    public class LeitorDeBoleto
    {
        public List<Boleto> LerBoletos(string caminhoArquivo)
        {
            //throw new NotImplementedException();

            // montar lista de boletos
            var boletos = new List<Boleto>();

            // ler arquivo de boletos
            using (var reader = new StreamReader(caminhoArquivo))
            {
                // ler cabeçalho do arquivo CSV
                string linha = reader.ReadLine();
                string[] cabecalho = linha.Split(',');

                // para cada linha do arquivo CSV
                while (!reader.EndOfStream)
                {
                    // ler dados
                    linha = reader.ReadLine();
                    string[] dados = linha.Split(',');

                    // carregar objeto Boleto
                    Boleto boleto = MapearTextoParaObjeto<Boleto>(cabecalho, dados);

                    // adicionar boleto à lista
                    boletos.Add(boleto);
                }
            }

            // retornar lista de boletos
            return boletos;
        }

        private T MapearTextoParaObjeto<T>(string[] nomesPropriedades, string[] valoresPropriedades)
        {
            T instancia = Activator.CreateInstance<T>();

            for(int c = 0; c < nomesPropriedades.Length; c++)
            {
                //Obtém a propriedade atual através do cabeçalho.
                string nomePropriedade = nomesPropriedades[c];
                PropertyInfo propertyInfo = instancia.GetType().GetProperty(nomePropriedade);

                //Verificar se encontrou a propriedade.
                if (propertyInfo != null)
                {
                    //Obtém o tipo da propriedade.
                    Type type = propertyInfo.PropertyType;

                    //Obtém o valor da propriedade.
                    string valor = valoresPropriedades[c];

                    //Converter para o tipo correto.
                    object valorConvertido = Convert.ChangeType(valor, type);

                    //Guardar o valor na propriedade.
                    propertyInfo.SetValue(instancia,valorConvertido);

                }
            }

            return instancia;
        }
    }
}
