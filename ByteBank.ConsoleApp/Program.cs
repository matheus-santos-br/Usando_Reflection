﻿using ByteBank.Common;
using System.Reflection;

MostrarBanner();

while (true)
{
    MostrarMenu();

    if (int.TryParse(Console.ReadLine(), out int escolha))
    {
        ExecutarEscolha(escolha);
    }
    else
    {
        Console.WriteLine("Opção inválida. Tente novamente.");
    }
}

static void MostrarBanner()
{
    Console.WriteLine(@"


    ____        __       ____              __      
   / __ )__  __/ /____  / __ )____ _____  / /__    
  / __  / / / / __/ _ \/ __  / __ `/ __ \/ //_/    
 / /_/ / /_/ / /_/  __/ /_/ / /_/ / / / / ,<       
/_____/\__, /\__/\___/_____/\__,_/_/ /_/_/|_|      
      /____/                                       
                                
        ");
}

static void MostrarMenu()
{
    Console.WriteLine("\nEscolha uma opção:");
    Console.WriteLine();
    Console.WriteLine("1. Ler arquivo de boletos.");
    Console.WriteLine("2. Gerar arquivo com boletos agrupados por cedente.");
    Console.WriteLine("3. Executar Plugins.");
    Console.WriteLine();
    Console.Write("Digite o número da opção desejada: ");
}

static void ExecutarEscolha(int escolha)
{
    switch (escolha)
    {
        case 1:
            LerArquivoBoletos();
            break;
        case 2: GravarGrupoBoletos();
            break;
        case 3:
            ExecutarPlugins();
            break;
   
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}

static void LerArquivoBoletos()
{
    Console.WriteLine("Lendo arquivo de boletos...");

    var leitorDeBoleto = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeBoleto.LerBoletos("Boletos.csv");

    foreach (var boleto in boletos)
    {
        Console.WriteLine($"Cedente: {boleto.CedenteNome}, Valor: {boleto.Valor:#0.00}, Vencimento: {boleto.DataVencimento}");
    }
}

static void GravarGrupoBoletos()
{
    Console.WriteLine("Gerando arquivos de boletos...");

    var leitorDeBoleto = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeBoleto.LerBoletos("Boletos.csv");

    //RelatorioDeBoleto gravadorDeCSV = new RelatorioDeBoleto("BoletosPorCedente.csv");
    //gravadorDeCSV.Processar(boletos);

    var nomeParametroConstrutor = "nomeArquivoSaida";
    var parametroConstrutor = "BoletosPorCedente.csv";
    var nomeMetodo = "Processar";
    var parametroMetodo = boletos;

    ProcessarDinamicamente(nomeParametroConstrutor, parametroConstrutor, nomeMetodo, parametroMetodo);
}

static void ProcessarDinamicamente(string nomeParametroConstrutor, string parametroConstrutor, string nomeMetodo, List<Boleto> parametroMetodo)
{
    var tipoClasseRelatorio = typeof(RelatorioDeBoleto);
    var construtores = tipoClasseRelatorio.GetConstructors();

    var construtor = construtores.Single(c => c.GetParameters().Length == 1 
                                         && c.GetParameters().Any(p => p.Name == nomeParametroConstrutor));

    //Cria uma instância da classe RelatorioDeBoleto.
    var instanciaClasse = construtor.Invoke(new object[] { parametroConstrutor });

    var metodoProcessar = tipoClasseRelatorio.GetMethod(nomeMetodo);

    //Realiza a chamada do método Processar.
    metodoProcessar.Invoke(instanciaClasse, new object[] {parametroMetodo});
}

static void ExecutarPlugins()
{
    //Ler boletos a partir do arquivo CSV
    var leitorDeCSV = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeCSV.LerBoletos("Boletos.csv");

    //Obter classes de plugin 
    List<Type> classesDePlugin = ObterClassesDePlugin<IRelatorio<Boleto>>();

    foreach (var classe in classesDePlugin)
    {
        // Criar uma instância do plugin
        //var plugin = Activator.CreateInstance(classe, new object[] { "BoletosPorCedente.csv" });
        var plugin = Activator.CreateInstance(classe);

        // Chamar o método Processar usando Reflection
        MethodInfo metodoSalvar = classe.GetMethod("Processar");
        metodoSalvar.Invoke(plugin, new object[] { boletos });
    }
}
static List<Type> ObterClassesDePlugin<T>()
{
    var tiposEncontrados = new List<Type>();

    //Pegar assembly em execução.
    //Assembly assemblyExec = Assembly.GetExecutingAssembly();

    //Assembly onde um tipo é declarado.
    //Assembly assemblyPlugins = typeof(T).Assembly;

    //Assembly onde um tipo é declarado.
    var assemblies = ObterAssembliesDePlugins();

    foreach (var assembly in assemblies)
    {
        Console.WriteLine($@"Assembly encontrado: {assembly.FullName}");

        List<Type> tiposImpInterfaceT = ObterTiposDoAssembly<T>(assembly);

        tiposEncontrados.AddRange(tiposImpInterfaceT);
    }

    return tiposEncontrados;

}

static List<Type> ObterTiposDoAssembly<T>(Assembly assemblyPlugins)
{
    //Descobrir tipos do assembly.
    var tipos = assemblyPlugins.GetTypes();

    //Tipos que implementam interface.
    List<Type> tiposImpInterfaceT = tipos.Where(tipo =>
                                          typeof(T).IsAssignableFrom(tipo)
                                          && tipo.IsClass
                                          && !tipo.IsAbstract).ToList();
    return tiposImpInterfaceT;
}

static List<Assembly> ObterAssembliesDePlugins()
{
    var assemblies = new List<Assembly>();

    const string diretorio = $@"C:\dev\C# Reflection manipule dinamicamente tipos e assemblies\Plugins";

    //Obter os arquivos .dll da pasta.
    string[] arquivosDll = Directory.GetFiles(diretorio,"*.dll");

    foreach(string arquivo in arquivosDll)
    {
        //Carregar o assembly a partir do arquivo dll.
        var assembly = Assembly.LoadFrom(arquivo);
        assemblies.Add(assembly);
    }

    return assemblies;
}