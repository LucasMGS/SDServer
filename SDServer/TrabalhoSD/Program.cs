using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TrabalhoSD;
using CompGrafica;

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
                for (int j = i+1; j < tam; j++) // percorre o triangulo superior da matriz de adjacencia
                {
                    if (matrizAdj[i,j] == 1)
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
            //var hostName = Dns.GetHostName();
            //var listaIP = Dns.GetHostAddresses(hostName);
            //var IP = (from ipAdress in listaIP
            //          where ipAdress.AddressFamily == AddressFamily.InterNetwork
            //          select ipAdress).FirstOrDefault();

            //var ipEnd = new IPEndPoint(IP, Porta);

            Console.WriteLine("Configurando servidor!");
            server.Bind(new IPEndPoint(IPAddress.Any, Porta));
            server.Listen(5);
            server.BeginAccept(AceitarCallback, null);
            Console.WriteLine("Servidor configurado!");
        }

        private static void ConexaoLoop()
        {
            while (true)
            {
                try
                {
                    byte[] Buffer = new byte[server.ReceiveBufferSize];
                    Socket socketServer = server.Accept();
                    int bytesLidos = socketServer.Receive(Buffer);
                    byte[] BytesRecebidos = new byte[bytesLidos];
                    Array.Copy(Buffer, BytesRecebidos, bytesLidos);

                    var NoRecebido = Encoding.UTF8.GetString(BytesRecebidos);

                    DtoInformacao info = new DtoInformacao();
                    if (info.Operador == 1)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
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
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            Console.WriteLine("Client connectado, esperando requisições...");
            server.BeginAccept(new AsyncCallback(AceitarCallback), null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket sAtual = (Socket)AR.AsyncState;
            int bytesLidos;
            try
            {
                bytesLidos = sAtual.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                sAtual.Close();
                clientSockets.Remove(sAtual);
                return;
            }

            byte[] bytesRecebidos = new byte[bytesLidos];
            Array.Copy(buffer, bytesRecebidos, bytesLidos);
            var mensagem = Encoding.ASCII.GetString(bytesRecebidos);
            Console.WriteLine("Mensagem recebida: "+ mensagem);
            
            if(mensagem == "horas")
            {
                Console.WriteLine("Requisição de horas!");
                byte[] dados = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
                sAtual.Send(dados);
                Console.WriteLine("Dados enviados ao client");
            }
            else if (mensagem == "sair")
            {
                sAtual.Shutdown(SocketShutdown.Both);
                sAtual.Close();
                clientSockets.Remove(sAtual);
                Console.WriteLine("Client disconnected");
                return;
            }
            else
            {
                Console.WriteLine("Mensagem inválida");
                byte[] data = Encoding.ASCII.GetBytes("Mensagem Inválida");
                sAtual.Send(data);
                Console.WriteLine("Aviso enviado");
            }
            sAtual.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, sAtual);
        }
        private static void CloseAllSockets()
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
