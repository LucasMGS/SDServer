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
using BellmanFordAlgorithm;

namespace SDServer
{
    class Program
    {
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> clientSockets = new List<Socket>();
        private const int Porta = 3333;
        private const int BUFFER_SIZE = 2048;
        private static byte[] buffer = new byte[BUFFER_SIZE];
        private static Grafo grafo = new Grafo(3);


        static void Main()
        {
            Console.Title = "Server";
            ConfigSocket();
            Console.ReadLine();
            //grafo.Mostrar();
            //BellmanFord.Buscar(grafo, 28, 1);
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
            catch (ObjectDisposedException) 
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

            var dadosGrafo = DeserializarDTO();
            int noPartida = dadosGrafo.NoPartida;
            int noDestino = dadosGrafo.NoDestino;

            if (dadosGrafo.Operador == 1)
            {
                Console.WriteLine("Requisição de busca por nó!");
                EnviarTexto(string.Format("Iniciando busca pelo no {0} partindo do nó {1}", noDestino, noPartida));
                if (grafo.QntdVertices > noPartida && noPartida > 0)
                {
                    var menorCaminho = BellmanFord.Buscar(grafo, noPartida, noDestino);
                    EnviarTexto(string.Format(menorCaminho));
                }
                else
                {
                    EnviarTexto(string.Format("Não foi possível realizar a busca pois este nó de partida não existe no grafo!"));
                }
            }

            else if (dadosGrafo.Operador == 2)
            {
                Console.WriteLine("Requisição para solicitar roteamento!");
            }

            else if (dadosGrafo.Operador == 3)
            {
                Console.WriteLine("Fechando conexão!");
                FecharSockets();
                Console.WriteLine("Conexão Finalizada!");
                EnviarTexto("Conexão Finalizada!"); //avisa o client que foi encerrado a conexao
            }
            else
            {
                socketListening.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socketListening);
            }
        }

        private static void EnviarTexto(string texto)
        {
            byte[] bufferTexto = Encoding.ASCII.GetBytes(texto);
            server.Send(bufferTexto);
        }

        private static DtoGrafo DeserializarDTO()
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            var dados = (DtoGrafo)formatter.Deserialize(stream);
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
