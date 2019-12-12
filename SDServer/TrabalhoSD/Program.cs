using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TrabalhoSD;
using CompGrafica;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SDServer
{
    class Program
    {
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> clientSockets = new List<Socket>();
        private const int Porta = 3333;
        private const int BUFFER_SIZE = 2048;
        private static byte[] buffer = new byte[BUFFER_SIZE];

        static void Main()
        {
            Console.Title = "Server";
            ConfigSocket();
            Console.ReadLine();
            // CloseAllSockets();
            //var matrizAdj = criarMatrizAdj(10);
            //var grafo = CriarGrafo(matrizAdj);
            //var source = grafo.FirstOrDefault();

            //imprimirGrafo(grafo);
        }

        private static int[,] criarMatrizAdj(int tamanho)
        {
            int[,] matrizAdj = new int[tamanho, tamanho];
            var random = new Random();

            for (var i = 0; i < matrizAdj.GetLongLength(0); i++)
            {
                for (var j = 0; j < matrizAdj.GetLongLength(1); j++)
                {
                    if (i == j)
                    {
                        matrizAdj[i, j] = 0;
                    }
                    else
                    {
                        int randomNumber = random.Next(2);
                        matrizAdj[i, j] = randomNumber;
                    }
                }
            }
            return matrizAdj;
        }

        private static List<Aresta> CriarGrafo(int[,] matrizAdj)
        {
            var grafo = new List<Aresta>();
            var random = new Random();
            var tam = matrizAdj.GetLongLength(0);
            for (int i = 0; i < tam; i++)
            {
                for (int j = i + 1; j < tam; j++) // percorre o triangulo superior da matriz de adjacencia
                {
                    if (matrizAdj[i, j] == 1)
                    {
                        var rangeCusto = Int32.Parse(tam.ToString());
                        var custo = random.Next(-rangeCusto, rangeCusto);
                        if (matrizAdj[i, j] == 1)
                        {
                            grafo.Add(new Aresta(i, j, custo));
                            grafo.Add(new Aresta(j, i, custo));
                        }
                    }
                }
            }
            return grafo;
        }

        private static void imprimirGrafo(List<Aresta> Grafo)
        {
            foreach (var vertice in Grafo)
            {
                Console.WriteLine("Vértice {0} se conecta ao vértice {1} com custo {2}.", vertice.Partida, vertice.Destino, vertice.Custo);
            }
        }

        private static void ConfigSocket()
        {
            Console.WriteLine("Configurando servidor!");
            server.Bind(new IPEndPoint(IPAddress.Any, Porta));
            server.Listen(5);
            server.BeginAccept(AceitarCallback, null);
            Console.WriteLine("Servidor configurado!");
        }

        private static void AceitarCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = server.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.Broadcast, new AsyncCallback(ReceiveCallback), socket);
            Console.WriteLine("Client connectado, esperando requisições...");
            server.BeginAccept(new AsyncCallback(AceitarCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socketListening = (Socket)AR.AsyncState;
            int bytesLidos;
            try
            {
                bytesLidos = socketListening.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client fechou conexão");
                socketListening.Close();
                clientSockets.Remove(socketListening);
                return;
            }

            var dados = DeserializarDTO();

            if (dados.Operador == 1)
            {
                Console.WriteLine("Requisição de busca por no!");
                EnviarTexto(string.Format("Iniciando busca pelo no {0} partindo do no 0", dados.No));
            }

            else if (dados.Operador == 2)
            {
                Console.WriteLine("Requisição para solicitar roteamento!");
            }

            else if (dados.Operador == 3)
            {
                Console.WriteLine("Fechando conexão!");
                FecharSockets();
                Console.WriteLine("Conexão Finalizada!");
                EnviarTexto("Conexão Finalizada!"); //avisa o client que foi encerrado a conexao
            }
            else
            {
                socketListening.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.Broadcast, ReceiveCallback, socketListening);
            }
        }

        private static void EnviarTexto(string texto)
        {
            byte[] bufferTexto = Encoding.ASCII.GetBytes(texto);
            server.Send(bufferTexto);
        }

        private static DtoInformacao DeserializarDTO()
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            var dados = (DtoInformacao)formatter.Deserialize(stream);
            return dados;
        }

        private static void FecharSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            server.Close();
        }
    }
}
